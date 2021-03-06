using System.Collections.Generic;
using System.Linq;

namespace MainGame
{
    public delegate int ScoringCriterion(int x, int y, GameManager gm);
    
    public static class Criteria
    {
        public static readonly Dictionary<string, ScoringCriterion> dict = new Dictionary<string, ScoringCriterion>()
        {
            {"Twins", Twins},
            {"Groves", Groves},
            {"Fields", Fields},
            {"Swamps", Swamps},
            {"Deserts", Deserts},
            {"Meadows", Meadows},
            {"Cliffs", Cliffs},
            {"Foothills", Foothills},
            {"Ponds", Ponds},
            {"Hollows", Hollows},
            {"Expanses", Expanses},
            {"Islands", Islands},
            {"Bridges", Bridges},
            {"Parks", Parks},
            {"Farmlands", Farmlands},
            {"Mines", Mines},
            {"Towns", Towns},
            {"Castles", Castles},
            {"Stations", Stations},
            {"Crossroads", Crossroads},
        };

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
                    .Where(type => type != TileTypes.Mountain && type != TileTypes.Empty)
                    .Distinct()
                    .Count() >= 3)
                return 1;
            return 0;
        }
        
        public static int Foothills(int x, int y, GameManager gm)
        {
            var neighbourTypes = gm.GetNeighbours(x, y)
                .Select(tile => tile.Type)
                .Where(type => type != TileTypes.Mountain && type != TileTypes.Empty)
                .ToArray();
            if(gm.GetTileAt(x,y).Type == TileTypes.Mountain
               && neighbourTypes.Length != neighbourTypes.Distinct().Count())
                return 1;
            return 0;
        }
        
        public static int Ponds(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type != TileTypes.Lake)
                return 0;
            var neighbours = gm.GetNeighbours(x, y);
            TileTypes type = TileTypes.Empty;
            foreach (var tile in neighbours)
            {
                if (tile.Type == TileTypes.Empty)
                    return 0;
                if (type == TileTypes.Empty)
                    type = tile.Type;
                else if (type != tile.Type)
                    return 0;
            }
            return 1;
        }
        
        public static int Hollows(int x, int y, GameManager gm)
        {
            if (gm.GetTileAt(x, y).Type != TileTypes.Mountain
                && gm.GetTileAt(x,y).Type != TileTypes.Empty
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
                && gm.GetTileAt(x,y).Type != TileTypes.Lake
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
            if (gm.GetTileAt(x, y).RoadDirection == RoadDirection.Crossroad)
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