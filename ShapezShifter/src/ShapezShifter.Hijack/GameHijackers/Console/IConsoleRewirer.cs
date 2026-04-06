#nullable enable
using System;

namespace ShapezShifter.Hijack
{
    public interface IConsoleRewirer : IRewirer
    {
        void RegisterConsoleCommand(
            Action<string, Action<DebugConsole.CommandContext>, bool, DebugConsole.ConsoleOption?,
                DebugConsole.ConsoleOption?> registerCommand);
    }
}
