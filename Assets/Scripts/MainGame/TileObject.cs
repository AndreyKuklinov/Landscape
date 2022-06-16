using UnityEngine;

namespace MainGame
{
    public class TileObject : MonoBehaviour
    {
        public Tile Tile { get; private set; }
        private GameObject model;
        private GameObject Light;
        private GameObject pointMarker;
        private GameObject tileButton;
        private SpriteRenderer tileButtonSpriteRenderer;
        private GameManager gameManager;
        
        private bool isLit;

        public bool IsLit
        {
            get => isLit;
            set
            {
                Light.SetActive(value);
                isLit = value;
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
            this.gameManager = gameManager;
            isLit = false;
            Light = Instantiate(lightModelPrefab, transform);
            Light.SetActive(false);
            tileButton = Instantiate(tileButtonPrefab, transform);
            tileButton.SetActive(false);
            tileButton.transform.rotation = Quaternion.Euler(90, 0, 0);
            tileButtonSpriteRenderer = tileButton.GetComponentInChildren<SpriteRenderer>();
            pointMarker = transform.Find("PointMarker").gameObject;
            Draw(modelPrefab);
        }

        public void Draw(GameObject newModel)
        {
            Destroy(model);
            model = Instantiate(newModel, transform);
        }

        public void OnMouseUp()
        {
            if (gameManager.GameOver)
                return;
            gameManager.boardRenderer.UnlightTiles();
            var moves = gameManager.GetMovesAt(Tile.X, Tile.Y);
            switch (moves.Length)
            {
                case 0:
                    return;
                case 1:
                    gameManager.MakeTurn(Tile.X, Tile.Y, moves[0]);
                    break;
                default:
                    gameManager.guiManager.DisplayTileButtons(Tile);
                    gameManager.boardRenderer.LightTile(Tile.X, Tile.Y);
                    break;
            }
        }

        public void DisplayMove(Sprite sprite)
        {
            tileButtonSpriteRenderer.sprite = sprite;
            tileButton.SetActive(true);
        }

        public void UndisplayMove() =>
            tileButton.SetActive(false);

        public void SetPointMarkerVisible(bool value)
            => pointMarker.SetActive(value);
    }
}