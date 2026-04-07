using Game.Core.Simulation;

namespace ShapezShifter.Flow
{
    internal class BuildingBuilder
        : IBuildingBuilder,
          IIdentifiableBuildingBuilder,
          IIdentifiableConnectableBuildingBuilder,
          IIdentifiableConnectableDynamicallyRenderableBuildingBuilder,
          IIdentifiableConnectableRenderableBuildingBuilder,
          IIdentifiableConnectableRenderablePredictableAudibleBuildingBuilder,
          IIdentifiableConnectableRenderablePredictableAudibleConfigurableBuildingBuilder
    {
        private readonly BuildingDefinitionId DefinitionId;
        private IBuildingConnectorData ConnectorData;

        private BuildingDefinition BuildingDefinition;
        private BuildingDefinitionId? DefinitionToCopyFrom;

        public BuildingBuilder(BuildingDefinitionId id)
        {
            DefinitionId = id;
        }

        public IIdentifiableConnectableBuildingBuilder WithConnectorData(IBuildingConnectorData connectorData)
        {
            ConnectorData = connectorData;
            BuildingDefinition runtimeDefinition = new(id: DefinitionId, connectorData: ConnectorData);
            runtimeDefinition.CustomData.Attach(ConnectorData);
            BuildingDefinition = runtimeDefinition;
            return this;
        }

        public IIdentifiableConnectableDynamicallyRenderableBuildingBuilder DynamicallyRendering<
            TRenderer, TSimulation, TDrawData>(TDrawData drawData)
            where TRenderer : StatelessBuildingSimulationRenderer<TSimulation, TDrawData>
            where TSimulation : ISimulation
            where TDrawData : IBuildingCustomDrawData
        {
            BuildingDefinition.CustomData.Attach(drawData);
            return this;
        }

        public IIdentifiableConnectableRenderableBuildingBuilder WithStaticDrawData(BuildingDrawData drawData)
        {
            BuildingDefinition.CustomData.Attach(drawData);
            return this;
        }

        public IIdentifiableConnectableRenderableBuildingBuilder WithCopiedStaticDrawData(
            BuildingDefinitionId definitionId)
        {
            DefinitionToCopyFrom = definitionId;
            return this;
        }

        public IIdentifiableConnectableRenderableBuildingBuilder WithPrediction(ISimulationSystem predictor)
        {
            return this;
        }

        public IIdentifiableConnectableRenderableBuildingBuilder WithoutPrediction()
        {
            return this;
        }

        public IIdentifiableConnectableRenderablePredictableAudibleBuildingBuilder WithSound(
            BuildingSoundDefinition buildingSoundDefinition)
        {
            BuildingDefinition.CustomData.Attach(buildingSoundDefinition);
            return this;
        }

        public IIdentifiableConnectableRenderablePredictableAudibleBuildingBuilder WithoutSound()
        {
            BuildingDefinition.CustomData.Attach(
                new BuildingSoundDefinition(
                    soundLOD: SoundLOD.None,
                    soundEffectPriority: SoundPriority.Disabled,
                    customSoundData: null));
            return this;
        }

        public IIdentifiableConnectableRenderablePredictableAudibleConfigurableBuildingBuilder
            WithSimulationConfiguration(ICustomSimulationConfiguration customSimulationConfiguration)
        {
            BuildingDefinition.CustomData.Attach(customSimulationConfiguration);
            return this;
        }

        public IIdentifiableConnectableRenderablePredictableAudibleConfigurableBuildingBuilder
            WithoutSimulationConfiguration()
        {
            BuildingDefinition.CustomData.Attach(new EmptyCustomSimulationConfiguration());

            return this;
        }

        public IBuildingBuilder WithEfficiencyData(BuildingEfficiencyData buildingEfficiencyData)
        {
            BuildingEfficiencyData efficiencyData = new(baseProcessingDuration: 2.0f, processingLaneCount: 1);
            BuildingDefinition.CustomData.Attach(efficiencyData);
            return this;
        }

        public IBuildingBuilder WithoutEfficiencyData()
        {
            return this;
        }

        public BuildingDefinition BuildAndRegister(BuildingDefinitionGroup group, GameBuildings gameBuildings)
        {
            CopyBuildingRenderingFromExisting(gameBuildings);
            BindBuildingToGroup(group);

            gameBuildings._DefinitionsById.Add(key: BuildingDefinition.Id, value: BuildingDefinition);

            return BuildingDefinition;
        }

        private void CopyBuildingRenderingFromExisting(GameBuildings gameBuildings)
        {
            if (!DefinitionToCopyFrom.HasValue)
            {
                return;
            }

            IBuildingDefinition definitionToCopyFrom = gameBuildings.GetDefinition(DefinitionToCopyFrom.Value);
            var drawDataReference = definitionToCopyFrom.CustomData.Get<IBuildingDrawData>();
            var dynamicDrawData = BuildingDefinition.CustomData.Get<IBuildingCustomDrawData>();
            BuildingDrawData drawData = new(
                renderVoidBelow: drawDataReference.RenderVoidBelow,
                mainMeshPerLayer: drawDataReference.MainMeshPerLayer,
                isolatedBlueprintMesh: drawDataReference.IsolatedBlueprintMesh,
                combinedBlueprintMesh: drawDataReference.CombinedBlueprintMesh,
                previewMesh: drawDataReference.PreviewMesh,
                glassMesh: drawDataReference.GlassMesh,
                colliders: drawDataReference.Colliders,
                customDrawData: dynamicDrawData,
                hasCustomOverviewMesh: drawDataReference.HasCustomOverviewMesh,
                customOverviewMesh: drawDataReference.CustomOverviewMesh,
                simulationRendererDrawsMainMesh: drawDataReference.SimulationRendererDrawsMainMesh);
            BuildingDefinition.CustomData.AttachOrReplace(drawData);
        }

        private void BindBuildingToGroup(BuildingDefinitionGroup group)
        {
            EntityPlacementPreferenceData placementPreferenceData = new(
                autoSnapToConnectors: group.AutoConnect,
                connectorsAutoSnapScoreMultiplier: group.AutoAttractIOScoreMultiplier);

            EntityReplacementPreferenceData replacementPreferenceData = new(
                allowNonForcingReplacementByEntitiesInDifferentGroup: group.AllowNonForcingReplacementByOtherBuildings,
                isTransportBuilding: group.IsTransportBuilding,
                shouldSkipReplacementIOChecks: group.ShouldSkipReplacementIOChecks);

            BuildingDefinition.CustomData.AttachOrReplace(group);
            BuildingDefinition.CustomData.AttachOrReplace(placementPreferenceData);
            BuildingDefinition.CustomData.AttachOrReplace(replacementPreferenceData);
            BuildingDefinition.CustomData.AttachOrReplace(group.DefaultPreferredPlacementMode);

            group.AddInternalVariant(BuildingDefinition);
        }
    }

