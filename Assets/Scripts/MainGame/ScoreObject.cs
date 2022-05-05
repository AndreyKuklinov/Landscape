using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    public class ScoreObject : MonoBehaviour
    {
        private Sprite _objectiveSprite;
        private Image _objectiveImage;

        public void Init(Sprite objectiveSprite, Image objectiveImage)
        {
            _objectiveImage = objectiveImage;
            _objectiveSprite = objectiveSprite;
        }
    
        public void OnPointerEnter()
        {
            _objectiveImage.gameObject.SetActive(true);
            _objectiveImage.sprite = _objectiveSprite;
        }

        public void OnPointerExit()
        {
            _objectiveImage.gameObject.SetActive(false);
        }
    }
}
