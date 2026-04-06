using System.Collections.Generic;
using Game.Core.Rails;

namespace ShapezShifter
{
    public class IslandInitiatorsParams
    {
        public readonly GameBuildings Buildings;
        public readonly GameIslands Islands;
        public readonly IIslandsModulesLookup IslandsModulesLookup;
        public readonly IEntityPlacementRunner EntityPlacementRunner;
        public readonly ITutorialStateWriteAccess TutorialState;
        public readonly IDictionary<IEntityDefinition, PipettePlacementRequest> PipetteMap;
        public readonly ResearchUnlockProgressManager ProgressManager;
        public readonly IReadOnlyRailColorRegistry RailColorRegistry;
        public readonly ResearchChunkLimitManager ChunkLimitManager;
        public readonly IViewportLayersController ViewportLayersController;
        public readonly short MaxBuildingLayer;

        public IslandInitiatorsParams(IslandPlacersCreator placersCreator, short maxBuildingLayer) : this(
            buildings: placersCreator.Buildings,
            islands: placersCreator.Islands,
            maxBuildingLayer: maxBuildingLayer,
            progressManager: placersCreator.ProgressManager,
            entityPlacementRunner: placersCreator.EntityPlacementRunner,
            islandsModulesLookup: placersCreator.IslandsModulesLookup,
            pipetteMap: placersCreator.PipetteMap,
            tutorialState: placersCreator.TutorialState,
            chunkLimitManager: placersCreator.ChunkLimitManager,
            viewportLayersController: placersCreator.ViewportLayersController,
            railColorRegistry: placersCreator.RailColorRegistry) { }

        private IslandInitiatorsParams(
            GameBuildings buildings,
            GameIslands islands,
            short maxBuildingLayer,
            ResearchUnlockProgressManager progressManager,
            IEntityPlacementRunner entityPlacementRunner,
            IIslandsModulesLookup islandsModulesLookup,
            IDictionary<IEntityDefinition, PipettePlacementRequest> pipetteMap,
            ITutorialStateWriteAccess tutorialState,
            ResearchChunkLimitManager chunkLimitManager,
            IViewportLayersController viewportLayersController,
            IReadOnlyRailColorRegistry railColorRegistry)
        {
            Buildings = buildings;
            Islands = islands;
            MaxBuildingLayer = maxBuildingLayer;
            IslandsModulesLookup = islandsModulesLookup;
            EntityPlacementRunner = entityPlacementRunner;
            TutorialState = tutorialState;
            PipetteMap = pipetteMap;
            ProgressManager = progressManager;
            RailColorRegistry = railColorRegistry;
            ChunkLimitManager = chunkLimitManager;
            ViewportLayersController = viewportLayersController;
        }
    }
}
