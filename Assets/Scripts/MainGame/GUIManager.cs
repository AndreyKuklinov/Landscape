using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaScripts;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace MainGame
{
    public class GUIManager : MonoBehaviour
    {
        public bool IsMouseOverUI { get; set; }
        [SerializeField] private GameManager gameManager;
        [FormerlySerializedAs("scoreHolder")] [SerializeField] private GameObject scoreObjectHolder;
        [SerializeField] private GameObject scoreObjectPrefab;
        [SerializeField] private Image objectiveImage;
        [SerializeField] private GameObject postProcessing;
        [SerializeField] private PostProcessProfile postProcessProfile;
        [SerializeField] private float bloomingSpeed;
        [SerializeField] private float debloomingSpeed;
        [SerializeField] private float bloomingMax;
        [SerializeField] private float bloomingBase;
        [SerializeField] private float tileLightMaxValue; 
        private Tile clickedTile;
        private List<Text> scoreTexts;
        private Bloom bloom;
        private bool isGameOver;
        private bool isBloomDescending;

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
        [SerializeField] private Canvas mainCanvas;

        public void Start()
        {
            postProcessing.SetActive(!Convert.ToBoolean(PlayerPrefs.GetInt("NOTpostProcessing")));
            scoreTexts = new List<Text>();
            foreach (var obj in gameManager.Objectives)
            {
                var scoreObject = Instantiate(scoreObjectPrefab, scoreObjectHolder.transform);
                var scoreText = scoreObject.transform.Find("ScoreHolder").GetComponentInChildren<Text>();
                var objectiveText = scoreObject.transform.Find("ObjectiveHolder").GetComponentInChildren<Text>();
                var scoreObjectScript = scoreObject.GetComponent<ScoreObject>();
                scoreTexts.Add(scoreText);
                objectiveText.text = obj.text;
                scoreObjectScript.Init(obj);
                scoreObjectScript.CursorEntered += OnCursorEnterScoreObject;
                scoreObjectScript.CursorExited += OnCursorExitScoreObject;
            }
        }

        public void UpdateScore()
        {
            for (var i = 0; i < gameManager.Objectives.Length; i++)
                scoreTexts[i].text = gameManager.Objectives[i].Points.ToString();
        }

        public void BackToTheMenu() => SceneChanger.ChangeScene(0);

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.F))
                mainCanvas.gameObject.SetActive(!mainCanvas.gameObject.activeSelf);
            if (!isGameOver) return;
            if (bloom.intensity.value < bloomingMax && !isBloomDescending)
                bloom.intensity.value += bloomingSpeed * Time.deltaTime;
            if (bloom.intensity.value > bloomingMax - 1) isBloomDescending = true;
            if (bloom.intensity.value > bloomingBase && isBloomDescending)
                bloom.intensity.value -= debloomingSpeed * Time.deltaTime;
        }

        public void GameOver()
        {
            scoreObjectHolder.SetActive(false);
            score.text = gameManager.Score.ToString();
            score.transform.parent.gameObject.SetActive(true);
            postProcessProfile.TryGetSettings(out bloom);
            isGameOver = true;
        }

        public void DisplayTileButtons(Tile clickedTile)
        {
            if (IsMouseOverUI)
                return;
            this.clickedTile = clickedTile;
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
                gameManager.MakeTurn(clickedTile.X, clickedTile.Y, TileTypes.Mountain);
            if (buttonPressed.image.sprite == plain)
                gameManager.MakeTurn(clickedTile.X, clickedTile.Y, TileTypes.Plain);
            if (buttonPressed.image.sprite == forest)
                gameManager.MakeTurn(clickedTile.X, clickedTile.Y, TileTypes.Forest);
            if (buttonPressed.image.sprite == lake)
                gameManager.MakeTurn(clickedTile.X, clickedTile.Y, TileTypes.Lake);
            if (buttonPressed.image.sprite == village)
                gameManager.MakeTurn(clickedTile.X, clickedTile.Y, TileTypes.Village);
            SwitchCardsOff();
        }

        public void SwitchCardsOff()
        {
            var buttons = new[] {b1, b2, b3, b4, b5};
            for (var i = 0; i < 5; i++)
                buttons[i].gameObject.SetActive(false);
            IsMouseOverUI = false;
        }

        public void ChangeLight(GameObject tileToLight, bool isLightOn)
        {
            tileToLight.GetComponent<Light>().intensity = isLightOn ? tileLightMaxValue : 0;
        }

        private void OnCursorEnterScoreObject(object sender, EventArgs e)
        {
            var scoreObject = (ScoreObject)sender;
            foreach (var tile in gameManager.boardRenderer.GameBoard)
                tile.SetPointMarkerVisible(
                    scoreObject.Objective.Criterion(tile.Tile.X, tile.Tile.Y, gameManager) == 1);
        }
        
        private void OnCursorExitScoreObject(object sender, EventArgs e)
        {
            foreach (var tile in gameManager.boardRenderer.GameBoard)
                tile.SetPointMarkerVisible(false);
        }
        
        private void OnDestroy()
        {
            postProcessProfile.TryGetSettings(out bloom);
            bloom.intensity.value = bloomingBase;
        }
    }
}