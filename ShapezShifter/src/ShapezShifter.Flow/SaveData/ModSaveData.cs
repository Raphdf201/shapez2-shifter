using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
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

        private Action<T> _onDataLoaded;
        public event Action<T> OnDataLoaded
        {
            add => _onDataLoaded += value;
            remove => _onDataLoaded -= value;
        }

        private Action<T> _onDataSaving;
        public event Action<T> OnDataSaving
        {
            add => _onDataSaving += value;
            remove => _onDataSaving -= value;
        }

        private ModSaveData(string fileName, T defaultData = null)
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
                _onDataSaving?.Invoke(_data);
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
                _onDataLoaded?.Invoke(_data);
            }
            catch (Exception)
            {
                _data = new T();
                Debugging.Logger?.Info?.Log($"No existing save data found for {FileName}, using defaults");
            }
        }

        internal static (ModSaveData<T>, RewirerHandle) Register(string fileName, T defaultData = null)
        {
            var obj = new ModSaveData<T>(fileName, defaultData);
            var handle = GameRewirers.AddRewirer(obj);
            return (obj, handle);
        }
    }
}
