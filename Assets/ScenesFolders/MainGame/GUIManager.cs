using UnityEngine;
using UnityEngine.UI;

namespace ScenesFolders.MainGame
{
    public class GUIManager : MonoBehaviour
    {
        public Text dice1;
        public Text dice2;
        public Text dice3;
        public Text score;

        public void DisplayDice(int[] dicesValues)
        {
            dice1.text = dicesValues[0].ToString();
            dice2.text = dicesValues[1].ToString();
            dice3.text = dicesValues[2].ToString();
        }

        public void DisplayScore(int newScore) =>
            score.text = newScore.ToString();
    }
}