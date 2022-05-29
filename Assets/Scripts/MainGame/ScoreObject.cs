using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    public class ScoreObject : MonoBehaviour
    {
        private Sprite ObjectiveSprite;
        private Image ObjectiveImage;
        public event EventHandler PointerExited;

        public void Init(Sprite objectiveSprite, Image objectiveImage)
        {
            ObjectiveImage = objectiveImage;
            ObjectiveSprite = objectiveSprite;
        }

        public void OnPointerEnter()
        {
            ObjectiveImage.gameObject.SetActive(true);
            ObjectiveImage.sprite = ObjectiveSprite;
        }

        public void OnPointerExit()
        {
            ObjectiveImage.gameObject.SetActive(false);
            PointerExited?.Invoke(this, EventArgs.Empty);
        }
            
    }
}