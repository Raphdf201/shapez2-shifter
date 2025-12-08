using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow.SaveData
{
    public class ModSaveData<T> : ISaveDataRewirer where T : class, new()
    {
        private readonly string FileName;
        private T _data;

        public T Data
        {
            get => _data;
            set => _data = value;
        }

        public event Action<T> OnDataLoaded;

        public event Action<T> OnDataSaving;

        public ModSaveData(string fileName, T defaultData = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Filename cannot be null or empty", nameof(fileName));
            }

            if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            FileName = fileName;
            _data = defaultData ?? new T();
        }

        public void Reset(T defaultData = null)
        {
            _data = defaultData ?? new T();
        }

        void ISaveDataRewirer.OnSave(ISavegameBlobWriter writer)
        {
            try
            {
                OnDataSaving?.Invoke(_data);
                writer.WriteObjectAsJson(FileName, _data);
            }
            catch (Exception ex)
            {
                Debugging.Logger?.Error?.Log($"Failed to save mod data for {FileName}: {ex}");
            }
        }

        void ISaveDataRewirer.OnLoad(SavegameBlobReader reader)
        {
            try
            {
                _data = reader.ReadObjectFromJson<T>(FileName);
                OnDataLoaded?.Invoke(_data);
            }
            catch (Exception)
            {
                _data = new T();
                Debugging.Logger?.Info?.Log($"No existing save data found for {FileName}, using defaults");
            }
        }
    }
}
