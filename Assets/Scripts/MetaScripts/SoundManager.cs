using UnityEngine;

namespace MetaScripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] musics;
        private int currentAudio;

        private void FixedUpdate()
        {
            if (musics[currentAudio].isPlaying) return;
            currentAudio++;
            if (currentAudio == musics.Length) currentAudio = 0;
            musics[currentAudio].Play();
        }

        private void Start()
        {
            foreach (var music in musics)
                music.volume = 1 + PlayerPrefs.GetFloat("MusicVolume");
        }

        public void ChangeMusicVolume()
        {
            foreach (var music in musics)
                music.volume = 1 + PlayerPrefs.GetFloat("MusicVolume");
        }
    }
}