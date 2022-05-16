using UnityEngine;

namespace MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile Tile { get; private set; }
        private GameObject _model;
        private GameObject _selection;
        private Material _glowMaterial;
        private GameObject _tileButton;
        private SpriteRenderer _tileButtonSpriteRenderer;
        private GameManager _gameManager;
        private MaterialHandler _materialHandler;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _selection.SetActive(value);
                _isSelected = value;
            }
        }

        public TileTypes Type
        {
            get => Tile.Type;
            set => Tile = new Tile(value, Tile.X, Tile.Y);
        }

        public void Init(Tile tile, GameObject modelPrefab, GameObject selectionModelPrefab, Material glowMaterial, GameObject tileButtonPrefab,
            GameManager gameManager, Vector3 screenPosition)
        {
            Tile = tile;
            transform.position = screenPosition;
            _gameManager = gameManager;
            _isSelected = false;
            _selection = Instantiate(selectionModelPrefab, transform);
            _selection.SetActive(false);
            _glowMaterial = glowMaterial;
            _tileButton = Instantiate(tileButtonPrefab, transform);
            _tileButton.SetActive(false);
            _tileButton.transform.rotation = Quaternion.Euler(90, 0, 0);
            _tileButtonSpriteRenderer = _tileButton.GetComponentInChildren<SpriteRenderer>();
            _materialHandler = GetComponentInChildren<MaterialHandler>();
            Draw(modelPrefab);
        }

        public void Draw(GameObject newModel)
        {
            Destroy(_model);
            _model = Instantiate(newModel, transform);
        }

        public void OnMouseUp()
        {
            if (_gameManager.GameOver)
                return;
            _gameManager.boardRenderer.UnSelectTiles();
            _gameManager.guiManager.SwitchCardsOff();
            var moves = _gameManager.GetMovesAt(Tile.X, Tile.Y);
            if(moves.Length == 0 )
                return;
            if (moves.Length == 1)
                _gameManager.MakeTurn(Tile.X, Tile.Y, moves[0]);
            else
            {
                _gameManager.guiManager.DisplayTileButtons(Tile);
                _gameManager.boardRenderer.SelectTile(Tile.X, Tile.Y);
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

        public void SetGlow(bool active)
        {
            if(active)
                _materialHandler.SetMaterial(_glowMaterial);
            else
                _materialHandler.ResetMaterial();
        }
    }
}
