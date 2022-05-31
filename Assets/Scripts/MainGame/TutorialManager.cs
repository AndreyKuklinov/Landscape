using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MainGame
{
    public class TutorialManager : MonoBehaviour
    {
        private enum TutorialStages
        {
            MoveCameraWithKeys,
            PlaceTile,
            PickOutOfSeveralTiles1,
            PickOutOfSeveralTiles2,
            PickOutOfSeveralTiles3,
            PlaceQuestion,
            ReadObjective,
            FulfillObjective,
            MoreObjectives,
            Finale
        }

        private readonly KeyCode[] cameraKeys = new KeyCode[]
        {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.E
        };
        private readonly Dictionary<KeyCode, bool> pressedCameraKeys = new Dictionary<KeyCode, bool>()
        {
            { KeyCode.W, false },
            { KeyCode.A, false },
            { KeyCode.S, false },
            { KeyCode.D, false },
            { KeyCode.Q, false },
            { KeyCode.E, false },
        };

        private HashSet<ScoreObject> seenObjectives = new HashSet<ScoreObject>();
        private ScoreObject firstObjective;

        [SerializeField]
        [FormerlySerializedAs("PopUp")] private GameObject popUp;
        [SerializeField]
        [FormerlySerializedAs("GameManager")] private GameManager gameManager;
        [SerializeField] private List<Objective> tutorialObjectives;

        public bool IsTutorialActive { get; private set; }
        public int[,] Moves { get; private set; }
    
        private int Stage;
        private Text PopupText;

        public void Begin()
        {
            IsTutorialActive = true;
            PopupText = popUp.GetComponentInChildren<Text>();
            popUp.SetActive(true);
            Stage = -1;
            ProceedToNextStage();
        }

        private void ProceedToNextStage()
        {
            Stage++;
            var currentStage = (TutorialStages)Stage;
            Moves = new int[gameManager.boardWidth, gameManager.boardWidth];
            switch (currentStage)
            {
                case TutorialStages.MoveCameraWithKeys:
                    PopupText.text = "Подвигайте камеру с помощью клавиш W, A, S, D, а также Q и E";
                    break;
                case TutorialStages.PlaceTile:
                    PopupText.text = "В Landscape вы размещаете клетки, чтобы зарабатывать очки. " +
                                     "Нажмите на клетку с домом, чтобы поставить деревню.";
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[2, 2] = 5;
                    gameManager.boardRenderer.DisplayPossibleMoves();
                    break;
                case TutorialStages.PickOutOfSeveralTiles1:
                    PopupText.text = "Есть 5 видов клеток: Гора, Лес, Степь, Озеро и Деревня. " +
                                     "Каждый ход вам предлагается выбрать один из нескольких вариантов. "
                                     +"Сделайте несколько ходов, чтобы продолжить.";
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[0, 0] = 1;
                    Moves[0, 4] = 2;
                    Moves[4, 4] = 3;
                    Moves[4, 0] = 4;
                    break;
                case TutorialStages.PickOutOfSeveralTiles2:
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[0, 1] = 2;
                    Moves[2, 0] = 4;
                    break;
                case TutorialStages.PickOutOfSeveralTiles3:
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[2, 4] = 1;
                    Moves[4, 2] = 3;
                    break;
                case TutorialStages.PlaceQuestion:
                    PopupText.text = "Иногда на поле будет появляться вопросительный знак. " +
                                     "Он означает возможность поставить любую клетку. " +
                                     "Нажмите на него и выберите тип.";
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[1, 0] = 6;
                    break;
                case TutorialStages.ReadObjective:
                    PopupText.text = "Вы получаете очки за выполнение целей. " +
                                     "Наведите курсор на счётчик в левом верхнем углу, чтобы прочитать вашу цель.";
                    gameManager.Objectives = new[] { tutorialObjectives[0] };
                    gameManager.guiManager.Start();
                    break;
                case TutorialStages.FulfillObjective:
                    PopupText.text = "Поставьте Степь, чтобы выполнить условие и получить очки.";
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[1, 2] = 3;
                    gameManager.boardRenderer.DisplayPossibleMoves();
                    break;
                case TutorialStages.MoreObjectives:
                    PopupText.text = "На игру вам даются 3 цели. Прочитайте их условия.";
                    gameManager.Objectives = new[] { 
                        tutorialObjectives[0], 
                        tutorialObjectives[1], 
                        tutorialObjectives[2] };
                    Destroy(firstObjective.gameObject);
                    gameManager.guiManager.Start();
                    break;
                case TutorialStages.Finale:
                    PopupText.text = "Выполняйте цели, чтобы заработать очки. Подсказка: не фокусируйтесь лишь на одной цели," +
                                     "а старайтесь выполнять их равномерно. Так вы получите больше очков. Удачи!";
                    gameManager.TilePlaced += OnTilePlaced;
                    IsTutorialActive = false;
                    gameManager.boardRenderer.DisplayPossibleMoves();
                    break;
                default:
                    popUp.SetActive(false);
                    break;
            }
        }

        private void OnTilePlaced(object sender, EventArgs e)
        {
            gameManager.TilePlaced -= OnTilePlaced;
            ProceedToNextStage();
        }

        private void Update()
        {
            if (!IsTutorialActive || (TutorialStages)Stage != TutorialStages.MoveCameraWithKeys)
                return;
            var nextStage = true;
            foreach (var key in cameraKeys)
            {
                if (Input.GetKeyDown(key))
                    pressedCameraKeys[key] = true;
                else if (!pressedCameraKeys[key])
                    nextStage = false;
            }
            if(nextStage)
                ProceedToNextStage();
        }

        public void OnObjectiveRead(object sender, EventArgs e)
        {
            if (!IsTutorialActive)
                return;
            var obj = (ScoreObject)sender;
            var stage = (TutorialStages)Stage;
            switch (stage)
            {
                case TutorialStages.ReadObjective:
                    ProceedToNextStage();
                    firstObjective = obj;
                    break;
                case TutorialStages.MoreObjectives:
                {
                    seenObjectives.Add(obj);
                    if(seenObjectives.Count == 3)
                        ProceedToNextStage();
                    break;
                }
            }
        }
    }
}