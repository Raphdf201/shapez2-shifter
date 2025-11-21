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

        public static ISingleTileSignalConnectorDataBuilder SignalSingleTile()
        {
            return new SingleTileBuildingSignalConnectorDataBuilder();
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
    
    public class SingleTileBuildingSignalConnectorDataBuilder : ISingleTileSignalConnectorDataBuilder
    {
        private readonly List<BuildingBaseIO> BuildingConnectors = new();

        public ISingleTileSignalConnectorDataBuilder AddWireInput(WireConnectorConfig signalInput)
        {
            BuildingConnectors.Add(
                new BuildingSignalInput
                {
                    Position_L = TileVector.Zero,
                    _IOType = signalInput.IoType,
                    TileDirection =  signalInput.Direction,
                });
            return this;
        }


        public ISingleTileSignalConnectorDataBuilder AddWireOutput(WireConnectorConfig signalOutput)
        {
            BuildingConnectors.Add(
                new BuildingSignalInput
                {
                    Position_L = TileVector.Zero,
                    _IOType = signalOutput.IoType,
                    TileDirection =  signalOutput.Direction,
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
        IBuildingConnectorData Build();
    }
    
    public interface ISingleTileSignalConnectorDataBuilder
    {
        ISingleTileSignalConnectorDataBuilder AddWireInput(WireConnectorConfig signalInput);


        ISingleTileSignalConnectorDataBuilder AddWireOutput(WireConnectorConfig signalOutput);
        IBuildingConnectorData Build();
    }

    public interface IMultiTileConnectorDataBuilder
    {
        IMultiTileConnectorDataBuilder AddTile(TileVector tile);
    }
}