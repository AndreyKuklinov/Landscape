using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

    public class GameManager
    {
        public int Score
        {
            get
            {
                var res = Objectives.Aggregate(1, (current, obj) => current * (obj.Points + 1));
                return res - 1;
            }
        }

        public Tile[,] GameBoard { get; private set; }
        public Objective[] Objectives { get; private set; }
        private int[] DiceRoll { get; set; }
        private bool SkippedTurn { get; set; }

        public GameManager()
        {
            GameBoard = new Tile[5, 5];
            Objectives = PickRandomObjectives(3);
            StartTurn();
        }

        public GameManager(Objective[] objectives)
        {
            GameBoard = new Tile[5, 5];
            Objectives = objectives;
            StartTurn();
        }


        public void EndGame()
        {
            //TODO
        }

        private Objective[] PickRandomObjectives(int num)
        {
            var possibleCriteria = new List<ScoringCriterion>(Criteria.GetAll);
            var res = new List<Objective>();
            for (var i = 0; i < num; i++)
            {
                var chosen = possibleCriteria[new Random().Next(0, possibleCriteria.Count)];
                res.Add(new Objective(chosen));
                possibleCriteria.Remove(chosen);
            }

            return res.ToArray();
        }

        public TileTypes[] GetPossibleTiles(int x, int y)
        {
            var diceValues = new List<int>(DiceRoll);
            if (GameBoard[x, y].Type != TileTypes.Empty
                || !diceValues.Remove(x)
                || !diceValues.Remove(y)
                || diceValues.Count != 1)
                return Array.Empty<TileTypes>();
            return GetPossibleTiles(diceValues[0]);
        }

        public void MakeTurn(int x, int y, TileTypes tileType)
        {
            if (tileType == TileTypes.Empty) Debug.Print("Tile to place can't be empty");

            GameBoard[x, y].Type = tileType;
            SkippedTurn = false;
            EndTurn();
        }

        private void CreateRoads()
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (GameBoard[x, y].Type != TileTypes.Village) continue;
                    for (var x1 = 0; x1 < 4; x1++)
                    {
                        if (GameBoard[x1, y].Type != TileTypes.Village) continue;
                        for (var x2 = Math.Min(x1, x); x2 < Math.Max(x1, x); x2++)
                        {
                            var direction = RoadDirection.LeftToRight;
                            if (GameBoard[x2, y].HasRoad) direction = RoadDirection.Crossroad;
                            GameBoard[x2, y].RoadDirection = direction;
                        }
                    }

                    for (var y1 = 0; y1 < 4; y1++)
                    {
                        if (GameBoard[x, y1].Type != TileTypes.Village) continue;
                        for (var y2 = Math.Min(y1, y); y2 < Math.Max(y1, y); y2++)
                        {
                            var direction = RoadDirection.UpToDown;
                            if (GameBoard[x, y2].HasRoad) direction = RoadDirection.Crossroad;
                            GameBoard[x, y2].RoadDirection = direction;
                        }
                    }
                }
            }
        }
        
        public int CountAdjacentOfType(int x, int y, TileTypes type) =>
            GetNeighbours(x, y).Count(tile => tile.Type == type);

        public IEnumerable<Tile> GetNeighbours(int x, int y)
        {
            for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
            {
                if (Math.Abs(dx) == Math.Abs(dy) || !TileExists(x + dx,y + dy))
                    continue;
                yield return GetTileAt(x + dx, y + dy);
            }
        }

        public Tile GetTileAt(int x, int y)
        {
            return TileExists(x,y) ? GameBoard[x, y] : new Tile();
        }

        private bool TileExists(int x, int y)
        {
            return x >= 0
                   && y >= 0
                   && x < GameBoard.GetLength(0)
                   && y < GameBoard.GetLength(1);
        }
        
        public void SkipTurn()
        {
            if (SkippedTurn)
                EndGame();
            SkippedTurn = true;
            EndTurn();
        }

        private void StartTurn()
        {
            DiceRoll = new int[3];
            for (var i = 0; i < 3; i++)
                DiceRoll[i] = new Random().Next(1, 7);
        }

        private void UpdatePoints()
        {
            foreach (var obj in Objectives)
                obj.UpdatePoints(this);
        }

        private void EndTurn()
        {
            UpdatePoints();
            CreateRoads();
            StartTurn();
        }

        private TileTypes[] GetPossibleTiles(int diceValue)
        {
            if (diceValue == 6)
                return new[]
                    {TileTypes.Mountain, TileTypes.Forest, TileTypes.Plain, TileTypes.Lake, TileTypes.Village};
            return new[] {(TileTypes) (diceValue)};
        }

        private void MoveTile(int startX, int startY, int targetX, int targetY)
        {
            throw new NotImplementedException();
            var a = GetTileAt(1, 1);
        }
    }   
}