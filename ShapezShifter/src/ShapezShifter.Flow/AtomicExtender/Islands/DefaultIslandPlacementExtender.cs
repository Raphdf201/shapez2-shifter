using Core.Events;
using ShapezShifter.Hijack;
using Unity.Core.Logging;

namespace ShapezShifter.Flow.Atomic
{
    public class DefaultIslandPlacementExtender
        : IPlatformIslandPlacementRewirers, IChainableRewirer<IslandPlacementResult>
    {
        private readonly IIslandDefinition IslandDefinition;

        public DefaultIslandPlacementExtender(IIslandDefinition islandDefinition)
        {
            IslandDefinition = islandDefinition;
        }

        public IEvent<IslandPlacementResult> AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent<IslandPlacementResult> _AfterExtensionApplied = new();

        public void ModifyIslandPlacers(
            IslandInitiatorsParams islandInitiatorsParams,
            IPlacementInitiatorIdRegistry placementRegistry)
        {
            PlatformIslandsPlacersCreators islandsPlacers = new(
                buildings: islandInitiatorsParams.Buildings,
                islands: islandInitiatorsParams.Islands,
                maxBuildingLayer: islandInitiatorsParams.MaxBuildingLayer,
                progressManager: islandInitiatorsParams.ProgressManager,
                entityPlacementRunner: islandInitiatorsParams.EntityPlacementRunner,
                islandsModulesLookup: islandInitiatorsParams.IslandsModulesLookup,
                pipetteMap: islandInitiatorsParams.PipetteMap,
                tutorialState: (ITutorialState)islandInitiatorsParams.TutorialState,
                chunkLimitManager: islandInitiatorsParams.ChunkLimitManager,
                viewportLayersController: islandInitiatorsParams.ViewportLayersController,
                railColorRegistry: islandInitiatorsParams.RailColorRegistry,
                logger: new UnityLogger());

            IPlacementInitiator placer = islandsPlacers.CreateDefaultPlacer(IslandDefinition);

            PlacementInitiatorId placementInitiatorId = placementRegistry.RegisterInitiator(
                serialId: new SerializedPlacerId($"{IslandDefinition.Id.Name}Initiator"),
                placementInitiator: placer);
            IslandPlacementResult result = new(initiatorId: placementInitiatorId, island: IslandDefinition);
            _AfterExtensionApplied.Invoke(result);
        }
    }
}
