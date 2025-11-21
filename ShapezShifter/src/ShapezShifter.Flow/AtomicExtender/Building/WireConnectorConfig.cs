namespace ShapezShifter.Flow.Atomic
{
    public struct WireConnectorConfig
    {
        public readonly TileDirection Direction;
        public readonly BuildingSignalIOType IoType;

        private WireConnectorConfig(TileDirection direction,
            BuildingSignalIOType ioType = BuildingSignalIOType.Building)
        {
            Direction = direction;
            IoType = ioType;
        }

        public static WireConnectorConfig DefaultInput(BuildingSignalIOType ioType = BuildingSignalIOType.Building)
        {
            return new WireConnectorConfig(TileDirection.West, ioType);
        }

        public static WireConnectorConfig DefaultOutput(BuildingSignalIOType ioType = BuildingSignalIOType.Building)
        {
            return new WireConnectorConfig(TileDirection.East, ioType);
        }

        public static WireConnectorConfig CustomInput(TileDirection direction,
            BuildingSignalIOType ioType = BuildingSignalIOType.Building)
        {
            return new WireConnectorConfig(direction, ioType);
        }
        
        public static WireConnectorConfig CustomOutput(TileDirection direction,
            BuildingSignalIOType ioType = BuildingSignalIOType.Building)
        {
            return new WireConnectorConfig(direction, ioType);
        }
    }
}