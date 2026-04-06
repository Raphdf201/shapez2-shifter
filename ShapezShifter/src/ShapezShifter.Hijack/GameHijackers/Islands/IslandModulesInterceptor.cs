using System;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class IslandModulesInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly Hook InjectIslandsModuleProvidersHook;

        public IslandModulesInterceptor(IRewirerProvider rewirerProvider)
        {
            RewirerProvider = rewirerProvider;
            InjectIslandsModuleProvidersHook =
                DetourHelper.CreatePostfixHook<GameSessionOrchestrator, IslandsModulesLookup>(
                    original: (core, lookup) => core.InjectIslandsModuleProviders(lookup),
                    postfix: Postfix);
        }

        private void Postfix(GameSessionOrchestrator gameCore, IslandsModulesLookup modulesLookup)
        {
            var islandModulesRewirers = RewirerProvider.RewirersOfType<IIslandModulesRewirer>();

            foreach (IIslandModulesRewirer islandModulesRewirer in islandModulesRewirers)
            {
                islandModulesRewirer.AddModules(modulesLookup);
            }
        }

        public void Dispose()
        {
            InjectIslandsModuleProvidersHook.Dispose();
        }
    }
}
