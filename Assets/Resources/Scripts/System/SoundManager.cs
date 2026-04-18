using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObjectUtils;
using UnityEngine;
using UnityEngine.Audio;

namespace Main.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Singleton { get; private set; }

        public const string MUSICS_PATH = "Sounds/Musics/";
        public const string SOUNDS_PATH = "Sounds/Sounds/";

        public static float masterVolume, musicVolume, soundEffectVolume, UIVolume;

        [SerializeReference]
        private AudioSource audioSource;

        public AudioResource currentMusic;
        public List<AudioResource> sounds;
        public List<AudioResource> musics;

        protected Dictionary<string, AudioResource> musicsDictionary;

        public bool goToNextMusic = false;
        public bool playRandomMusicOnList = true;
        public bool loopMusic = false;

        private float currentFade = 5f;
        private float currentVolume = 1f;
        private static float musicVolumeMultipliyer;

#if UNITY_EDITOR
        /*private void OnValidate()
        {
            musicDictionary = new Dictionary<string, AudioClipMod>();
            foreach (AudioClip clip in musicList)
            {
                musicDictionary.Add(clip.Name(), clip);
            }
        }*/
#endif

        public void UpdateVolumeMultiplier()
        {
            musicVolumeMultipliyer = masterVolume * musicVolume;
        }

        private void Start()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingleton<SoundManager>(this, Singleton);

            audioSource = GetComponent<AudioSource>();

            sounds = Resources.LoadAll<AudioResource>(SOUNDS_PATH).ToList();
            musics = Resources.LoadAll<AudioResource>(MUSICS_PATH).ToList();

            musicsDictionary = new Dictionary<string, AudioResource>();
            foreach (AudioResource clip in musics)
            {
                musicsDictionary.Add(clip.name, clip);
            }

            UpdateVolumeMultiplier();
        }

        private void Update()
        {
            UpdateVolumeMultiplier();

            if (musics.Count == 0)
                return;
            else if (!Application.isFocused)
                audioSource.Pause();

            audioSource.loop = loopMusic;

            if (musics.Count > 0 && !audioSource.isPlaying)
                if (!loopMusic && playRandomMusicOnList && musics.Count > 1)
                {
                    if (!goToNextMusic)
                        if (audioSource.isPlaying)
                            return;
                        newMusic:
                    var nextClip = musics.GetRandom();
                    if (nextClip == audioSource.clip)
                        goto newMusic;
                    PlayMusic(nextClip);
                }
                else if (loopMusic && musics.Count >= 1)
                {
                    PlayMusic(musics.GetRandom());
                }

            if (currentMusic != null)
                audioSource.volume = currentVolume * musicVolumeMultipliyer;

            goToNextMusic = false;
        }

        public static void StopMusic(float fade = 0f)
        {
            if (fade > 0f)
            {
                Singleton.StartCoroutine(Singleton.FadeOutCoroutine(fade));
            }
            else
            {
                Singleton.StartCoroutine(Singleton.FadeOutCoroutine(Singleton.currentFade));
            }
        }
        public static void PlayMusic(string name, float fade = 0f)
        {
            Singleton.audioSource.volume = musicVolumeMultipliyer;
            Singleton.currentMusic = Singleton.musicsDictionary[name];
            Singleton.StopCoroutine("FadeCoroutine");
            if (fade > 0f)
            {
                Singleton.StartCoroutine(Singleton.FadeInCoroutine(fade));
            }
            else
            {
                Singleton.StartCoroutine(Singleton.FadeInCoroutine(Singleton.currentFade));
            }
        }

        public static void PlayMusic(AudioResource clip, float fade = 0f)
        {
            Singleton.audioSource.volume = musicVolumeMultipliyer;
            Singleton.currentMusic = clip;
            Singleton.StopCoroutine("FadeCoroutine");
            if (fade > 0f)
            {
                Singleton.StartCoroutine(Singleton.FadeInCoroutine(fade));
            }
            else
            {
                Singleton.StartCoroutine(Singleton.FadeInCoroutine(Singleton.currentFade));
            }
        }

        private IEnumerator FadeInCoroutine(float time)
        {
            time *= 100f;
            audioSource.resource = currentMusic;
            audioSource.volume = 0f;
            audioSource.Play();
            for (float a = 1f; a > 0f; a -= 1f / time)
            {
                currentVolume = 1f - a;
                yield return new WaitForSeconds(0.01f);
            }
            audioSource.volume = currentVolume * musicVolumeMultipliyer;
        }

        private IEnumerator FadeOutCoroutine(float time)
        {
            time *= 100f;
            audioSource.volume = currentVolume * musicVolumeMultipliyer;
            for (float a = 1f; a > 0f; a -= 1f / time)
            {
                currentVolume = a;
                yield return new WaitForSeconds(0.01f);
            }
            audioSource.volume = 0f;
            audioSource.Stop();
        }

        public static void PlaySound(AudioResource clip, SoundType type, Vector3? position = null)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound named " + clip.name + " not found.");
                return;
            }

            var volumeMultipliyer = masterVolume *
                (
                type == SoundType.Music ? musicVolume :
                type == SoundType.SoundEffect ? soundEffectVolume :
                type == SoundType.UI ? UIVolume : 1f
                );

            var obj = new GameObject(clip.name);
            AudioSource source = obj.AddComponent<AudioSource>();
            source.resource = clip;
            source.volume = volumeMultipliyer;
            source.Play();

            if (position == null)
                obj.transform.parent = Singleton.transform;
            else
                obj.transform.position = position.Value;

            Singleton.StartCoroutine(SoundDestroyTimer());

            IEnumerator SoundDestroyTimer()
            {
                while (source.isPlaying)
                    yield return new WaitForEndOfFrame();
                Destroy(obj);
            }
        }

        public static void PlaySound(string name, SoundType type, Vector3? position = null)
        {
            AudioResource clip = null;
            foreach (var sound in Singleton.sounds)
            {
                if (sound.name == name)
                {
                    clip = sound;
                    break;
                }
            }

            PlaySound(clip, type, position);
        }

        public enum SoundType
        {
            Any = 0,
            Music = 1,
            SoundEffect = 2,
            UI = 3,
        }
    }
}
