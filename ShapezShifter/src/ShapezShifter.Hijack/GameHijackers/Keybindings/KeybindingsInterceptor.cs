using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class KeybindingsInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly ILogger Logger;
        private readonly Hook ComputeDefaultsHook;

        public KeybindingsInterceptor(IRewirerProvider rewirerProvider, ILogger logger)
        {
            RewirerProvider = rewirerProvider;
            Logger = logger;

            ComputeDefaultsHook = DetourHelper.CreateStaticPostfixHook(
                typeof(DefaultKeybindings),
                () => DefaultKeybindings.ComputeDefaults(),
                InjectCustomKeybindings);
        }

        private KeybindingsLayer[] InjectCustomKeybindings(KeybindingsLayer[] originalLayers)
        {
            IEnumerable<IKeybindingsRewirer> rewirers = RewirerProvider.RewirersOfType<IKeybindingsRewirer>();
            
            if (!rewirers.Any())
            {
                return originalLayers;
            }

            Logger.Info?.Log("Injecting custom keybindings...");

            List<KeybindingsLayer> layersList = originalLayers.ToList();

            foreach (IKeybindingsRewirer rewirer in rewirers)
            {
                try
                {
                    rewirer.ModifyKeybindingLayers(layersList);
                }
                catch (Exception ex)
                {
                    Logger.Error?.Log($"Error applying keybindings rewirer {rewirer.GetType().Name}: {ex}");
                }
            }

            Logger.Info?.Log($"Injected custom keybindings. Total layers: {layersList.Count}");
            return layersList.ToArray();
        }

        public void Dispose()
        {
            ComputeDefaultsHook?.Dispose();
        }
    }
}
