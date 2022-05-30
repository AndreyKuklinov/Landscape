﻿using System;
using System.Collections;
using System.Collections.Generic;
using MainGame;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public bool IsTutorialActive { get; private set; }
    public GameObject PopUp;

    private int _stage;
    private Text _popupText;
    private GameManager _gameManager;

    public void Begin(GameManager gameManager)
    {
        IsTutorialActive = true;
        _stage = 0;
        _popupText = PopUp.GetComponent<Text>();

        gameManager.TilePlaced += ProceedToNextStage;
        _popupText.text = "Нажми на кнопку с деревьями, чтобы поставить лес";
    }

    private void ProceedToNextStage(object sender, EventArgs e)
    {
        _popupText.text = "Тест удался!";
    }
}