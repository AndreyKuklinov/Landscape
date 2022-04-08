using System;
using UnityEngine;
using static ScenesFolders.MainGame.DirectionsEnum;

namespace ScenesFolders.MainGame
{
    public class AnimationsController : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            //do passive animations
        }

        public void StartMovingAnimation(DirectionsEnum direction, GameObject gameObject)
        {
            //animator.SetTrigger(...);
            throw new NotImplementedException();
        }

        public void StartPlacingAnimation()
        {
            //animator.SetTrigger(...);
            throw new NotImplementedException();
        }
    }
}