using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Logging;
using Newtonsoft.Json;

namespace ShapezShifter.Hijack
{
    /// <summary>
    /// A wrapper around the game's savegame blob writer that allows mods to add custom data files.
    /// </summary>
    internal class ModSaveDataBlobWriter : ISavegameBlobWriter
    {
        private readonly IReadOnlyDictionary<string, byte[]> OriginalData;
        private readonly ConcurrentDictionary<string, byte[]> ModData;
        private readonly ILogger Logger;

        public Encoding Encoding
        {
            get { return SavegameSerializer.Encoding; }
        }

        public ModSaveDataBlobWriter(IReadOnlyDictionary<string, byte[]> originalData, ILogger logger)
        {
            OriginalData = originalData;
            ModData = new ConcurrentDictionary<string, byte[]>();
            Logger = logger;
        }

        public void Write(string filename, System.Action<BinaryStringLUTSerializationVisitor> handler)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Note: We can't create a full BinaryStringLUTSerializationVisitor here without access to all dependencies
                // Mods should use WriteObjectAsJson for simplicity
                Logger.Warning?.Log(
                    $"Binary write not fully supported in mod save data. Use WriteObjectAsJson instead. File: {filename}");
            }
        }

        public void WriteObjectAsJson<T>(string filename, T obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(value: obj, settings: SavegameSerializer.JsonSettings);
                byte[] data = Encoding.GetBytes(json);

                if (ModData.TryAdd(key: filename, value: data))
                {
                    Logger.Info?.Log($"Mod added save data file: {filename}");
                }
                else
                {
                    Logger.Warning?.Log($"Mod tried to add duplicate save data file: {filename}");
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error?.Log($"Failed to serialize mod save data for {filename}: {ex}");
            }
        }

        public IReadOnlyDictionary<string, byte[]> GetCombinedData()
        {
            // Combine original game data with mod data
            var combined = new Dictionary<string, byte[]>(OriginalData);

            foreach (var modEntry in ModData)
            {
                combined[modEntry.Key] = modEntry.Value;
            }

            return combined;
        }
    }
}
