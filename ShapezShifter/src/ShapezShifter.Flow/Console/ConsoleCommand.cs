#nullable enable
using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
{
    public static class ConsoleCommand
    {
        public static RewirerHandle Register(
            string commandName,
            Action<DebugConsole.CommandContext> handler,
            bool isCheat,
            DebugConsole.ConsoleOption? arg1,
            DebugConsole.ConsoleOption? arg2)
        {
            return GameRewirers.AddRewirer(
                new ConsoleCommandRewirer(
                    commandName: commandName,
                    handler: handler,
                    isCheat: isCheat,
                    arg1: arg1,
                    arg2: arg2));
        }
    }
}
