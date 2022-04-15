using System;
using UnityEngine;

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

        public static void StartMovingAnimation(int startX, int startY, int targetX, int targetY)
        {
            //animator.SetTrigger(...);
            throw new NotImplementedException();
        }

        public static void StartPlacingAnimation(int x, int y, TileTypes tileType, TileVariations tileTypeVariant)
        {
            //animator.SetTrigger(...);
            throw new NotImplementedException();
        }

        public static void StartRoadCreationAnimation(int x, int y, TileTypes tileType, RoadDirection roadDirection)
        {
            //animator.SetTrigger(...);
            throw new NotImplementedException();
        }
    }
}   