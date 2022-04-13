using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private static int Twins(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Mountain
                && gm.CountAdjacentOfType(x, y, TileTypes.Mountain) == 1)
                return 1;
            return 0;
        }

        private static int Groves(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Forest
               && gm.CountAdjacentOfType(x, y, TileTypes.Forest) == 0)
                return 1;
            return 0;
        }
        
        private static int Fields(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Plain
               && gm.CountAdjacentOfType(x, y, TileTypes.Plain) >= 1)
                return 1;
            return 0;
        }
        
        private static int Swamps(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Forest
               && gm.CountAdjacentOfType(x, y, TileTypes.Plain) >= 1)
                return 1;
            return 0;
        }
        
        private static int Deserts(int x, int y, GameManager gm)
        {
            if(gm.GetTileAt(x,y).Type == TileTypes.Plain
               && gm.CountAdjacentOfType(x, y, TileTypes.Forest) == 0
               && gm.CountAdjacentOfType(x, y, TileTypes.Lake) == 0)
                return 1;
            return 0;
        }
    }
}