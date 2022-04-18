using System;
using System.Security.Cryptography;
using UnityEngine;

namespace ScenesFolders.MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile Tile { get; private set; }
        private GameObject _model;

        public void Init(Tile tile, GameObject modelPrefab, Vector3 screenPosition)
        {
            Tile = tile;
            transform.position = screenPosition;
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
            Debug.Log("(" + Tile.X + " " + Tile.Y + ") was clicked!");
        }
    }
}
