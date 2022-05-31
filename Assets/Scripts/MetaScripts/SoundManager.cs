using UnityEngine;

namespace MetaScripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] musics;
        [SerializeField] private AudioSource[] sfx;
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
            foreach (var sound in sfx)
                sound.volume = 1 + PlayerPrefs.GetFloat("MusicVolume");
        }

        public void PlayTilePlacementSound() =>
            sfx[Random.Range(0, sfx.Length - 1)].Play();

        public void ChangeMusicVolume()
        {
            foreach (var music in musics)
                music.volume = 1 + PlayerPrefs.GetFloat("MusicVolume");
        }
    }
}