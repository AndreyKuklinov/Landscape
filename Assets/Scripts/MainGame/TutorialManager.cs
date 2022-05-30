using System;
using System.Collections;
using System.Collections.Generic;
using MainGame;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private static readonly string[] _stages = new string[]
    {
        "moveCameraWithKeys",
        "placeTile",
        "pickOutOfSeveralTiles1",
        "pickOutOfSeveralTiles2",
        "pickOutOfSeveralTiles3",
        "placeQuestion",
        "readObjective",
        "fulfillObjective",
        "moveCamera"
    };
    
    [SerializeField]
    [FormerlySerializedAs("PopUp")] private GameObject popUp;
    [SerializeField]
    [FormerlySerializedAs("GameManager")] private GameManager gameManager;
    [SerializeField] private Camera mainCamera;
    
    public bool IsTutorialActive { get; private set; }
    public int[,] Moves { get; private set; }
    
    private int _stage;
    private Text _popupText;
    private SimpleCameraController _cameraController;

    public void Begin()
    {
        IsTutorialActive = true;
        _popupText = popUp.GetComponentInChildren<Text>();
        _cameraController = mainCamera.GetComponent<SimpleCameraController>();
        popUp.SetActive(true);
        _stage = -1;
        ProceedToNextStage();
    }

    private void ProceedToNextStage()
    {
        _stage++;
        var stageName = _stages[_stage];
        Moves = new int[gameManager.boardWidth, gameManager.boardWidth];

        switch (stageName)
        {
            case "moveCameraWithKeys":
                _popupText.text = "Подвигайте камеру с помощью клавиш W, A, S, D, а также Q и E";
                _cameraController.CameraMoved += OnCameraMoved;
                break;
            case "placeTile":
                _popupText.text = "В Landscape вы размещаете клетки, чтобы зарабатывать очки. " +
                                  "Нажмите на клетку с домом, чтобы поставить деревню.";
                gameManager.TilePlaced += OnTilePlaced;
                Moves[2, 2] = 5;
                break;
            case "pickOutOfSeveralTiles1":
                _popupText.text = "Есть 5 видов клеток: Гора, Лес, Равнина, Озеро и Деревня. " +
                                  "Каждый ход вам предлагается выбрать один из нескольких вариантов. "
                                  +"Сделайте несколько ходов, чтобы продолжить.";
                gameManager.TilePlaced += OnTilePlaced;
                Moves[0, 0] = 1;
                Moves[0, 4] = 2;
                Moves[4, 4] = 3;
                Moves[4, 0] = 4;
                break;
            case "pickOutOfSeveralTiles2":
                gameManager.TilePlaced += OnTilePlaced;
                Moves[0, 1] = 2;
                Moves[2, 0] = 4;
                break;
            case "pickOutOfSeveralTiles3":
                gameManager.TilePlaced += OnTilePlaced;
                Moves[2, 4] = 1;
                Moves[4, 2] = 3;
                break;
            case "placeQuestion":
                _popupText.text = "Иногда на поле будет появляться вопросительный знак. " +
                                  "Он означает возможность поставить любую клетку. " +
                                  "Нажмите на него и выберите тип.";
                gameManager.TilePlaced += OnTilePlaced;
                Moves[1, 0] = 6;
                break;
            default:
                _popupText.text = "Туториал сломался :(";
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
        _cameraController.CameraMoved -= OnCameraMoved;
        ProceedToNextStage();
    }
}