    public interface IIdentifiableBuildingBuilder
    {
        IIdentifiableConnectableBuildingBuilder WithConnectorData(IBuildingConnectorData connectorData);
    }

    public interface IIdentifiableConnectableBuildingBuilder
    {
        IIdentifiableConnectableDynamicallyRenderableBuildingBuilder DynamicallyRendering<
            TRenderer, TSimulation, TDrawData>(TDrawData drawData)
            where TRenderer : StatelessBuildingSimulationRenderer<TSimulation, TDrawData>
            where TSimulation : ISimulation
            where TDrawData : IBuildingCustomDrawData;

        IIdentifiableConnectableRenderableBuildingBuilder WithCopiedStaticDrawData(BuildingDefinitionId definitionId);
    }

    public interface IIdentifiableConnectableDynamicallyRenderableBuildingBuilder
    {
        IIdentifiableConnectableRenderableBuildingBuilder WithStaticDrawData(BuildingDrawData drawData);

        IIdentifiableConnectableRenderableBuildingBuilder WithCopiedStaticDrawData(BuildingDefinitionId definitionId);
    }

    public interface IIdentifiableConnectableRenderableBuildingBuilder
    {
        IIdentifiableConnectableRenderablePredictableAudibleBuildingBuilder WithSound(
            BuildingSoundDefinition soundDefinition);

        IIdentifiableConnectableRenderablePredictableAudibleBuildingBuilder WithoutSound();
    }

    public interface IIdentifiableConnectableRenderablePredictableAudibleBuildingBuilder
    {
        IIdentifiableConnectableRenderablePredictableAudibleConfigurableBuildingBuilder WithSimulationConfiguration(
            ICustomSimulationConfiguration customSimulationConfiguration);

        IIdentifiableConnectableRenderablePredictableAudibleConfigurableBuildingBuilder
            WithoutSimulationConfiguration();
    }

    public interface IIdentifiableConnectableRenderablePredictableAudibleConfigurableBuildingBuilder
    {
        IBuildingBuilder WithEfficiencyData(BuildingEfficiencyData buildingEfficiencyData);

        IBuildingBuilder WithoutEfficiencyData();
    }
}
