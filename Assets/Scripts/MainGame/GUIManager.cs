using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ScenesFolders.MainGame
{
    public class GUIManager : MonoBehaviour
    {
        public Text dice1;
        public Text dice2;
        public Text dice3;
        public GameObject skipButton;
        public GameObject objectiveHolder;
        public GameObject uiImagePrefab;
        public GameManager gameManager;
        public Text score;
        public bool IsChoosingATile { get; private set; }
        private Tile _clickedTile;
        private Text _skipButtonText;
        private GameObject[] _objectives;
        
        // TODO: Oh, kind soul, please, rid us of this.
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

        public void Start()
        {
            _skipButtonText = skipButton.GetComponentInChildren<Text>();
            IsChoosingATile = false;

            _objectives = new GameObject[gameManager.Objectives.Length];
            for (var i = 0; i<_objectives.Length; i++)
            {
                _objectives[i] = Instantiate(uiImagePrefab, transform);
                _objectives[i].GetComponent<Image>().sprite = gameManager.Objectives[i].sprite;
            }
        }
        
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
            dice1.text = "";
            dice2.text = "";
            dice3.text = "";
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

        public void ToggleObjectives()
        {
            objectiveHolder.SetActive(!objectiveHolder.activeSelf);
        }
    }
}
