using System;
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
        public List<GameObject> models;
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
                GameBoard[x, y].Init(gameManager.GameBoard[x,y], models[0], lightModel, gameManager, pos);
            }
        }

        // public void LightTile(int x, int y)
        // {
        //     var tile = GameBoard[x, y];
        //     _litTiles.Add(tile);
        //     tile.IsLit = true;
        // }
        //
        // public void UnlightTiles()
        // {
        //     foreach (var tile in _litTiles)
        //         tile.IsLit = false;
        //     _litTiles = new List<TileObject>();
        // }

        public void ChangeTile(int x, int y, TileTypes newType)
        {
            var tile = GameBoard[x, y];
            tile.Type = newType;
            tile.Draw(models[(int)newType]);
        }
    }
}
