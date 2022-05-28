using UnityEngine;

namespace MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile Tile { get; private set; }
        private GameObject Model;
        private GameObject Light;
        private GameObject TileButton;
        private SpriteRenderer TileButtonSpriteRenderer;
        private GameManager GameManager;

        private bool _isLit;

        public bool IsLit
        {
            get => _isLit;
            set
            {
                Light.SetActive(value);
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
            GameManager = gameManager;
            _isLit = false;
            Light = Instantiate(lightModelPrefab, transform);
            Light.SetActive(false);
            TileButton = Instantiate(tileButtonPrefab, transform);
            TileButton.SetActive(false);
            TileButton.transform.rotation = Quaternion.Euler(90, 0, 0);
            TileButtonSpriteRenderer = TileButton.GetComponentInChildren<SpriteRenderer>();
            Draw(modelPrefab);
        }

        public void Draw(GameObject newModel)
        {
            Destroy(Model);
            Model = Instantiate(newModel, transform);
        }

        public void OnMouseUp()
        {
            if (GameManager.GameOver)
                return;
            GameManager.boardRenderer.UnlightTiles();
            GameManager.guiManager.SwitchCardsOff();
            var moves = GameManager.GetMovesAt(Tile.X, Tile.Y);
            switch (moves.Length)
            {
                case 0:
                    return;
                case 1:
                    GameManager.MakeTurn(Tile.X, Tile.Y, moves[0]);
                    break;
                default:
                    GameManager.guiManager.DisplayTileButtons(Tile);
                    GameManager.boardRenderer.LightTile(Tile.X, Tile.Y);
                    break;
            }
        }

        public void DisplayMove(Sprite sprite)
        {
            TileButtonSpriteRenderer.sprite = sprite;
            TileButton.SetActive(true);
        }

        public void UndisplayMove() =>
            TileButton.SetActive(false);
    }
}