using UnityEngine;

namespace MetaScripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] musics;
        private int CurrentAudio;

        private void FixedUpdate()
        {
            if (musics[CurrentAudio].isPlaying) return;
            CurrentAudio++;
            if (CurrentAudio == musics.Length) CurrentAudio = 0;
            musics[CurrentAudio].Play();
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