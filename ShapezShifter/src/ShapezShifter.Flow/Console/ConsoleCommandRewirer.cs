#nullable enable
using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
{
    internal class ConsoleCommandRewirer : IConsoleRewirer
    {
        private readonly string CommandName;
        private readonly DebugConsole.ConsoleOption? Arg1;
        private readonly DebugConsole.ConsoleOption? Arg2;
        private readonly Action<DebugConsole.CommandContext> Handler;
        private readonly bool IsCheat;

        public ConsoleCommandRewirer(
            string commandName,
            Action<DebugConsole.CommandContext> handler,
            bool isCheat,
            DebugConsole.ConsoleOption? arg1,
            DebugConsole.ConsoleOption? arg2)
        {
            CommandName = commandName;
            Handler = handler;
            IsCheat = isCheat;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public void RegisterConsoleCommand(
            Action<string, Action<DebugConsole.CommandContext>, bool, DebugConsole.ConsoleOption?,
                DebugConsole.ConsoleOption?> registerCommand)
        {
            registerCommand(arg1: CommandName, arg2: Handler, arg3: IsCheat, arg4: Arg1, arg5: Arg2);
        }
    }
}
