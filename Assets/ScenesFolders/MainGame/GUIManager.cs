using UnityEngine;
using UnityEngine.UI;

namespace ScenesFolders.MainGame
{
    public class GUIManager : MonoBehaviour
    {
        public Text dice1;
        public Text dice2;
        public Text dice3;
        public GameObject skipButton;
        private Text skipButtonText;

        public void Start()
        {
            skipButtonText = skipButton.GetComponentInChildren<Text>();
        }
        
        public void DisplayDice(int[] dicesValues)
        {
            dice1.text = dicesValues[0].ToString();
            dice2.text = dicesValues[1].ToString();
            dice3.text = dicesValues[2].ToString();
        }

        public void SetSkipButton(bool value, string text = "")
        { 
            skipButton.SetActive(value);
            skipButtonText.text = text;
        }

        public void GameOver()
        {
            SetSkipButton(false);
            dice1.text = "";
            dice2.text = "";
            dice3.text = "";
        }
    }
}
