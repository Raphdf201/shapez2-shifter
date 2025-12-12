using System;
using System.Collections.Generic;
using Core.Logging;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

#nullable enable
namespace ShapezShifter.Hijack
{
    internal class ConsoleInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly ILogger Logger;
        private readonly Hook ConsoleCommandsHook;

        public ConsoleInterceptor(IRewirerProvider rewirerProvider, ILogger logger)
        {
            RewirerProvider = rewirerProvider;
            Logger = logger;
            ConsoleCommandsHook = DetourHelper
                .CreatePostfixHook<GameSessionOrchestrator, IGameData>(
                    (orchestrator, gameData) => orchestrator.Init_9_ConsoleCommands(gameData),
                    SetupConsoleCommands);
        }

        private void SetupConsoleCommands(GameSessionOrchestrator orchestrator, IGameData gameData)
        {
            IDebugConsole console = orchestrator.DependencyContainer.Resolve<IDebugConsole>();

            IEnumerable<IConsoleRewirer> consoleRewirers = 
                RewirerProvider.RewirersOfType<IConsoleRewirer>();

            foreach (IConsoleRewirer consoleRewirer in consoleRewirers)
            {
                try
                {
                    consoleRewirer.RegisterConsoleCommand(RegisterCommand);
                }
                catch (Exception ex)
                {
                    Logger.Error?.Log($"Failed to register console commands from rewirer {consoleRewirer.GetType().Name}: {ex}");
                }
            }

            return;

            void RegisterCommand(string command, Action<DebugConsole.CommandContext> handler, bool isCheat,
                DebugConsole.ConsoleOption? arg1, DebugConsole.ConsoleOption? arg2)
            {
                Logger.Info?.Log($"Registering console command: {command}");
                if (arg1 != null && arg2 != null)
                {
                    console.Register(command, arg1, arg2, handler, isCheat);
                }
                else if (arg1 != null && arg2 == null)
                {
                    console.Register(command, arg1, handler, isCheat);
                }
                else if (arg1 == null && arg2 != null)
                {
                    console.Register(command, arg2, handler, isCheat);
                }
                else
                {
                    console.Register(command, handler, isCheat);
                }
            }
        }

        public void Dispose()
        {
            ConsoleCommandsHook.Dispose();
        }
    }
}
