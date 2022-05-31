using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject lightModel;
        [SerializeField] private GameObject tileButtonPrefab;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private List<GameObject> models;
        [SerializeField] private GameObject[] modelsWithRoadsLeftToRight;
        [SerializeField] private GameObject[] modelsWithRoadsUpToDown;
        [SerializeField] private GameObject[] modelsWithCrossroads;
        [SerializeField] private Sprite[] moveSprites;
        [SerializeField] private float tileSize;
        private List<TileObject> litTiles;
        public TileObject[,] GameBoard { get; private set; }

        public void DrawEmptyBoard(int width, int height)
        {
            litTiles = new List<TileObject>();
            GameBoard = new TileObject[width, height];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                GameBoard[x, y] = Instantiate(tilePrefab, transform).GetComponent<TileObject>();
                var pos = new Vector3(x * tileSize, 0, y * tileSize);
                GameBoard[x, y].Init(gameManager.GameBoard[x, y], models[0], lightModel, tileButtonPrefab, gameManager,
                    pos);
            }

            var centerPos = GameBoard[2, 0].transform.position;
            mainCamera.transform.position = new Vector3(centerPos.x, 7.5f, centerPos.z - tileSize / 4);
        }


        public void LightTile(int x, int y)
        {
            var tile = GameBoard[x, y];
            litTiles.Add(tile);
            tile.IsLit = true;
        }

        public void DisplayPossibleMoves()
        {
            foreach (var tile in gameManager.GameBoard)
            {
                var moves = gameManager.GetMovesAt(tile.X, tile.Y);
                if (moves.Length == 5)
                    GameBoard[tile.X, tile.Y].DisplayMove(moveSprites[moveSprites.Length - 1]);
                else if (moves.Length == 1)
                    GameBoard[tile.X, tile.Y].DisplayMove(moveSprites[(int) moves[0] - 1]);
            }
        }

        public void UndisplayMoves()
        {
            foreach (var tile in GameBoard)
                tile.UndisplayMove();
        }

        public void UnlightTiles()
        {
            foreach (var tile in litTiles)
                tile.IsLit = false;
            litTiles = new List<TileObject>();
        }

        public void ChangeTile(int x, int y, TileTypes newType)
        {
            var tile = GameBoard[x, y];
            tile.Type = newType;
            tile.Draw(models[(int) newType]);
        }

        public void DisplayRoad(int x, int y)
        {
            var tile = GameBoard[x, y];
            if (!tile.Tile.HasRoad) return;
            if (tile.Tile.RoadDirection == RoadDirection.LeftToRight) tile.Draw(modelsWithRoadsLeftToRight[(int) tile.Type]);
            if (tile.Tile.RoadDirection == RoadDirection.UpToDown) tile.Draw(modelsWithRoadsUpToDown[(int) tile.Type]);
            if (tile.Tile.RoadDirection == RoadDirection.Crossroad) 
                tile.Draw(modelsWithCrossroads[(int) tile.Type]);
            else throw new Exception("Tile has road without direction!");
        }
    }
}