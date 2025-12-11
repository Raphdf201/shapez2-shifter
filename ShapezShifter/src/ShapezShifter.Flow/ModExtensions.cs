using System;
using ShapezShifter.Hijack;

#nullable enable
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

        /// <summary>
        /// Register a console command for this mod
        /// </summary>
        /// <param name="mod">The mod instance</param>
        /// <param name="commandName">The command name (e.g., "mymod.hello")</param>
        /// <param name="handler">The command handler</param>
        /// <param name="isCheat">Weather the command needs cheats enabled to run or not</param>
        /// <param name="arg1">The first argument of the command (optional)</param>
        /// <param name="arg2">The second argument of the command (optional)</param>
        /// <returns>A rewirer handle that can be used to unregister the command</returns>
        public static RewirerHandle RegisterConsoleCommand(this IMod mod, string commandName,
            Action<DebugConsole.CommandContext> handler, bool isCheat = false, DebugConsole.ConsoleOption? arg1 = null,
            DebugConsole.ConsoleOption? arg2 = null)
        {
            return ConsoleCommand.Register(commandName, handler, isCheat, arg1, arg2);
        }
    }
}
