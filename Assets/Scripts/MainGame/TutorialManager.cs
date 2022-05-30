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
        "placeTile",
        "pickOutOfSeveralTiles",
        "placeQuestion",
        "readObjective",
        "fulfillObjective",
        "moveCamera"
    };
    
    [SerializeField]
    [FormerlySerializedAs("PopUp")] private GameObject popUp;
    [SerializeField]
    [FormerlySerializedAs("GameManager")] private GameManager gameManager;
    
    public bool IsTutorialActive { get; private set; }
    public int[,] Moves { get; private set; }
    
    private int _stage;
    private Text _popupText;

    public void Begin()
    {
        IsTutorialActive = true;
        _popupText = popUp.GetComponentInChildren<Text>();
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
            case "placeTile":
                _popupText.text = "В Landscape ваша задача -- размещать клетки и получать очки. Нажмите на клетку с деревьями, чтобы поставить лес.";
                gameManager.TilePlaced += OnTilePlaced;
                Moves[2, 2] = 2;
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
}