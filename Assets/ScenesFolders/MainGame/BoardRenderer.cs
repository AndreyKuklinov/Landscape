using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScenesFolders.MainGame
{
    public class BoardRenderer : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject tilePrefab;
        public List<GameObject> models;
        private TileObject[,] _gameBoard;
        public float squareSize;

        public void DrawEmptyBoard(int width, int height)
        {
            _gameBoard = new TileObject[width, height];
            for(var x = 0; x<width; x++)
            for (var y = 0; y < height; y++)
            {
                _gameBoard[x, y] = Instantiate(tilePrefab, transform).GetComponent<TileObject>();
                var pos = new Vector3(x * squareSize, 0, y*squareSize);
                _gameBoard[x, y].Init(gameManager.GameBoard[x,y], models[0], pos);
            }
        }
    }
}
