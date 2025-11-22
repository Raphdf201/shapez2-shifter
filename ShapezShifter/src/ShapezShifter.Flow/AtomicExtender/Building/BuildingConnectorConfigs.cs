namespace ShapezShifter.Flow.Atomic
{
    public struct ShapeConnectorConfig
    {
        public readonly TileDirection Direction;
        public readonly BuildingBeltStandType StandType;
        public readonly BuildingItemIOType CapsType;
        public readonly bool Separators;

        private ShapeConnectorConfig(TileDirection direction,
            BuildingItemIOType capsType = BuildingItemIOType.ElevatedBorder,
            BuildingBeltStandType standType = BuildingBeltStandType.Normal, bool separators = false)
        {
            Direction = direction;
            StandType = standType;
            CapsType = capsType;
            Separators = separators;
        }

        public static ShapeConnectorConfig DefaultInput(
            BuildingItemIOType capsType = BuildingItemIOType.ElevatedBorder,
            BuildingBeltStandType standType = BuildingBeltStandType.Normal, bool separators = false)
        {
            return new ShapeConnectorConfig(TileDirection.West, capsType, standType, separators);
        }

        public static ShapeConnectorConfig DefaultOutput(
            BuildingItemIOType capsType = BuildingItemIOType.ElevatedBorder,
            BuildingBeltStandType standType = BuildingBeltStandType.Normal, bool separators = false)
        {
            return new ShapeConnectorConfig(TileDirection.East, capsType, standType, separators);
        }
    }
    
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