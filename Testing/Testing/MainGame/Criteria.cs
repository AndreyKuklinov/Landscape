using System;
using System.Linq;

namespace ScenesFolders.MainGame
{
    public delegate int ScoringCriterion(int x, int y, GameManager gm);
    
    public static class Criteria
    {
        public static ScoringCriterion[] GetAll
        {
            get
            {
                return new ScoringCriterion[]
                {
                    Twins,
                    Groves,
                    Fields,
                    Swamps,
                    Deserts,
                    Meadows,
                    Cliffs,
                    Foothills,
                    Ponds,
                    Hollows,
                    Expanses,
                    Islands,
                    Bridges,
                    Parks,
                    Farmlands,
                    Mines,
                    Crossroads,
                    Towns,
                    Castles,
                    Stations
                };
            }
        }

        public static int Twins(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Mountain
               && gm.CountAdjacentOfType(x, y, TileTypes.Mountain) == 1)
                return 1;
            return 0;
        }

        public static int Groves(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Forest
               && gm.CountAdjacentOfType(x, y, TileTypes.Forest) == 0)
                return 1;
            return 0;
        }
        
        public static int Fields(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Plain
               && gm.CountAdjacentOfType(x, y, TileTypes.Plain) >= 1)
                return 1;
            return 0;
        }
        
        public static int Swamps(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Forest
               && gm.CountAdjacentOfType(x, y, TileTypes.Lake) >= 1)
                return 1;
            return 0;
        }
        
        public static int Deserts(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Plain
               && gm.CountAdjacentOfType(x, y, TileTypes.Forest) == 0
               && gm.CountAdjacentOfType(x, y, TileTypes.Lake) == 0)
                return 1;
            return 0;
        }
        
        public static int Meadows(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Forest
               && gm.CountAdjacentOfType(x, y, TileTypes.Plain) == 1)
                return 1;
            return 0;
        }
        
        public static int Cliffs(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Mountain
               && gm.GetNeighbours(x, y)
                   .Select(tile => tile.Type)
                   .Where(type => type != (TileTypes.Mountain | TileTypes.Empty))
                   .Distinct()
                   .Count() >= 3)
                return 1;
            return 0;
        }
        
        public static int Foothills(int x, int y, GameManager gm)
        {
            var neighbourTypes = gm.GetNeighbours(x, y)
                .Select(tile => tile.Type)
                .Where(type => type != (TileTypes.Mountain | TileTypes.Empty))
                .ToArray();
            if(gm.GetTileAt(x,y).Type == TileTypes.Mountain
               && neighbourTypes.Length != neighbourTypes.Distinct().Count())
                return 1;
            return 0;
        }
        
        public static int Ponds(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Lake
                && gm.GetNeighbours(x, y).Distinct().Count() == 1
                && gm.CountAdjacentOfType(x, y, TileTypes.Empty) == 0)
                return 1;
            return 0;
        }
        
        public static int Hollows(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type != TileTypes.Mountain
                && (gm.GetTileAt(x + 1, y).Type == TileTypes.Mountain &&
                    gm.GetTileAt(x - 1, y).Type == TileTypes.Mountain
                    || gm.GetTileAt(x, y + 1).Type == TileTypes.Mountain &&
                    gm.GetTileAt(x, y - 1).Type == TileTypes.Mountain))
                return 1;
            return 0;
        }
        
        public static int Expanses(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type != TileTypes.Plain && gm.GetTileAt(x, y).Type != TileTypes.Empty
                && gm.CountAdjacentOfType(x, y, TileTypes.Plain) >= 2)
                return 1;
            return 0;
        }
        
        public static int Islands(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type != TileTypes.Empty
                && gm.CountAdjacentOfType(x, y, TileTypes.Lake) == gm.GetNeighbours(x,y).Count())
                return 1;
            return 0;
        }
        
        public static int Bridges(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Lake
                && gm.GetTileAt(x,y).HasRoad)
                return 1;
            return 0;
        }
        
        public static int Parks(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Forest
                && gm.CountAdjacentOfType(x, y, TileTypes.Village) == 0
                && gm.GetNeighbours(x, y).All(tile => tile.HasRoad != true))
                return 1;
            return 0;
        }
        
        public static int Farmlands(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Plain
                && gm.CountAdjacentOfType(x, y, TileTypes.Village) >= 1)
                return 1;
            return 0;
        }
        
        public static int Mines(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Mountain
                && gm.GetNeighbours(x, y).All(tile => tile.HasRoad || tile.Type == TileTypes.Village))
                return 1;
            return 0;
        }
        
        public static int Crossroads(int x, int y, GameManager gm)
        {
            var bool1 = false;
            var bool2 = false;
            if (gm.GetTileAt(x, y).HasRoad)
                  
                for (var x1 = 0; x1 < 5; x1++)
                    if (gm.GetTileAt(x1, y).Type == TileTypes.Village)
                        if (x1 != x && gm.GetTileAt(x, y).Type == TileTypes.Village)
                            bool1 = true;
                for (var y1 = 0; y1 < 5; y1++)
                    if (gm.GetTileAt(x, y1).Type == TileTypes.Village)
                        if (y1 != x && gm.GetTileAt(x,y).Type == TileTypes.Village)
                            bool2 = true;
                if (bool1 == true && bool2 == true && gm.GetTileAt(x, y).RoadDirection == RoadDirection.Crossroad)
                return 1;
            return 0;
        }
        
        public static int Towns(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Village
                && gm.CountAdjacentOfType(x, y, TileTypes.Village) >= 1)
                return 1;
            return 0;
        }
        
        public static int Castles(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Village
                && gm.CountAdjacentOfType(x, y, TileTypes.Village) == 0)
                return 1;
            return 0;
        }
        
        public static int Stations(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type == TileTypes.Village
                && gm.GetTileAt(x, y).HasRoad)
                return 1;
            return 0;
        }
    }
}