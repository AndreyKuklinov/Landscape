using System;
using TreeEditor;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Water.Scripts;

namespace StartMenu
{
    public class GUIManager : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Material day;
        [SerializeField] private Material night;
        [SerializeField] private Material oceanMaterial;


        private void Awake()
        {
            var currentDate = DateTime.Now;
            if (currentDate.Hour > 6 && currentDate.Hour < 20)
            {
                RenderSettings.skybox = day;
                oceanMaterial.color = new Color(112, 12, 133);
            }
            else
            {
                RenderSettings.skybox = night;
                oceanMaterial.color = new Color(0, 167, 255);
            }

        }

        private void Start()
        {
            scoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();
        
        }
    
    
    }
}
