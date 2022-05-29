using System;
using System.Collections;
using System.Collections.Generic;
using MainGame;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public bool IsTutorialActive { get; private set; }
    public TileTypes[,] Moves { get; private set; }
    public Objective[,] Objectives { get; private set; }
    public GameObject PopUp;

    private int _stage;
    private Text _popupText;

    public void Begin()
    {
        IsTutorialActive = true;
        _stage = 0;
        _popupText = PopUp.GetComponent<Text>();
    }
}
