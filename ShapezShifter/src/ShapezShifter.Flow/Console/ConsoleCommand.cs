using System;
using ShapezShifter.Hijack;

#nullable enable
namespace ShapezShifter.Flow
{
    public static class ConsoleCommand
    {
        public static RewirerHandle Register(string commandName, Action<DebugConsole.CommandContext> handler, bool isCheat,
            DebugConsole.ConsoleOption? arg1, DebugConsole.ConsoleOption? arg2)
        {
            return GameRewirers.AddRewirer(new ConsoleCommandRewirer(commandName, handler, isCheat, arg1, arg2));
        }
    }
}
