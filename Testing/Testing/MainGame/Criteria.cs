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
                    Deserts
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
               && gm.CountAdjacentOfType(x, y, TileTypes.Plain) >= 1)
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
    }
}