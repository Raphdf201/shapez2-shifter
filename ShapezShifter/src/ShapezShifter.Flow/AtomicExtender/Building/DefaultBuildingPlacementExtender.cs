using Core.Events;
using ShapezShifter.Hijack;
using Unity.Core.Logging;

namespace ShapezShifter.Flow.Atomic
{
    public class DefaultBuildingPlacementExtender
        : IShapeBuildingPlacementRewirers, IChainableRewirer<BuildingPlacementResult>
    {
        private readonly IBuildingDefinition BuildingDefinition;

        public DefaultBuildingPlacementExtender(IBuildingDefinition buildingDefinition)
        {
            BuildingDefinition = buildingDefinition;
        }

        public void ModifyBuildingPlacers(
            BuildingInitiatorsParams @params,
            IPlacementInitiatorIdRegistry placementRegistry)
        {
            CreatePlacementInitiator(buildingInitiatorsParams: @params, placementRegistry: placementRegistry);
        }

        private void CreatePlacementInitiator(
            BuildingInitiatorsParams buildingInitiatorsParams,
            IPlacementInitiatorIdRegistry placementRegistry)
        {
            ShapeBuildingsPlacersCreator buildingsCreator = new(
                buildings: buildingInitiatorsParams.Buildings,
                progressManager: buildingInitiatorsParams.ProgressManager,
                entityPlacementRunner: buildingInitiatorsParams.EntityPlacementRunner,
                buildingModules: buildingInitiatorsParams.BuildingsModules,
                pipetteMap: buildingInitiatorsParams.PipetteMap,
                tutorialState: (ITutorialState)buildingInitiatorsParams.TutorialState,
                viewportLayersController: buildingInitiatorsParams.ViewportLayerController,
                logger: new UnityLogger());

            IPlacementInitiator placer = buildingsCreator.CreateDefaultPlacer(BuildingDefinition);

            PlacementInitiatorId placementInitiatorId = placementRegistry.RegisterInitiator(
                serialId: new SerializedPlacerId($"{BuildingDefinition.Id.Name}Initiator"),
                placementInitiator: placer);
            BuildingPlacementResult result = new(initiatorId: placementInitiatorId, building: BuildingDefinition);
            _AfterExtensionApplied.Invoke(result);
        }

        public IEvent<BuildingPlacementResult> AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent<BuildingPlacementResult> _AfterExtensionApplied = new();
    }
}
