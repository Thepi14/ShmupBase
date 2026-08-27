using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorTools;
using ObjectUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//using TagLib;

namespace Main.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundManager : MonoBehaviour
    {
        public static SoundManager Singleton { get; private set; }

        public const string SOUNDS_PATH = "Sounds/SoundEffects/", MUSICS_PATH = "Sounds/Musics/";

#if UNITY_EDITOR
#pragma warning disable 0414
        [ShowOnly]
        [SerializeField]
        private string soundsPath, musicsPath;
#pragma warning restore 0414
#endif

        [Space(10f)]
        public static float masterVolume, musicVolume, soundEffectVolume, UIVolume;

        [Space(10f)]
        public MusicContainer currentMusic;
        public int musicID = -1;

        [Space(10f)]
        public List<AudioResource> sounds;
        public List<MusicContainer> musics;

        private Dictionary<string, AudioResource> soundsDictionary;
        private Dictionary<string, MusicContainer> musicsDictionary;

        public int randomSeed = 0;
        public static System.Random random;

        [Space(10f)]
        public bool goToNextMusic = false;
        public bool playRandomMusicOnList = true;
        public bool loopMusic = false;

        [Space(10f)]
        public List<AudioSource> currentAudioSources = new();

        private AudioSource mainAudioSource;

        private float currentFade = 5f;
        private float currentVolume = 1f;
        private static float musicVolumeMultipliyer;
        private static bool keepPlayingWhenUnfocused = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            mainAudioSource = GetComponent<AudioSource>();

            soundsPath = "Assets/Resources/" + SOUNDS_PATH;
            musicsPath = "Assets/Resources/" + MUSICS_PATH;

            UpdateLists();
        }
#endif

        public void UpdateVolumeMultiplier()
        {
            musicVolumeMultipliyer = masterVolume * musicVolume;
        }

        private void UpdateLists()
        {
            sounds = Resources.LoadAll<AudioResource>(SOUNDS_PATH).ToList();
            musics = Resources.LoadAll<MusicContainer>(MUSICS_PATH).ToList();

            soundsDictionary = new Dictionary<string, AudioResource>();
            musicsDictionary = new Dictionary<string, MusicContainer>();

            foreach (AudioResource clip in sounds)
            {
                soundsDictionary.Add(clip.name, clip);
            }
            foreach (MusicContainer clip in musics)
            {
                musicsDictionary.Add(char.IsNumber(clip.name.ToCharArray()[0]) ? clip.name.Remove(0, 1) : clip.name, clip);
            }
        }

        public static void RemoveNullsSoundSources()
        {
            Singleton.currentAudioSources.RemoveAll((AudioSource ar) => ar == null);
        }

        private void Awake()
        {
            random ??= new System.Random(randomSeed);

            Singleton = MonoBehaviourGeneral.DeclareSingleton(this, Singleton);

            UpdateLists();

            mainAudioSource = GetComponent<AudioSource>();
            currentAudioSources = new();

            UpdateVolumeMultiplier();
        }

        private void OnDisable()
        {
            mainAudioSource.Pause();
        }

        private void OnEnable()
        {
            mainAudioSource.Play();
        }

        private void LateUpdate()
        {
            UpdateVolumeMultiplier();
            RemoveNullsSoundSources();

            if (musics.Count == 0)
                return;
            else if (keepPlayingWhenUnfocused)
            {
               if (!Application.isFocused)
                    mainAudioSource.Pause();
                else if (Application.isFocused)
                    mainAudioSource.Play();
            }

            mainAudioSource.loop = loopMusic;

            if (musics.Count > 0 && !mainAudioSource.isPlaying && !loopMusic)
            {
                if (playRandomMusicOnList)
                {
                    var copyList = musics.ToList();
                    copyList.Remove(currentMusic);
                    PlayMusic(copyList.GetRandom(random));
                }
                else
                {
                    bool next = false;
                    MusicContainer audio = null;
                    foreach (var music in musics)
                    {
                        if (next)
                        {
                            audio = music;
                            break;
                        }
                        if (music == currentMusic)
                            next = true;
                        else if (music == musics.Last())
                        {
                            audio = musics.First();
                            break;
                        }
                    }

                    PlayMusic(audio);
                }
            }
            if (currentMusic != null)
                mainAudioSource.volume = currentVolume * musicVolumeMultipliyer;

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

        public static void PlayMusic(int index, float fade = 0f, bool replayIfPlaying = false) => PlayMusic(Singleton.musics[index], fade, replayIfPlaying);

        public static void PlayMusic(string name, float fade = 0f, bool replayIfPlaying = false) => PlayMusic(Singleton.musicsDictionary[name], fade, replayIfPlaying);

        public static void PlayMusic(MusicContainer clip, float fade = 0f, bool replayIfPlaying = false)
        {
            if (clip == Singleton.currentMusic && !replayIfPlaying)
                return;

            Singleton.mainAudioSource.volume = musicVolumeMultipliyer;
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
            mainAudioSource.resource = currentMusic.audioClip;
            mainAudioSource.volume = 0f;
            mainAudioSource.Play();

            for (float a = 1f; a > 0f; a -= 1f / time)
            {
                currentVolume = 1f - a;
                yield return new WaitForSeconds(0.01f);
            }
            mainAudioSource.volume = currentVolume * musicVolumeMultipliyer;
        }

        private IEnumerator FadeOutCoroutine(float time)
        {
            time *= 100f;
            mainAudioSource.volume = currentVolume * musicVolumeMultipliyer;

            for (float a = 1f; a > 0f; a -= 1f / time)
            {
                currentVolume = a;
                yield return new WaitForSeconds(0.01f);
            }
            mainAudioSource.volume = 0f;
            mainAudioSource.Stop();
        }

        public static void PlaySound(AudioResource clip, SoundType type, Vector3? position = null, bool scaleWithTime = true, PlayMode playMode = PlayMode.Default)
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

            Singleton.currentAudioSources.Add(source);

            if (position == null)
                obj.transform.parent = Singleton.transform;
            else
                obj.transform.position = position.Value;

            Singleton.StartCoroutine(SoundDestroyTimer());

            IEnumerator SoundDestroyTimer()
            {
                while (source.isPlaying)
                {
                    if (scaleWithTime)
                        source.pitch = Time.timeScale;

                    if (TimeManager.GameIsPaused && (playMode == PlayMode.DoNotPlayOnPause || !(playMode == PlayMode.Default && type == SoundType.UI)))
                        source.Pause();
                    else
                        source.UnPause();

                    yield return new WaitForEndOfFrame();
                }
                Destroy(obj);
            }
        }

        public static void PlaySound(string name, SoundType type, Vector3? position = null, bool scaleWithTime = true, PlayMode playMode = PlayMode.Default)
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

            PlaySound(clip, type, position, scaleWithTime, playMode);
        }

        public enum SoundType
        {
            Any,
            Music,
            SoundEffect,
            UI
        }

        public enum PlayMode
        {
            Default,
            PlayOnPause,
            DoNotPlayOnPause
        }
    }
}
