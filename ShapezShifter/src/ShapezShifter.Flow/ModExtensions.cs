using ShapezShifter.Flow.SaveData;

namespace ShapezShifter.Flow
{
    /// <summary>
    /// Extension methods to make mod development easier
    /// </summary>
    public static class ModExtensions
    {
        /// <summary>
        /// Creates a ModSaveData instance with automatic filename generation based on the mod's assembly.
        /// </summary>
        /// <typeparam name="T">The type of data to save/load</typeparam>
        /// <param name="mod">The mod instance</param>
        /// <param name="dataName">Optional custom name for the data file (defaults to "data")</param>
        /// <returns>A configured ModSaveData instance</returns>
        public static ModSaveData<T> CreateSaveData<T>(this IMod mod, string dataName = "data") 
            where T : class, new()
        {
            // Generate filename from mod assembly name
            string assemblyName = mod.GetType().Assembly.GetName().Name;
            string fileName = $"{assemblyName}-{dataName}.json";
            
            return new ModSaveData<T>(fileName);
        }
    }
}
