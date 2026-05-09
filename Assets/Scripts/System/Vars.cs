using System.Collections;
using System.Collections.Generic;
using Main.BulletSystem;
using Main.EntitySystem;
using Main.Sound;
using UnityEngine;

namespace Main
{
    public delegate IEnumerator CustomCoroutine(GameObject gameObject);
    public static class Vars
    {
        public static readonly int FIXED_UPDATES_PER_SECOND = 60;
        public const short TARGET_FPS = 60;
        public static bool StartedVars { get; private set; } = false;

        public static SoundManager SoundManager;
        public static BulletManager bulletManager;
        public static GameManager gameManager;

        public static void StartVars()
        {
            if (StartedVars)
                return;
            Application.targetFrameRate = 60;

            PhysicsCollisionMatrixLayerMasks.Init();

            //GameStarter.Generate();
            //ResetVolumePrefs();
            SetSoundVolumes();

            StartedVars = true;
        }

        #region PREFERENCE KEYS
        public static string SelectedLanguage { get => GetPrefString(PrefKey.SelectedLanguage); set => SetPrefString(PrefKey.SelectedLanguage, value); }
        public static bool ShowFPS { get => GetPrefBool(PrefKey.ShowFPS); set => SetPrefBool(PrefKey.ShowFPS, value); }

        public static float MasterVolume { get => GetPrefFloat(PrefKey.MasterVolume); set => SetPrefFloat(PrefKey.MasterVolume, value); }
        public static float MusicVolume { get => GetPrefFloat(PrefKey.MusicVolume); set => SetPrefFloat(PrefKey.MusicVolume, value); }
        public static float SoundEffectVolume { get => GetPrefFloat(PrefKey.SoundEffectVolume); set => SetPrefFloat(PrefKey.SoundEffectVolume, value); }
        public static float UIVolume { get => GetPrefFloat(PrefKey.UIVolume); set => SetPrefFloat(PrefKey.UIVolume, value); }

        public enum PrefKey : byte
        {
            None,
            SelectedLanguage,
            ShowFPS,
            MasterVolume,
            MusicVolume,
            SoundEffectVolume,
            UIVolume
        }

        private static string GetPlayerPref(PrefKey key)
        {
            switch (key)
            {
                case PrefKey.SelectedLanguage:
                    return "SELECTED_LANGUAGE";
                case PrefKey.ShowFPS:
                    return "SHOW_FPS";
                case PrefKey.MasterVolume:
                    return "MASTER_VOLUME";
                case PrefKey.MusicVolume:
                    return "MUSIC_VOLUME";
                case PrefKey.SoundEffectVolume:
                    return "SOUND_EFFECTS_VOLUME";
                case PrefKey.UIVolume:
                    return "UI_VOLUME";
            }

            throw new KeyNotFoundException("The key " + key + " was not found for some unknown and scary reason.");
            //return "NULL";
        }

        public static void ResetAllPrefs()
        {
            ResetSystemPrefs();
            ResetGeneralPrefs();
            ResetVolumePrefs();
        }

        public static void ResetSystemPrefs()
        {
            SelectedLanguage = "EN";
            ShowFPS = false;
        }

        public static void ResetGeneralPrefs()
        {

        }

        public static void ResetVolumePrefs()
        {
            MasterVolume = 0.8f;
            MusicVolume = 0.8f;
            SoundEffectVolume = 0.8f;
            UIVolume = 0.8f;
        }

        public static bool GetPrefBool(PrefKey key) => PlayerPrefs.GetInt(GetPlayerPref(key), 0) == 1;
        public static void SetPrefBool(PrefKey key, bool value) => PlayerPrefs.SetInt(GetPlayerPref(key), value ? 1 : 0);
        public static int GetPrefInt(PrefKey key) => PlayerPrefs.GetInt(GetPlayerPref(key), 0);
        public static void SetPrefInt(PrefKey key, int value) => PlayerPrefs.SetInt(GetPlayerPref(key), value);
        public static float GetPrefFloat(PrefKey key) => PlayerPrefs.GetFloat(GetPlayerPref(key), 0f);
        public static void SetPrefFloat(PrefKey key, float value) => PlayerPrefs.SetFloat(GetPlayerPref(key), value);
        public static string GetPrefString(PrefKey key) => PlayerPrefs.GetString(GetPlayerPref(key), "");
        public static void SetPrefString(PrefKey key, string value) => PlayerPrefs.SetString(GetPlayerPref(key), value);
        #endregion

        #region LAYER
        public enum Layer : byte
        {
            Default,
            TransparentFX,
            Ignore_Raycast,
            Background,
            Water,
            UI,
            Scenario,
            Player,
            Enemy,
            NonCollideableEnemy,
            BulletPlayer,
            BulletEnemy
        }

        public static int LayerInt(Layer layer)
        {
            return (int)layer;
        }
        public static int LayerInt(int layer)
        {
            return LayerInt((Layer)layer);
        }
        public static string LayerString(Layer layer)
        {
            return LayerMask.LayerToName((int)layer);
        }

        public static int GetMask(params Layer[] layers)
        {
            var newLayers = new string[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                newLayers[i] = LayerString(layers[i]);
            }
            return LayerMask.GetMask(newLayers);
        }
        #endregion

        #region SOUND

        public static void SetSoundVolumes()
        {
            SoundManager.masterVolume = GetPrefFloat(PrefKey.MasterVolume);
            SoundManager.musicVolume = GetPrefFloat(PrefKey.MusicVolume);
            SoundManager.soundEffectVolume = GetPrefFloat(PrefKey.SoundEffectVolume);
            SoundManager.UIVolume = GetPrefFloat(PrefKey.UIVolume);
        }

        #endregion
    }
}
