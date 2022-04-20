using System;
using System.Security.Cryptography;
using UnityEngine;

namespace ScenesFolders.MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile Tile { get; private set; }
        private GameObject _model;
        private GameObject _light;

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
        
        public void Init(Tile tile, GameObject modelPrefab, GameObject lightModelPrefab, Vector3 screenPosition)
        {
            Tile = tile;
            transform.position = screenPosition;
            _isLit = false;
            _light = Instantiate(lightModelPrefab, transform);
            _light.SetActive(false);
            Draw(modelPrefab);
        }
        
        public void Draw(GameObject newModel)
        {
            if (newModel != _model) //TODO: Проверить, что это условие правильное
            {
                Destroy(_model);
                _model = Instantiate(newModel, transform);
            }
        }

        public void OnMouseUp()
        {
            IsLit = false;
        }
    }
}
