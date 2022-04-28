using System;
using UnityEngine;
using UnityEngine.UI;

namespace ScenesFolders.MainGame
{
    public class GUIManager : MonoBehaviour
    {
        public Text dice1;
        public Text dice2;
        public Text dice3;
        public GameObject skipButton;
        public GameManager gameManager;
        public new Camera camera;
        public float scrollSpeed;
        public float rotaionSpeed;
        public bool IsChoosingATile { get; private set; }
        private Tile _clickedTile;
        private Text _skipButtonText;
        private bool isGameDone = true;
        private bool isMovementAlowed;
        private Vector3 LastMousePosition;

        public void Start()
        {
            _skipButtonText = skipButton.GetComponentInChildren<Text>();
            IsChoosingATile = false;
        }

        private void ScrollHandler()
        {
            var horizontalInput = Input.mouseScrollDelta.x;
            var verticalInput = Input.mouseScrollDelta.y;
            camera.transform.Translate(Vector3.forward * verticalInput * scrollSpeed * Time.deltaTime);
            camera.transform.Translate(Vector3.right * horizontalInput * scrollSpeed * Time.deltaTime);
        }

        private void MovementHandler()
        {
            var currentMousePosition = Input.mousePosition;
            var difference = currentMousePosition - LastMousePosition;

            camera.transform.RotateAround(new Vector3(2, 2, 0), new Vector3(difference.y, difference.x, difference.z),
                Mathf.SmoothStep(0, 90, Time.deltaTime * rotaionSpeed));
            LastMousePosition = currentMousePosition;
        }

        public void FixedUpdate()
        {
            if (!isGameDone) return;
            if (Input.GetMouseButtonDown(1))
                isMovementAlowed = true;
            if (Input.GetMouseButtonUp(1))
                isMovementAlowed = false;
            ScrollHandler();
            if (isMovementAlowed)
                MovementHandler();
        }

        public Text score;
        public Button b1;
        public Button b2;
        public Button b3;
        public Button b4;
        public Button b5;
        public Sprite mountain;
        public Sprite plain;
        public Sprite lake;
        public Sprite village;
        public Sprite forest;


        public void DisplayDice(int[] dicesValues)
        {
            dice1.text = dicesValues[0].ToString();
            dice2.text = dicesValues[1].ToString();
            dice3.text = dicesValues[2].ToString();
        }

        public void SetSkipButton(bool value, string text = "")
        {
            skipButton.SetActive(value);
            _skipButtonText.text = text;
        }

        public void GameOver()
        {
            SetSkipButton(false);
            Destroy(dice1);
            Destroy(dice2);
            Destroy(dice3);
            isGameDone = true;
        }

        public void DisplayScore(int newScore) =>
            score.text = newScore.ToString();

        public void DisplayTileButtons(Tile clickedTile)
        {
            _clickedTile = clickedTile;
            IsChoosingATile = true;
            var buttons = new[] {b1, b2, b3, b4, b5};
            var count = 0;
            var types = gameManager.GetTileFromDice(6);
            foreach (var type in types)
            {
                switch (type)
                {
                    case TileTypes.Mountain:
                        buttons[count].image.sprite = mountain;
                        buttons[count].gameObject.SetActive(true);
                        count++;
                        break;
                    case TileTypes.Village:
                        buttons[count].image.sprite = village;
                        buttons[count].gameObject.SetActive(true);
                        count++;
                        break;
                    case TileTypes.Lake:
                        buttons[count].image.sprite = lake;
                        buttons[count].gameObject.SetActive(true);
                        count++;
                        break;
                    case TileTypes.Plain:
                        buttons[count].image.sprite = plain;
                        buttons[count].gameObject.SetActive(true);
                        count++;
                        break;
                    case TileTypes.Forest:
                        buttons[count].image.sprite = forest;
                        buttons[count].gameObject.SetActive(true);
                        count++;
                        break;
                    case TileTypes.Empty:
                        Debug.LogError("Tile card can't be empty");
                        break;
                }
            }
        }

        public void ManageChoice(int buttonNumber)
        {
            var buttons = new[] {b1, b2, b3, b4, b5};
            var buttonPressed = buttons[buttonNumber];
            if (buttonPressed.image.sprite == mountain)
                gameManager.MakeTurn(_clickedTile.X, _clickedTile.Y, TileTypes.Mountain);
            if (buttonPressed.image.sprite == plain)
                gameManager.MakeTurn(_clickedTile.X, _clickedTile.Y, TileTypes.Plain);
            if (buttonPressed.image.sprite == forest)
                gameManager.MakeTurn(_clickedTile.X, _clickedTile.Y, TileTypes.Forest);
            if (buttonPressed.image.sprite == lake)
                gameManager.MakeTurn(_clickedTile.X, _clickedTile.Y, TileTypes.Lake);
            if (buttonPressed.image.sprite == village)
                gameManager.MakeTurn(_clickedTile.X, _clickedTile.Y, TileTypes.Village);
        }

        public void SwitchCardsOff()
        {
            var buttons = new[] {b1, b2, b3, b4, b5};
            for (var i = 0; i < 5; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }

            IsChoosingATile = false;
        }
    }
}