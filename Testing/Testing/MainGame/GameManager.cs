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

    public enum TileVariations
    {
        Wet,
        Elevated,
        WithCrops,
        Default
    }

    public struct Tile
    {
        public bool HasRoad { get; set; }
        public TileTypes Type { get; set; }
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

        private TileVariations TileVariation(int x, int y)
        {
            var types = new List<TileVariations>();
            foreach (var tile in GetNeighbours(x, y))
            {
                switch (tile.Type)
                {
                    case TileTypes.Village:
                        types.Add(TileVariations.WithCrops);
                        break;
                    case TileTypes.Lake:
                        types.Add(TileVariations.Wet);
                        break;
                    case TileTypes.Plain:
                        types.Add(TileVariations.Default);
                        break;
                    case TileTypes.Mountain:
                        types.Add(TileVariations.Elevated);
                        break;
                    case TileTypes.Forest:
                        types.Add(TileVariations.Default);
                        break;
                    case TileTypes.Empty:
                        break;
                    default:
                        Debug.Print(
                            "Tile seems to be newly added to Enum, need to add case for this enum to TileVariation");
                        break;
                }
            }

            return types.GroupBy(type => type)
                .OrderByDescending(g => g.Count())
                .First().Key;
        }

        public void MakeTurn(int x, int y, TileTypes tileType)
        {
            if (tileType == TileTypes.Empty) Debug.Print("Tile to place can't be empty");

            GameBoard[x, y].Type = tileType;
            SkippedTurn = false;
            EndTurn();
        }

        public int CountAdjacentOfType(int x, int y, TileTypes type) =>
            GetNeighbours(x, y).Count(tile => tile.Type == type);

        public IEnumerable<Tile> GetNeighbours(int x, int y)
        {
            for (var dx = -1; dx < 1; dx++)
            for (var dy = -1; dy < 1; dy++)
            {
                if (Math.Abs(dx) == Math.Abs(dy))
                    continue;
                yield return GetTileAt(x + dx, y + dy);
            }
        }

        public Tile GetTileAt(int x, int y)
        {
            if (x < 0
                || y < 0
                || x >= GameBoard.GetLength(0)
                || y >= GameBoard.GetLength(1))
                return new Tile();
            return GameBoard[x, y];
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
        }
    }
}