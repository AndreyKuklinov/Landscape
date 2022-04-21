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
        private Text skipButtonText;

        public void Start()
        {
            skipButtonText = skipButton.GetComponentInChildren<Text>();
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
            skipButtonText.text = text;
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

        public void DisplayAvailableTiles(GameManager gm, int diceValue)
        {
            var buttons = new[] {b1, b2, b3, b4, b5};
            var count = 0;
            var types = gm.GetTileFromDice(diceValue);
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
            var tileObj = gameObject.AddComponent<TileObject>();
            var buttonPressed = buttons[buttonNumber];
            if (buttonPressed.image.sprite == mountain)
                tileObj.ChoseTileType( TileTypes.Mountain);
            if (buttonPressed.image.sprite == plain)
                tileObj.ChoseTileType( TileTypes.Plain);
            if (buttonPressed.image.sprite == forest)
                tileObj.ChoseTileType(TileTypes.Forest);
            if (buttonPressed.image.sprite == lake)
                tileObj.ChoseTileType(TileTypes.Lake);
            if (buttonPressed.image.sprite == village)
                tileObj.ChoseTileType(TileTypes.Village);
        }

        public void SwitchCardsOff()
        {
            var buttons = new[] {b1, b2, b3, b4, b5};
            for (var i = 0; i < 5; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }
}
