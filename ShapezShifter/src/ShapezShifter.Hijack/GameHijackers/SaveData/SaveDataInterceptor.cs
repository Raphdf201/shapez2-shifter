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
                name: nameof(SavegameSerializer.SerializeSavegame),
                bindingAttr: BindingFlags.Public | BindingFlags.Instance);

            SerializeSavegameHook = new Hook(
                source: serializeMethod,
                target: new Func<
                    Func<SavegameSerializer, Savegame, SavegameSerializer.GameContext, MapSerializer,
                        SimulationStateSerializer, Viewport, IReadOnlyDictionary<string, byte[]>>, SavegameSerializer,
                    Savegame, SavegameSerializer.GameContext, MapSerializer, SimulationStateSerializer, Viewport,
                    IReadOnlyDictionary<string, byte[]>>(OnSerializeSavegameHook));

            DeserializePlayerHook = DetourHelper.CreatePrefixHook<Player, SavegameBlobReader, Viewport>(
                original: (player, reader, viewport) => player.Deserialize(reader, viewport),
                prefix: OnDeserializePlayerPrefix);
        }

        private IReadOnlyDictionary<string, byte[]> OnSerializeSavegameHook(
            Func<SavegameSerializer, Savegame, SavegameSerializer.GameContext, MapSerializer, SimulationStateSerializer,
                Viewport, IReadOnlyDictionary<string, byte[]>> orig,
            SavegameSerializer serializer,
            Savegame savegame,
            SavegameSerializer.GameContext context,
            MapSerializer mapSerializer,
            SimulationStateSerializer simulationStateSerializer,
            Viewport viewport)
        {
            var originalResult = orig(
                arg1: serializer,
                arg2: savegame,
                arg3: context,
                arg4: mapSerializer,
                arg5: simulationStateSerializer,
                arg6: viewport);

            Logger.Info?.Log("Intercepting savegame serialization for mod data");

            var saveDataRewirers = RewirerProvider.RewirersOfType<ISaveDataRewirer>();

            // Create a wrapper that can add to the existing serialized data
            var modWriter = new ModSaveDataBlobWriter(originalData: originalResult, logger: Logger);

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

        private (SavegameBlobReader, Viewport) OnDeserializePlayerPrefix(
            Player player,
            SavegameBlobReader reader,
            Viewport viewport)
        {
            Logger.Info?.Log("Intercepting savegame deserialization for mod data");

            // Get all save data rewirers
            var saveDataRewirers = RewirerProvider.RewirersOfType<ISaveDataRewirer>();

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

            return (reader, viewport);
        }

        public void Dispose()
        {
            SerializeSavegameHook?.Dispose();
            DeserializePlayerHook?.Dispose();
        }
    }
}
