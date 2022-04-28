﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScenesFolders.MainGame
{
    public class BoardRenderer : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject tilePrefab;
        public GameObject lightModel;
        public GameObject tileButtonPrefab;
        public GameObject mainCamera;
        public List<GameObject> models; 
        public List<Sprite> moveSprites;
        public float tileSize;
        private List<TileObject> _litTiles;
        public TileObject[,] GameBoard { get; private set; }

        public void DrawEmptyBoard(int width, int height)
        {
            _litTiles = new List<TileObject>();
            GameBoard = new TileObject[width, height];
            for(var x = 0; x<width; x++)
            for (var y = 0; y < height; y++)
            {
                GameBoard[x, y] = Instantiate(tilePrefab, transform).GetComponent<TileObject>();
                var pos = new Vector3(x * tileSize, 0, y*tileSize);
                GameBoard[x, y].Init(gameManager.GameBoard[x,y], models[0], lightModel, tileButtonPrefab, gameManager, pos);
            }

            var centerPos = GameBoard[2, 0].transform.position;
            mainCamera.transform.position = new Vector3(centerPos.x, 7.5f, centerPos.z-tileSize/4);
        }

        public void LightTile(int x, int y)
        {
            var tile = GameBoard[x, y];
            _litTiles.Add(tile);
            tile.IsLit = true;
        }

        public void DisplayPossibleMoves()
        {
            foreach (var tile in gameManager.GameBoard)
            {
                var moves = gameManager.GetMovesAt(tile.X, tile.Y);
                if(moves.Length == 5)
                    GameBoard[tile.X, tile.Y].DisplayMove(moveSprites[5]);
                else if(moves.Length == 1)
                    GameBoard[tile.X, tile.Y].DisplayMove(moveSprites[(int)moves[0]-1]);
            }
                
        }

        public void UndisplayMoves()
        {
            foreach(var tile in GameBoard)
                tile.UndisplayMove();
        }
        
        public void UnlightTiles()
        {
            foreach (var tile in _litTiles)
                tile.IsLit = false;
            _litTiles = new List<TileObject>();
        }

        public void ChangeTile(int x, int y, TileTypes newType)
        {
            var tile = GameBoard[x, y];
            tile.Type = newType;
            tile.Draw(models[(int)newType]);
        }
    }
}