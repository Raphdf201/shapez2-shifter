using System;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class BuildingModulesInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly Hook InjectBuildingsModuleProvidersHook;

        public BuildingModulesInterceptor(IRewirerProvider rewirerProvider)
        {
            RewirerProvider = rewirerProvider;
            InjectBuildingsModuleProvidersHook =
                DetourHelper.CreatePostfixHook<GameSessionOrchestrator, BuildingsModulesLookup>(
                    original: (core, lookup) => core.InjectBuildingsModuleProviders(lookup),
                    postfix: Postfix);
        }

        private void Postfix(GameSessionOrchestrator gameCore, BuildingsModulesLookup modulesLookup)
        {
            var buildingModulesRewirers = RewirerProvider.RewirersOfType<IBuildingModulesRewirer>();

            foreach (IBuildingModulesRewirer buildingModulesRewirer in buildingModulesRewirers)
            {
                buildingModulesRewirer.AddModules(modulesLookup);
            }
        }

        public void Dispose()
        {
            InjectBuildingsModuleProvidersHook.Dispose();
        }
    }
}
