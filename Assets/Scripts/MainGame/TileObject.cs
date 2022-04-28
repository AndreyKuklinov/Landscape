using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScenesFolders.MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile Tile { get; private set; }
        private GameObject _model;
        private GameObject _light;
        private GameObject _tileButton;
        private SpriteRenderer _tileButtonSpriteRenderer;
        private GameManager _gameManager;

        private bool _isLit;
        public bool IsLit
        {
            get => _isLit;
            set
            {
                _light.SetActive(value);
                _isLit = value;
            }
        }

        public TileTypes Type
        {
            get => Tile.Type;
            set => Tile = new Tile(value, Tile.X, Tile.Y);
        }

        public void Init(Tile tile, GameObject modelPrefab, GameObject lightModelPrefab, GameObject tileButtonPrefab,
            GameManager gameManager, Vector3 screenPosition)
        {
            Tile = tile;
            transform.position = screenPosition;
            _gameManager = gameManager;
            _isLit = false;
            _light = Instantiate(lightModelPrefab, transform);
            _light.SetActive(false);
            _tileButton = Instantiate(tileButtonPrefab, transform);
            _tileButton.SetActive(false);
            _tileButton.transform.rotation = Quaternion.Euler(90, 0, 0);
            _tileButtonSpriteRenderer = _tileButton.GetComponentInChildren<SpriteRenderer>();
            Draw(modelPrefab);
        }

        public void Draw(GameObject newModel)
        {
            Destroy(_model);
            _model = Instantiate(newModel, transform);
        }

        public void OnMouseUp()
        {
            if (_gameManager.guiManager.IsChoosingATile || _gameManager.GameOver)
                return;
            var moves = _gameManager.GetMovesAt(Tile.X, Tile.Y);
            if(moves.Length == 0 )
                return;
            if (moves.Length == 1)
                _gameManager.MakeTurn(Tile.X, Tile.Y, moves[0]);
            else
            {
                _gameManager.guiManager.DisplayTileButtons(Tile);
                _gameManager.boardRenderer.LightTile(Tile.X, Tile.Y);
            }
        }

        public void DisplayMove(Sprite sprite)
        {
            _tileButtonSpriteRenderer.sprite = sprite;
            _tileButton.SetActive(true);
        }

        public void UndisplayMove()
        {
            _tileButton.SetActive(false);
        }
    }
}
