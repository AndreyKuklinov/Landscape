using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace MainGame
{
    public class GUIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject scoreHolder;
        [SerializeField] private GameObject scoreObjectPrefab;
        [SerializeField] private Image objectiveImage;
        [SerializeField] private GameObject postProcessing;
        [SerializeField] private PostProcessProfile postProcessProfile;
        [SerializeField] private float bloomingSpeed;
        [SerializeField] private float debloomingSpeed;
        [SerializeField] private float bloomingMax;
        [SerializeField] private float bloomingBase;
        private Tile ClickedTile;
        private List<Text> ScoreTexts;
        private Bloom Bloom;
        private bool IsGameOver;
        private bool IsBloomDescending;

        [SerializeField] private Text score;
        [SerializeField] private Button b1;
        [SerializeField] private Button b2;
        [SerializeField] private Button b3;
        [SerializeField] private Button b4;
        [SerializeField] private Button b5;
        [SerializeField] private Sprite mountain;
        [SerializeField] private Sprite plain;
        [SerializeField] private Sprite lake;
        [SerializeField] private Sprite village;
        [SerializeField] private Sprite forest;

        public void Start()
        {
            postProcessing.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("postProcessing")));
            ScoreTexts = new List<Text>();
            foreach (var obj in gameManager.Objectives)
            {
                var scoreObject = Instantiate(scoreObjectPrefab, scoreHolder.transform);
                scoreObject.GetComponent<ScoreObject>().Init(obj.sprite, objectiveImage);
                ScoreTexts.Add(scoreObject.GetComponentInChildren<Text>());
            }
        }

        public void UpdateScore()
        {
            for (var i = 0; i < gameManager.Objectives.Length; i++)
                ScoreTexts[i].text = gameManager.Objectives[i].Points.ToString();
        }

        private void Update()
        {
            if (!IsGameOver) return;
            if (Bloom.intensity.value < bloomingMax && !IsBloomDescending)
                Bloom.intensity.value += bloomingSpeed * Time.deltaTime;
            if (Bloom.intensity.value > bloomingMax - 1) IsBloomDescending = true;
            if (Bloom.intensity.value > bloomingBase && IsBloomDescending)
                Bloom.intensity.value -= debloomingSpeed * Time.deltaTime;
        }

        public void GameOver()
        {
            scoreHolder.SetActive(false);
            score.text = gameManager.Score.ToString();
            score.transform.parent.gameObject.SetActive(true);
            postProcessProfile.TryGetSettings(out Bloom);
            IsGameOver = true;
        }

        public void DisplayTileButtons(Tile clickedTile)
        {
            ClickedTile = clickedTile;
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
                gameManager.MakeTurn(ClickedTile.X, ClickedTile.Y, TileTypes.Mountain);
            if (buttonPressed.image.sprite == plain)
                gameManager.MakeTurn(ClickedTile.X, ClickedTile.Y, TileTypes.Plain);
            if (buttonPressed.image.sprite == forest)
                gameManager.MakeTurn(ClickedTile.X, ClickedTile.Y, TileTypes.Forest);
            if (buttonPressed.image.sprite == lake)
                gameManager.MakeTurn(ClickedTile.X, ClickedTile.Y, TileTypes.Lake);
            if (buttonPressed.image.sprite == village)
                gameManager.MakeTurn(ClickedTile.X, ClickedTile.Y, TileTypes.Village);
        }

        public void SwitchCardsOff()
        {
            var buttons = new[] {b1, b2, b3, b4, b5};
            for (var i = 0; i < 5; i++)
                buttons[i].gameObject.SetActive(false);
        }
    }
}