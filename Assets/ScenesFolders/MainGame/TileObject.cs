using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScenesFolders.MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile _tile;
        private GameObject _model;
        private GameObject _light;
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
            get => _tile.Type;
            set => _tile = new Tile(value, _tile.X, _tile.Y);
        }
        
        public void Init(Tile tile, GameObject modelPrefab, GameObject lightModelPrefab, 
            GameManager gameManager, Vector3 screenPosition)
        {
            _tile = tile;
            transform.position = screenPosition;
            _gameManager = gameManager;
            _isLit = false;
            _light = Instantiate(lightModelPrefab, transform);
            _light.SetActive(false);
            Draw(modelPrefab);
        }

        public void Draw(GameObject newModel)
        {
            Destroy(_model);
            _model = Instantiate(newModel, transform);
        }

        public void OnMouseUp()
        {
            var moves = _gameManager.GetMovesAt(_tile.X, _tile.Y);
            if(moves.Length == 0)
                return;
            
            //TESTING (6 not implemented)
            var choice = moves[Random.Range(0, moves.Length-1)];
            _gameManager.MakeTurn(_tile.X, _tile.Y, choice);
        }
    }
}
