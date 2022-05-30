using System;
using System.Collections;
using System.Collections.Generic;
using MainGame;
using UnityEngine;
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
    
    public bool IsTutorialActive { get; private set; }
    public GameObject PopUp;
    public GameManager GameManager;
    
    
    private int _stage;
    private Text _popupText;

    public void Start()
    {
        IsTutorialActive = true;
        _popupText = PopUp.GetComponentInChildren<Text>();
        PopUp.SetActive(true);
        _stage = -1;
        ProceedToNextStage();
    }

    private void ProceedToNextStage()
    {
        _stage++;
        var stageName = _stages[_stage];

        switch (stageName)
        {
            case "placeTile":
                _popupText.text = "Нажми на кнопку с деревьями, чтобы поставить лес";
                GameManager.TilePlaced += OnTilePlaced;
                break;
            default:
                _popupText.text = "Туториал сломался :(";
                break;
        }
    }

    private void OnTilePlaced(object sender, EventArgs e)
    {
        GameManager.TilePlaced -= OnTilePlaced;
        ProceedToNextStage();
    }
}