﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScenesFolders.MainGame
{
    public enum TileTypes
    {
        Mountain,
        Forest,
        Plain,
        Lake,
        Village,
        Empty
    }

    public enum TileVariations
    {
        Wet,
        Dry,
        Elevated,
        WithCrops,
        WithSmallTrees,
        Default
    }

    public struct Tile
    {
        public bool HasRoad { get; set; }
        public TileTypes Type { get; private set; }

        public Tile(TileTypes type)
        {
            HasRoad = false;
            Type = type;
        }
    }

    public class GameManager : MonoBehaviour
    {
        public int Score { get; private set; }
        public Tile[,] GameBoard { get; private set; }
        private int[] DiceRoll { get; set; }
        private bool SkippedTurn { get; set; }

        public void StartGame()
        {
            GameBoard = new Tile[5, 5];
            StartTurn();
        }

        public void EndGame()
        {
            //TODO
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

        private (int, int)[] FindNeighbours(int x, int y)
        {
            (int, int)[] result;
            if (x + y == 8)
            {
                result = new (int, int)[2];
                result[0] = (x, y - 1);
                result[1] = (x - 1, y);
                return result;
            }

            if (x + y == 0)
            {
                result = new (int, int)[2];
                result[0] = (x, y + 1);
                result[1] = (x + 1, y);
                return result;
            }

            if (x == 0 && y == 4)
            {
                result = new (int, int)[2];
                result[0] = (x, y - 1);
                result[1] = (1, y);
                return result;
            }

            if (y == 0 && x == 4)
            {
                result = new (int, int)[2];
                result[0] = (x, 1);
                result[1] = (x - 1, y);
                return result;
            }


            if (x == 4)
            {
                result = new (int, int)[3];
                result[0] = (x, y - 1);
                result[1] = (x - 1, y);
                result[2] = (x - 1, y + 1);
                return result;
            }

            if (y == 4)
            {
                result = new (int, int)[3];
                result[0] = (x - 1, y);
                result[1] = (x, y - 1);
                result[2] = (x + 1, y);
                return result;
            }

            result = new (int, int)[4];
            result[0] = (x, y + 1);
            result[1] = (x + 1, y);
            result[2] = (x - 1, y);
            result[3] = (x, y - 1);

            return result;
        }

        private TileVariations TileVariation(int x, int y)
        {
            var types = new List<TileVariations>();
            var neighbours = FindNeighbours(x, y);
            foreach (var (x1, y1) in neighbours)
            {
                if (GameBoard[x1, y1].Type == TileTypes.Village)
                    types.Add(TileVariations.WithCrops);
                if (GameBoard[x1, y1].Type == TileTypes.Lake)
                    types.Add(TileVariations.Wet);
                if (GameBoard[x1, y1].Type == TileTypes.Plain)
                    types.Add(TileVariations.Dry);
                if (GameBoard[x1, y1].Type == TileTypes.Mountain)
                    types.Add(TileVariations.Elevated);
                if (GameBoard[x1, y1].Type == TileTypes.Forest)
                    types.Add(TileVariations.WithSmallTrees);
            }

            return types.GroupBy(type => type)
                .OrderByDescending(g => g.Count())
                .First().Key;
        }

        public void MakeTurn(int x, int y, TileTypes tileType)
        {
            GameBoard[x, y] = new Tile(tileType);


            AnimationsController.StartPlacingAnimation(x, y, tileType, TileVariation(x, y));

            SkippedTurn = false;
            if (tileType == TileTypes.Village)
                CreateRoads();
            EndTurn();
        }

        private void CreateRoads()
        {
            for (var x = 0; x < GameBoard.GetLength(0); x++)
            {
                for (var y = 0; y < GameBoard.GetLength(1); y++)
                {
                    if (GameBoard[x, y].Type != TileTypes.Village) continue;
                    for (var x1 = 0; x1 < 4; x1++)
                    {
                        if (GameBoard[x1, y].Type != TileTypes.Village) continue;
                        for (var x2 = Math.Min(x1, x); x2 < Math.Max(x1, x); x2++)
                        {
                            GameBoard[x2, y].HasRoad = true;
                            AnimationsController.StartRoadCreationAnimation(x2, y, GameBoard[x2, y].Type);
                        }
                    }

                    for (var y1 = 0; y1 < 4; y1++)
                    {
                        if (GameBoard[x, y1].Type != TileTypes.Village) continue;
                        for (var y2 = Math.Min(y1, y); y2 < Math.Max(y1, y); y2++)
                        {
                            GameBoard[x, y2].HasRoad = true;
                            AnimationsController.StartRoadCreationAnimation(x, y2, GameBoard[x, y2].Type);
                        }
                    }
                }
            }
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

        private void EndTurn()
        {
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