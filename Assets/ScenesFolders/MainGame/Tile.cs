using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScenesFolders.MainGame
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

        public Tile(TileTypes type)
        {
            Type = type;
            RoadDirection = RoadDirection.None;
        }

        public bool HasRoad => RoadDirection != RoadDirection.None;
    }
}
