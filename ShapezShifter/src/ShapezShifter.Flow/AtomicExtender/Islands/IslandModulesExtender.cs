using System;
using System.Collections.Generic;
using Core.Events;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow.Atomic
{
    public class IslandModulesExtender : IIslandModulesRewirer, IChainableRewirer
    {
        private readonly IIslandDefinition IslandDefinition;
        private readonly IIslandModulesData Data;

        public IslandModulesExtender(IIslandDefinition buildingDefinition, IIslandModulesData data)
        {
            IslandDefinition = buildingDefinition;
            Data = data;
        }

        public IEvent AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent _AfterExtensionApplied = new();

        public void AddModules(IslandsModulesLookup modulesLookup)
        {
            IIslandModuleDataProvider modules = Data switch
            {
                CustomIslandsModulesData customModulesData => customModulesData.Modules,
                NoModulesData => new NoModulesProvider(),
                _ => throw new Exception()
            };

            modulesLookup.AddModuleProvider(definition: IslandDefinition.Id, islandModuleDataProvider: modules);
            _AfterExtensionApplied.Invoke();
        }
    }

    public class NoModulesProvider : IIslandModuleDataProvider
    {
        public IEnumerable<IHUDSidePanelModuleData> GetStats()
        {
            yield break;
        }

        public IEnumerable<IHUDSidePanelModuleData> GetModules(IslandModel island)
        {
            yield break;
        }
    }
}
