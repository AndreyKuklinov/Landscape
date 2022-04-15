using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public class GameManager : MonoBehaviour
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

        public GameManager(Objective[] objectives, Tile[,] gameBoard)
        {
            Objectives = objectives;
            GameBoard = gameBoard;
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
                var chosen = possibleCriteria[Random.Range(0, possibleCriteria.Count - 1)];
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
                        Debug.LogError(
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
            if (tileType == TileTypes.Empty) Debug.LogError("Tile to place can't be empty");

            GameBoard[x, y].Type = tileType;
            SkippedTurn = false;
            if (tileType == TileTypes.Village)
                CreateRoads();
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
                            // direction 0 <=> сверху вниз
                            // direction 1 <=> слева направо
                            // direction 2 <=> перекресток
                            var direction = 1;
                            if (GameBoard[x2, y].HasRoad) direction = 2;
                            GameBoard[x2, y].HasRoad = true;
                            AnimationsController.StartRoadCreationAnimation(x2, y, GameBoard[x2, y].Type, direction);
                        }
                    }

                    for (var y1 = 0; y1 < 4; y1++)
                    {
                        if (GameBoard[x, y1].Type != TileTypes.Village) continue;
                        for (var y2 = Math.Min(y1, y); y2 < Math.Max(y1, y); y2++)
                        {
                            var direction = 0;
                            if (GameBoard[x, y2].HasRoad) direction = 2;
                            GameBoard[x, y2].HasRoad = true;
                            AnimationsController.StartRoadCreationAnimation(x, y2, GameBoard[x, y2].Type, direction);
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
                DiceRoll[i] = Random.Range(1, 6);
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
            AnimationsController.StartMovingAnimation(startX, startY, targetX, targetY);
            throw new NotImplementedException();
        }
    }
}