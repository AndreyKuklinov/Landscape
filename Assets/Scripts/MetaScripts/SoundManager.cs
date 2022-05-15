using UnityEngine;

namespace MetaScripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource buttonClickSound;
        [SerializeField] private AudioSource[] musics;

        private void Start()
        {
            buttonClickSound.volume = 1 + PlayerPrefs.GetFloat("InterfaceVolume");
            foreach (var music in musics)
            {
                Debug.Log(PlayerPrefs.GetFloat("MusicVolume"));
                music.volume = 1 + PlayerPrefs.GetFloat("MusicVolume");
            }
        }

        public void ChangeInterfaceVolume() =>
            buttonClickSound.volume = 1 + PlayerPrefs.GetFloat("InterfaceVolume");

        public void ChangeMusicVolume()
        {
            foreach (var music in musics)
            {
                music.volume = 1 + PlayerPrefs.GetFloat("MusicVolume");
            }
        }

        public void PlayButtonClickSound() =>
            buttonClickSound.Play();

        public void PlayMusic()
        {
            musics[0].Play();
        }
    }
}