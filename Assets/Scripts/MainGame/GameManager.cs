using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int boardWidth;
        [SerializeField] private int creativeBoardWidth;
        [SerializeField] private List<Objective> possibleObjectives;
        [SerializeField] private Objective testingObjective;
        [SerializeField] private bool cheatMode;

        public BoardRenderer boardRenderer;
        public GUIManager guiManager;
        public Tile[,] GameBoard { get; private set; }
        public Objective[] Objectives { get; private set; }
        public bool GameOver { get; private set; }

        public int Score
        {
            get
            {
                var res = Objectives.Aggregate(1, (current, obj) => current * (obj.Points + 1));
                return res - 1;
            }
        }

        private int[] DiceRoll;
        private int TurnCount;

        public void Start()
        {
            if (Convert.ToBoolean(PlayerPrefs.GetInt("creativeMode")))
            {
                boardWidth = creativeBoardWidth;
                cheatMode = true;
            }
            else cheatMode = false;
            
            GameBoard = new Tile[boardWidth, boardWidth];
            for (var x = 0; x < boardWidth; x++)
            for (var y = 0; y < boardWidth; y++)
                GameBoard[x, y] = new Tile(TileTypes.Empty, x, y);
            boardRenderer.DrawEmptyBoard(boardWidth, boardWidth);
            Objectives = PickRandomObjectives(3);
            GameOver = false;
            StartTurn();
        }

        public void EndGame()
        {
            if (PlayerPrefs.GetInt("MaxScore") < Score && PlayerPrefs.GetInt("boardWidth") <= 5)
                PlayerPrefs.SetInt("MaxScore", Score);
            GameOver = true;
            guiManager.GameOver();
        }

        private Objective[] PickRandomObjectives(int num)
        {
            var res = new List<Objective>();
            if (testingObjective.name != string.Empty)
            {
                res.Add(testingObjective);
                num--;
            }

            var objs = new List<Objective>(possibleObjectives);
            for (var i = 0; i < num; i++)
            {
                var chosen = objs[Random.Range(0, objs.Count - 1)];
                res.Add(chosen);
                objs.Remove(chosen);
            }

            return res.ToArray();
        }

        public TileTypes[] GetMovesAt(int x, int y)
        {
            var diceValues = new List<int>(DiceRoll);
            if (GameBoard[x, y].Type != TileTypes.Empty
                || !(diceValues.Remove(x + 1) || diceValues.Remove(6))
                || !(diceValues.Remove(y + 1) || diceValues.Remove(6)))
                return Array.Empty<TileTypes>();
            return GetTileFromDice(diceValues[0]);
        }

        public IEnumerable<Tile> GetAllMoves()
        {
            for (var x = 0; x < boardWidth; x++)
            for (var y = 0; y < boardWidth; y++)
            {
                var moves = GetMovesAt(x, y);
                if (moves.Length > 0)
                    yield return GameBoard[x, y];
            }
        }

        public void MakeTurn(int x, int y, TileTypes tileType)
        {
            if (tileType == TileTypes.Empty) Debug.LogError("Tile to place can't be empty");

            GameBoard[x, y].Type = tileType;
            boardRenderer.ChangeTile(x, y, tileType);
            if (tileType == TileTypes.Village)
                CreateRoads();
            EndTurn();
        }

        public int CountAdjacentOfType(int x, int y, TileTypes type) =>
            GetNeighbours(x, y).Count(tile => tile.Type == type);

        public IEnumerable<Tile> GetNeighbours(int x, int y)
        {
            for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
            {
                if (Math.Abs(dx) == Math.Abs(dy) || !TileExists(x + dx, y + dy))
                    continue;
                yield return GetTileAt(x + dx, y + dy);
            }
        }

        public Tile GetTileAt(int x, int y) =>
            TileExists(x, y) ? GameBoard[x, y] : new Tile();

        private bool TileExists(int x, int y)
        {
            return x >= 0
                   && y >= 0
                   && x < GameBoard.GetLength(0)
                   && y < GameBoard.GetLength(1);
        }

        public void SkipTurn()
        {
            if (GameOver)
                return;
            if (TurnCount == 25)
                EndGame();
            EndTurn();
        }

        private void StartTurn()
        {
            if (GameOver) return;
            RollDice();
            var moves = GetAllMoves().ToArray();
            boardRenderer.DisplayPossibleMoves();
            if (moves.Length == 0)
                SkipTurn();
            else
                TurnCount++;
        }

        public void EndTurn()
        {
            UpdatePoints();
            boardRenderer.UnlightTiles();
            boardRenderer.UndisplayMoves();
            guiManager.SwitchCardsOff();
            guiManager.UpdateScore();
            StartTurn();
        }

        public TileTypes[] GetTileFromDice(int diceValue)
        {
            if (diceValue % 6 == 0 && diceValue != 0)
                return new[]
                    {TileTypes.Mountain, TileTypes.Forest, TileTypes.Plain, TileTypes.Lake, TileTypes.Village};
            return new[] {(TileTypes) (diceValue % 6)};
        }

        private void RollDice()
        {
            DiceRoll = new int[3];
            if (cheatMode)
                for (var i = 0; i < DiceRoll.Length; i++)
                    DiceRoll[i] = 6;

            else if (TurnCount == 0)
                for (var i = 0; i < 3; i++)
                    DiceRoll[i] = Random.Range(1, boardWidth + 1);
            else
                for (var i = 0; i < 3; i++)
                    DiceRoll[i] = Random.Range(1, boardWidth + 2);
        }

        private void UpdatePoints()
        {
            foreach (var obj in Objectives)
                obj.UpdatePoints(this);
            // TODO
        }

        private void MoveTile(int startX, int startY, int targetX, int targetY, out bool isSuccessful)
        {
            if (GameBoard[targetX, targetY].Type == TileTypes.Empty ||
                GameBoard[startX, startY].Type == TileTypes.Empty)
            {
                isSuccessful = false;
                return;
            }

            boardRenderer.ChangeTile(targetX, targetY, GameBoard[startX, startY].Type);
            boardRenderer.ChangeTile(startX, startY, TileTypes.Empty);
            CreateRoads();
            isSuccessful = true;
        }

        private void CreateRoads()
        {
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    if (GameBoard[x, y].Type != TileTypes.Village) continue;
                    for (var x1 = 0; x1 < 5; x1++)
                    {
                        if (GameBoard[x1, y].Type != TileTypes.Village) continue;
                        for (var x2 = Math.Min(x1, x); x2 < Math.Max(x1, x); x2++)
                        {
                            var direction = RoadDirection.LeftToRight;
                            if (GameBoard[x2, y].HasRoad) direction = RoadDirection.Crossroad;
                            GameBoard[x2, y].RoadDirection = direction;
                            boardRenderer.DisplayRoad(x2, y);
                        }
                    }

                    for (var y1 = 0; y1 < 5; y1++)
                    {
                        if (GameBoard[x, y1].Type != TileTypes.Village) continue;
                        for (var y2 = Math.Min(y1, y); y2 < Math.Max(y1, y); y2++)
                        {
                            var direction = RoadDirection.UpToDown;
                            if (GameBoard[x, y2].HasRoad) direction = RoadDirection.Crossroad;
                            GameBoard[x, y2].RoadDirection = direction;
                            boardRenderer.DisplayRoad(x, y2);
                        }
                    }
                }
            }
        }
    }
}