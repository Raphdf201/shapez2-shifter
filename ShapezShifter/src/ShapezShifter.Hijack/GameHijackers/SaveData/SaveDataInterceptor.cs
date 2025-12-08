using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Logging;
using Game.Core.Savegame;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class SaveDataInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly ILogger Logger;
        private readonly Hook SerializeSavegameHook;
        private readonly Hook DeserializePlayerHook;

        public SaveDataInterceptor(IRewirerProvider rewirerProvider, ILogger logger)
        {
            RewirerProvider = rewirerProvider;
            Logger = logger;

            // Use reflection to get the method since it has too many parameters for the helper
            MethodInfo serializeMethod = typeof(SavegameSerializer).GetMethod(
                nameof(SavegameSerializer.SerializeSavegame),
                BindingFlags.Public | BindingFlags.Instance);

            SerializeSavegameHook = new Hook(
                serializeMethod,
                new Func<Func<SavegameSerializer, Savegame, SavegameSerializer.GameContext, 
                    MapSerializer, SimulationStateSerializer, Viewport, IReadOnlyDictionary<string, byte[]>>,
                    SavegameSerializer, Savegame, SavegameSerializer.GameContext,
                    MapSerializer, SimulationStateSerializer, Viewport, IReadOnlyDictionary<string, byte[]>>(
                    OnSerializeSavegameHook));

            DeserializePlayerHook = DetourHelper.CreatePrefixHook<
                Player,
                SavegameBlobReader,
                ResearchProgression,
                Viewport>(
                (player, reader, layout, viewport) => player.Deserialize(reader, layout, viewport),
                OnDeserializePlayerPrefix);
        }

        private IReadOnlyDictionary<string, byte[]> OnSerializeSavegameHook(
            Func<SavegameSerializer, Savegame, SavegameSerializer.GameContext, 
                MapSerializer, SimulationStateSerializer, Viewport, IReadOnlyDictionary<string, byte[]>> orig,
            SavegameSerializer serializer,
            Savegame savegame,
            SavegameSerializer.GameContext context,
            MapSerializer mapSerializer,
            SimulationStateSerializer simulationStateSerializer,
            Viewport viewport)
        {
            IReadOnlyDictionary<string, byte[]> originalResult = orig(
                serializer, savegame, context, mapSerializer, simulationStateSerializer, viewport);

            Logger.Info?.Log("Intercepting savegame serialization for mod data");

            IEnumerable<ISaveDataRewirer> saveDataRewirers =
                RewirerProvider.RewirersOfType<ISaveDataRewirer>();

            // Create a wrapper that can add to the existing serialized data
            ModSaveDataBlobWriter modWriter = new ModSaveDataBlobWriter(originalResult, Logger);

            foreach (ISaveDataRewirer rewirer in saveDataRewirers)
            {
                try
                {
                    rewirer.OnSave(modWriter);
                }
                catch (Exception ex)
                {
                    Logger.Error?.Log($"Error in mod save data rewirer: {ex}");
                }
            }

            return modWriter.GetCombinedData();
        }

        private (SavegameBlobReader, ResearchProgression, Viewport) OnDeserializePlayerPrefix(
            Player player,
            SavegameBlobReader reader,
            ResearchProgression layout,
            Viewport viewport)
        {
            Logger.Info?.Log("Intercepting savegame deserialization for mod data");

            // Get all save data rewirers
            IEnumerable<ISaveDataRewirer> saveDataRewirers =
                RewirerProvider.RewirersOfType<ISaveDataRewirer>();

            foreach (ISaveDataRewirer rewirer in saveDataRewirers)
            {
                try
                {
                    rewirer.OnLoad(reader);
                }
                catch (Exception ex)
                {
                    Logger.Error?.Log($"Error in mod load data rewirer: {ex}");
                }
            }

            return (reader, layout, viewport);
        }

        public void Dispose()
        {
            SerializeSavegameHook?.Dispose();
            DeserializePlayerHook?.Dispose();
        }
    }
}
