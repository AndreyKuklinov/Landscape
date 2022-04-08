using System;
using System.Collections.Generic;
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
    
    public struct Tile
    {
        public bool HasRoad { get; private set; }
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
        public int[] DiceRoll { get; private set; }
        public bool SkippedTurn { get; private set; }
        
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
            if (GameBoard[x,y].Type != TileTypes.Empty
                || !diceValues.Remove(x) 
                || !diceValues.Remove(y) 
                || diceValues.Count != 1)
                return Array.Empty<TileTypes>();
            return GetPossibleTiles(diceValues[0]);
        }

        public void MakeTurn(int x, int y, TileTypes tileType)
        {
            GameBoard[x, y] = new Tile(tileType);
            SkippedTurn = false;
            EndTurn();
        }

        public void SkipTurn()
        {
            if(SkippedTurn)
                EndGame();
            SkippedTurn = true;
            EndTurn();
        }
        
        private void StartTurn()
        {
            DiceRoll = new int[3];
            for (int i = 0; i < 3; i++)
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
            throw new NotImplementedException();
        }
    }
}