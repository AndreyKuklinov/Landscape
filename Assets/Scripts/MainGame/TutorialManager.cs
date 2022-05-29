using System;
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

    public void Begin(GameManager gameManager)
    {
        IsTutorialActive = true;
        _stage = 0;
        _popupText = PopUp.GetComponent<Text>();
        gameManager.OnTilePlaced += OnTilePlaced;
    }

    private void OnTilePlaced(object sender, EventArgs e)
    {
        //TODO
    }

    private void OnObjectiveSeen(object sender, EventArgs e)
    {
        //TODO
    }

    private void OnCameraMoved(object sender, EventArgs e)
    {
        //TODO
    }
}
