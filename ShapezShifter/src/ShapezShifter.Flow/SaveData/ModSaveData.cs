using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
{
    public class ModSaveData<T> : ISaveDataRewirer
        where T : class, new()
    {
        private readonly string FileName;

        public T Data { get; set; }

        private Action<T> _onDataLoaded;

        public event Action<T> OnDataLoaded
        {
            add { _onDataLoaded += value; }
            remove { _onDataLoaded -= value; }
        }

        private Action<T> _onDataSaving;

        public event Action<T> OnDataSaving
        {
            add { _onDataSaving += value; }
            remove { _onDataSaving -= value; }
        }

        private ModSaveData(string fileName, T defaultData = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(message: "Filename cannot be null or empty", paramName: nameof(fileName));
            }

            if (!fileName.EndsWith(value: ".json", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            FileName = fileName;
            Data = defaultData ?? new T();
        }

        public void Reset(T defaultData = null)
        {
            Data = defaultData ?? new T();
        }

        void ISaveDataRewirer.OnSave(ISavegameBlobWriter writer)
        {
            try
            {
                _onDataSaving?.Invoke(Data);
                writer.WriteObjectAsJson(filename: FileName, obj: Data);
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
                Data = reader.ReadObjectFromJson<T>(FileName);
                _onDataLoaded?.Invoke(Data);
            }
            catch (Exception)
            {
                Data = new T();
                Debugging.Logger?.Info?.Log($"No existing save data found for {FileName}, using defaults");
            }
        }

        internal static (ModSaveData<T>, RewirerHandle) Register(string fileName, T defaultData = null)
        {
            var obj = new ModSaveData<T>(fileName: fileName, defaultData: defaultData);
            RewirerHandle handle = GameRewirers.AddRewirer(obj);
            return (obj, handle);
        }
    }
}
