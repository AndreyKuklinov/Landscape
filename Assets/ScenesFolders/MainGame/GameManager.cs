using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
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
        Empty = 0
    }
    
    public struct Tile
    {
        public bool HasRoad { get; private set; }
        public TileTypes Type { get; set; }
    }
    
    public class GameManager : MonoBehaviour
    {
        public Tile[,] GameBoard { get; private set; }
        public Objective[] Objectives { get; private set; }
        public int[] DiceRoll { get; private set; }
        public bool SkippedTurn { get; private set; }

        public void StartGame()
        {
            GameBoard = new Tile[5, 5];
            Objectives = PickRandomObjectives(3);
            StartTurn();
        }

        public void EndGame()
        {
            //TODO
        }

        public TileTypes[] GetPossibleMovesAt(int x, int y)
        {
            var diceValues = new List<int>(DiceRoll);
            if (GameBoard[x,y].Type != TileTypes.Empty
                || !diceValues.Remove(x) 
                || !diceValues.Remove(y) 
                || diceValues.Count != 1)
                return Array.Empty<TileTypes>();
            return GetPossibleMoves(diceValues[0]);
        }

        public void MakeTurn(int x, int y, TileTypes tileType)
        {
            GameBoard[x, y].Type = tileType;
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

        public Tile GetTileAt(int x, int y)
        {
            if (x < 0
                || y < 0
                || x >= GameBoard.GetLength(0)
                || y >= GameBoard.GetLength(1))
                return new Tile();
            return GameBoard[x, y];
        }

        public int CountAdjacentOfType(int x, int y, TileTypes type)
        {
            var count = 0;
            foreach (var tile in GetNeighbours(x, y))
            {
                if (tile.Type == type)
                    count++;
            }

            return count;
        }

        public IEnumerable<Tile> GetNeighbours(int x, int y)
        {
            for(var dx = -1; dx<1; dx++)
            for (var dy = -1; dy < 1; dy++)
            {
                if(Math.Abs(dx) == Math.Abs(dy))
                    continue;
                yield return GetTileAt(x + dx, y + dy);
            }
        }
        
        private void UpdatePoints()
        {
            foreach (var obj in Objectives)
                obj.UpdatePoints(this);
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
        
        private void StartTurn()
        {
            DiceRoll = new int[3];
            for (int i = 0; i < 3; i++)
                DiceRoll[i] = Random.Range(1, 6);
        }

        private void EndTurn()
        {
            UpdatePoints();
            StartTurn();
        }
        
        private TileTypes[] GetPossibleMoves(int diceValue)
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