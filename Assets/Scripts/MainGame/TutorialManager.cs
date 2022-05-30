using System;
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
        }
    
        [SerializeField]
        [FormerlySerializedAs("PopUp")] private GameObject popUp;
        [SerializeField]
        [FormerlySerializedAs("GameManager")] private GameManager gameManager;
        [SerializeField] private Camera mainCamera;
    
        public bool IsTutorialActive { get; private set; }
        public int[,] Moves { get; private set; }
    
        private int Stage;
        private Text PopupText;
        private SimpleCameraController CameraController;

        public void Begin()
        {
            IsTutorialActive = true;
            PopupText = popUp.GetComponentInChildren<Text>();
            CameraController = mainCamera.GetComponent<SimpleCameraController>();
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
                    CameraController.CameraMoved += OnCameraMoved;
                    break;
                case TutorialStages.PlaceTile:
                    PopupText.text = "В Landscape вы размещаете клетки, чтобы зарабатывать очки. " +
                                     "Нажмите на клетку с домом, чтобы поставить деревню.";
                    gameManager.TilePlaced += OnTilePlaced;
                    Moves[2, 2] = 5;
                    gameManager.boardRenderer.DisplayPossibleMoves();
                    break;
                case TutorialStages.PickOutOfSeveralTiles1:
                    PopupText.text = "Есть 5 видов клеток: Гора, Лес, Равнина, Озеро и Деревня. " +
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
                default:
                    PopupText.text = "Туториал сломался :(";
                    break;
            }
        }

        private void OnTilePlaced(object sender, EventArgs e)
        {
            gameManager.TilePlaced -= OnTilePlaced;
            ProceedToNextStage();
        }

        private void OnCameraMoved(object sender, EventArgs e)
        {
            CameraController.CameraMoved -= OnCameraMoved;
            ProceedToNextStage();
        }
    }
}