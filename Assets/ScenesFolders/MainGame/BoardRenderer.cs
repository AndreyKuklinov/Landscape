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
        private TileObject[,] _gameBoard;

        public void DrawEmptyBoard(int width, int height)
        {
            _litTiles = new List<TileObject>();
            _gameBoard = new TileObject[width, height];
            for(var x = 0; x<width; x++)
            for (var y = 0; y < height; y++)
            {
                _gameBoard[x, y] = Instantiate(tilePrefab, transform).GetComponent<TileObject>();
                var pos = new Vector3(x * tileSize, 0, y*tileSize);
                _gameBoard[x, y].Init(gameManager.GameBoard[x,y], models[0], lightModel, pos);
            }
        }

        public void LightTile(int x, int y)
        {
            var tile = _gameBoard[x, y];
            _litTiles.Add(tile);
            tile.IsLit = true;
        }

        public void UnlightTiles()
        {
            foreach (var tile in _litTiles)
                tile.IsLit = false;
            _litTiles = new List<TileObject>();
        }
    }
}
