using System.Collections.Generic;
using ScenesFolders.MainGame;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    public class GUIManager : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject scoreHolder;
        public GameObject scoreObjectPrefab;
        public Image objectiveImage;
        public bool IsChoosingATile { get; private set; }
        private Tile _clickedTile;
        private List<Text> _scoreTexts;
        
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

        public void Start()
        {
            IsChoosingATile = false;
            _scoreTexts = new List<Text>();
            foreach (var obj in gameManager.Objectives)
            {
                var scoreObject = Instantiate(scoreObjectPrefab, scoreHolder.transform);
                scoreObject.GetComponent<ScoreObject>().Init(obj.sprite, objectiveImage);
                _scoreTexts.Add(scoreObject.GetComponentInChildren<Text>());
            }
        }

        public void UpdateScore()
        {
            for (var i = 0; i < gameManager.Objectives.Length; i++)
            {
                Debug.Log("Objective "+i+": "+gameManager.Objectives[i].Points);
                _scoreTexts[i].text = gameManager.Objectives[i].Points.ToString();
            }
        }
        
        public void GameOver()
        {
            scoreHolder.SetActive(false);
            score.text = gameManager.Score.ToString();
            score.transform.parent.gameObject.SetActive(true);
        }

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
                    default:
                        Debug.LogError("Tile not initialized in GUIManager");
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