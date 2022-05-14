namespace MainGame
{
    public enum TileTypes
    {
        Empty,
        Mountain,
        Forest,
        Plain,
        Lake,
        Village
    }

    public enum RoadDirection
    {
        None,
        LeftToRight,
        UpToDown,
        Crossroad
    }
    
    public struct Tile
    {
        public RoadDirection RoadDirection { get; set; }
        public TileTypes Type { get; set; }
        public int X { get; }
        public int Y { get; }

        public Tile(TileTypes type, int x, int y)
        {
            Type = type;
            RoadDirection = RoadDirection.None;
            X = x;
            Y = y;
        }

        public bool HasRoad => RoadDirection != RoadDirection.None;
    }
}
