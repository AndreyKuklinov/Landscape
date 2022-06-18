using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    public class ScoreObject : MonoBehaviour
    {
        private Sprite ObjectiveSprite;
        private Image ObjectiveImage;
        public Objective Objective { get; private set; }
        public event EventHandler PointerEntered;
        public event EventHandler PointerExited;
        
        public void Init(Sprite objectiveSprite, Image objectiveImage, Objective objective)
        {
            ObjectiveImage = objectiveImage;
            ObjectiveSprite = objectiveSprite;
            Objective = objective;
        }

        public void OnPointerEnter()
        {
            ObjectiveImage.gameObject.SetActive(true);
            ObjectiveImage.sprite = ObjectiveSprite;
            PointerEntered?.Invoke(this, EventArgs.Empty);
        }

        public void OnPointerExit()
        {
            ObjectiveImage.gameObject.SetActive(false);
            PointerExited?.Invoke(this, EventArgs.Empty);
        }
    }
}