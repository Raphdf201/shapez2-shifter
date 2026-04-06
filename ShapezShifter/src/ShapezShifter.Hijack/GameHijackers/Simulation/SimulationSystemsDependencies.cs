using Core.Logging;
using Game.Content.Features.Fluids;
using Game.Content.Features.Signals.Channels;
using Game.Core.Simulation;
using Game.Orchestration;

namespace ShapezShifter.Hijack
{
    public class SimulationSystemsDependencies
    {
        public readonly Ticks InitialSimulationTime;
        public readonly GameMode Mode;
        public readonly IShapeRegistry ShapeRegistry;
        public readonly IShapeIdManager ShapeIdManager;
        public readonly IGameResourcesMap ResourcesMap;
        public readonly IFluidRegistry FluidRegistry;
        public readonly FluidPackageItemSolver FluidPackagesItem;
        public readonly ISignalChannelRegistry SignalChannelRegistry;
        public readonly IResearchUnlockManager ResearchUnlockManager;
        public readonly ILogger Logger;

        public SimulationSystemsDependencies(BuiltinSimulationSystems builtinSimulationSystems)
        {
            InitialSimulationTime = builtinSimulationSystems.InitialSimulationTime;
            Mode = builtinSimulationSystems.Mode;
            ShapeRegistry = builtinSimulationSystems.ShapeRegistry;
            ShapeIdManager = builtinSimulationSystems.ShapeIdManager;
            ResourcesMap = builtinSimulationSystems.ResourcesMap;
            FluidRegistry = builtinSimulationSystems.FluidRegistry;
            FluidPackagesItem = builtinSimulationSystems.FluidPackagesItem;
            SignalChannelRegistry = builtinSimulationSystems.SignalChannelRegistry;
            ResearchUnlockManager = builtinSimulationSystems.ResearchUnlockManager;
            Logger = builtinSimulationSystems.Logger;
        }

        public SimulationSystemsDependencies(
            Ticks initialSimulationTime,
            GameMode mode,
            IShapeRegistry shapeRegistry,
            IShapeIdManager shapeIdManager,
            IGameResourcesMap resourcesMap,
            IFluidRegistry fluidRegistry,
            FluidPackageItemSolver fluidPackagesItem,
            ISignalChannelRegistry signalChannelRegistry,
            IResearchUnlockManager researchUnlockManager,
            ILogger logger)
        {
            InitialSimulationTime = initialSimulationTime;
            Mode = mode;
            ShapeRegistry = shapeRegistry;
            ShapeIdManager = shapeIdManager;
            ResourcesMap = resourcesMap;
            FluidRegistry = fluidRegistry;
            FluidPackagesItem = fluidPackagesItem;
            SignalChannelRegistry = signalChannelRegistry;
            ResearchUnlockManager = researchUnlockManager;
            Logger = logger;
        }
    }
}
