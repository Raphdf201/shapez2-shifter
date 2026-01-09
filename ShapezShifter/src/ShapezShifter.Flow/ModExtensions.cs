using System;
using System.Linq;
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
        /// <returns>A configured ModSaveData instance and its Rewirer handle</returns>
        public static (ModSaveData<T>, RewirerHandle) CreateSaveDataWithHandle<T>(this IMod mod, string dataName = "data") 
            where T : class, new()
        {
            // Generate filename from mod assembly name
            string assemblyName = mod.GetType().Assembly.GetName().Name;
            string fileName = $"{assemblyName}-{dataName}.json";
            
            return ModSaveData<T>.Register(fileName);
        }
        
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
            
            return ModSaveData<T>.Register(fileName).Item1;
        }

        /// <summary>
        /// Register a console command for this mod
        /// </summary>
        /// <param name="mod">The mod instance</param>
        /// <param name="commandName">The command name (e.g., "hello"). Must be lowercase</param>
        /// <param name="handler">The command handler</param>
        /// <param name="isCheat">Whether the command needs cheats enabled to run or not</param>
        /// <param name="arg1">The first argument of the command (optional)</param>
        /// <param name="arg2">The second argument of the command (optional)</param>
        /// <param name="useAssemblyPrefix">Whether to prefix the command with the assembly name</param>
        /// <returns>A rewirer handle that can be used to unregister the command</returns>
        public static RewirerHandle RegisterConsoleCommand(this IMod mod, string commandName,
            Action<DebugConsole.CommandContext> handler, bool isCheat = false, DebugConsole.ConsoleOption? arg1 = null,
            DebugConsole.ConsoleOption? arg2 = null, bool useAssemblyPrefix = true)
        {
            var processedName = commandName;
            if (useAssemblyPrefix)
            {
                processedName = mod.GetType().Assembly.GetName().Name + "." + commandName;
            }
            if (processedName.Any(char.IsUpper))
            {
                Debugging.Logger?.Warning?.Log("Console commands can't contain uppercase characters. The command has been lowercased");
                processedName = processedName.ToLower();
            }
            return ConsoleCommand.Register(processedName, handler, isCheat, arg1, arg2);
        }

        public static RewirerHandle RunPeriodically(this IMod mod, Action<GameSessionOrchestrator, float> action)
        {
            return TickRewirer.Register(action);
        }
    }
}
