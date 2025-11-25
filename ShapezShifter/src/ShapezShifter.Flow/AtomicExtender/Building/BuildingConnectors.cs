using System.Collections.Generic;
using Game.Core.Coordinates;

namespace ShapezShifter.Flow.Atomic
{
    public static class BuildingConnectors
    {
        public static ISingleTileConnectorDataBuilder SingleTile()
        {
            return new SingleTileBuildingConnectorDataBuilder();
        }

        // public static IMultiTileConnectorDataBuilder MultiTile()
        // {
        //     throw new NotImplementedException();
        // }
    }

    public class SingleTileBuildingConnectorDataBuilder : ISingleTileConnectorDataBuilder
    {
        private readonly List<BuildingBaseIO> BuildingConnectors = new();

        public ISingleTileConnectorDataBuilder AddShapeInput(ShapeConnectorConfig shapeConnectorConfig)
        {
            BuildingConnectors.Add(new BuildingItemInput
            {
                Position_L = TileVector.Zero,
                Direction_L = shapeConnectorConfig.Direction.Value,
                StandType = shapeConnectorConfig.StandType,
                IOType = shapeConnectorConfig.CapsType,
                Seperators = shapeConnectorConfig.Separators
            });
            return this;
        }


        public ISingleTileConnectorDataBuilder AddShapeOutput(ShapeConnectorConfig shapeConnectorConfig)
        {
            BuildingConnectors.Add(new BuildingItemOutput
            {
                Position_L = TileVector.Zero,
                Direction_L = shapeConnectorConfig.Direction.Value,
                StandType = shapeConnectorConfig.StandType,
                IOType = shapeConnectorConfig.CapsType,
                Seperators = shapeConnectorConfig.Separators
            });
            return this;
        }

        public ISingleTileConnectorDataBuilder AddWireInput(WireConnectorConfig wireConnectorConfig)
        {
            BuildingConnectors.Add(
                new BuildingSignalInput
                {
                    Position_L = TileVector.Zero,
                    Direction_L = wireConnectorConfig.Direction.Value,
                    _IOType = wireConnectorConfig.IoType,
                    TileDirection =  wireConnectorConfig.Direction,
                });
            return this;
        }


        public ISingleTileConnectorDataBuilder AddWireOutput(WireConnectorConfig wireConnectorConfig)
        {
            BuildingConnectors.Add(
                new BuildingSignalOutput
                {
                    Position_L = TileVector.Zero,
                    Direction_L = wireConnectorConfig.Direction.Value,
                    _IOType = wireConnectorConfig.IoType,
                    TileDirection =  wireConnectorConfig.Direction,
                });
            return this;
        }

        public ISingleTileConnectorDataBuilder AddFluidInput(FluidConnectorConfig fluidConnectorConfig)
        {
            BuildingConnectors.Add(
                new BuildingFluidInput
                {
                    Position_L = TileVector.Zero,
                    Direction_L = fluidConnectorConfig.Direction.Value,
                    _IOType = fluidConnectorConfig.IoType,
                    TileDirection = fluidConnectorConfig.Direction
                });
            return this;
        }

        public ISingleTileConnectorDataBuilder AddFluidOutput(FluidConnectorConfig fluidConnectorConfig)
        {
            BuildingConnectors.Add(
                new BuildingFluidOutput
                {
                    Position_L = TileVector.Zero,
                    Direction_L = fluidConnectorConfig.Direction.Value,
                    _IOType = fluidConnectorConfig.IoType,
                    TileDirection = fluidConnectorConfig.Direction
                });
            return this;
        }

        public IBuildingConnectorData Build()
        {
            TileVector[] tiles = { TileVector.Zero };

            LocalTileBounds tileBounds = new(TileVector.Zero, TileVector.Zero);

            TileDimensions tileDimensions = tileBounds.Dimensions;
            LocalVector center = LocalVector.Lerp((LocalVector)tileBounds.Min, (LocalVector)tileBounds.Max, 0.5f);

            return new BuildingConnectorData(
                BuildingConnectors,
                tiles,
                tileBounds,
                center,
                tileDimensions);
        }
    }

    public interface ISingleTileConnectorDataBuilder
    {
        ISingleTileConnectorDataBuilder AddShapeInput(ShapeConnectorConfig shapeConnectorConfig);
        ISingleTileConnectorDataBuilder AddShapeOutput(ShapeConnectorConfig shapeConnectorConfig);

        ISingleTileConnectorDataBuilder AddWireInput(WireConnectorConfig wireConnectorConfig);
        ISingleTileConnectorDataBuilder AddWireOutput(WireConnectorConfig wireConnectorConfig);

        ISingleTileConnectorDataBuilder AddFluidInput(FluidConnectorConfig fluidConnectorConfig);
        ISingleTileConnectorDataBuilder AddFluidOutput(FluidConnectorConfig fluidConnectorConfig);

        IBuildingConnectorData Build();
    }

    public interface IMultiTileConnectorDataBuilder
    {
        IMultiTileConnectorDataBuilder AddTile(TileVector tile);
    }
}
