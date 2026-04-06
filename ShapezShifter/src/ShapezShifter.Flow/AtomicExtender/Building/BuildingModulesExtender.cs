using System;
using Core.Events;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow.Atomic
{
    public class BuildingModulesExtender : IBuildingModulesRewirer, IChainableRewirer
    {
        private readonly IBuildingDefinition BuildingDefinition;
        private readonly IBuildingModulesData Data;

        public BuildingModulesExtender(IBuildingDefinition buildingDefinition, IBuildingModulesData data)
        {
            BuildingDefinition = buildingDefinition;
            Data = data;
        }

        public IEvent AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent _AfterExtensionApplied = new();

        public void AddModules(BuildingsModulesLookup modulesLookup)
        {
            IBuildingModules buildingModules = Data switch
            {
                BuildingModulesData processingModulesData => new ItemSimulationBuildingModuleDataProvider(
                    beltSpeed: GetBeltBuildingSpeedId(),
                    beltBuildingSpeed: processingModulesData.SpeedId,
                    beltBuildingDuration: processingModulesData.InitialProcessingDuration),
                CustomBuildingsModulesData customModulesData => customModulesData.Modules,
                _ => throw new Exception()
            };

            modulesLookup.AddModule(
                variant: BuildingDefinition.Id,
                buildingSimulationData: BuildingDefinition,
                buildingModulesProvider: buildingModules);
            _AfterExtensionApplied.Invoke();
        }

        private ResearchSpeedId GetBeltBuildingSpeedId()
        {
            return new ResearchSpeedId("BeltSpeed");
        }
    }
}
