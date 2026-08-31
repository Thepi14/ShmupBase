using System.Collections;
using System.IO;
using Main.InputSystem;
using Main.ReplaySystem;
using Main.Sound;
using Main.Stages;
using Main.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Main
{
    public delegate IEnumerator CustomCoroutine(GameObject gameObject);
    public static class Vars
    {
        //General
        public const ushort TARGET_FPS = 60;

        public static bool StartedVars { get; private set; } = false;

        public static void StartVars()
        {
            if (StartedVars)
                return;

            if (!HasPrefKey(PrefKey.GameHasEverStarted))
            {
                ResetAllPrefs();
                //setting it to false opens possibility for cutscenes when game first starts.
                SetPrefBool(PrefKey.GameHasEverStarted, true);
            }

            GenerateFolders();

            PhysicsCollisionMatrixLayerMasks.Init();

            //graphics
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Application.runInBackground = false;

            if (!Application.isMobilePlatform)
            {
                Screen.fullScreen = FullScreen;
                Screen.fullScreenMode = ScreenMode;
            }
            else
            {
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }

            //replays
            ReplayManagement.LoadAllReplayFiles();

            //controls
            InputManager.LockMouse(!UseMouse);

            //time
            TimeManager.Start();

            //others
            SetSoundVolumes();
            UseCommasOnScores = true;

#if UNITY_EDITOR
            //ResetAllPrefs();
#endif

            StartedVars = true;
        }

        public static void GenerateFolders()
        {
            Directory.CreateDirectory(ReplayManagement.REPLAYS_PATH);
        }

        #region PREFERENCE KEYS

        //general
        public static bool ShowFPS { get => GetPrefBool(PrefKey.ShowFPS); set => SetPrefBool(PrefKey.ShowFPS, value); }

        //audio
        public static float MasterVolume { get => GetPrefFloat(PrefKey.MasterVolume); set => SetPrefFloat(PrefKey.MasterVolume, value); }
        public static float MusicVolume { get => GetPrefFloat(PrefKey.MusicVolume); set => SetPrefFloat(PrefKey.MusicVolume, value); }
        public static float SoundEffectVolume { get => GetPrefFloat(PrefKey.SoundEffectVolume); set => SetPrefFloat(PrefKey.SoundEffectVolume, value); }
        public static float UIVolume { get => GetPrefFloat(PrefKey.UIVolume); set => SetPrefFloat(PrefKey.UIVolume, value); }

        //controls
        public static bool UseMouse { get { return GetPrefBool(PrefKey.UseMouse); } set { SetPrefBool(PrefKey.UseMouse, value); InputManager.LockMouse(!UseMouse); } }
        public static bool UseIngameKeyboard { get => GetPrefBool(PrefKey.UseIngameKeyboard); set => SetPrefBool(PrefKey.UseIngameKeyboard, value); }

        //graphics
        public static bool FullScreen { get => GetPrefBool(PrefKey.FullScreen); set => SetPrefBool(PrefKey.FullScreen, value); }
        public static FullScreenMode ScreenMode { get => (FullScreenMode)GetPrefInt(PrefKey.ScreenMode); set => SetPrefInt(PrefKey.ScreenMode, (int)value); }
        public static bool UseCommasOnScores { get => GetPrefBool(PrefKey.UseCommasOnScores); set => SetPrefBool(PrefKey.UseCommasOnScores, value); }

        //localization
        public static int SelectedLanguage { get => GetPrefInt(PrefKey.SelectedLanguage); set => SetPrefInt(PrefKey.SelectedLanguage, value); }

        //system
        public static bool SaveReplaysAsJson { get => GetPrefBool(PrefKey.SaveReplaysAsJson); set => SetPrefBool(PrefKey.SaveReplaysAsJson, value); }
        public static bool UseLocalDataPath { get => GetPrefBool(PrefKey.UseLocalDataPath); set => SetPrefBool(PrefKey.UseLocalDataPath, value); }

        //persistent data
        public static bool GameHasEverStarted { get => GetPrefBool(PrefKey.GameHasEverStarted); set => SetPrefBool(PrefKey.GameHasEverStarted, value); }
        public static long Highscore { get => long.Parse(GetPrefString(PrefKey.Highscore)); set => SetPrefString(PrefKey.Highscore, value.ToString()); } //made as string because of the limits of int32, so making it as string is just easier
        public static Difficulty LastDifficulty { get => (Difficulty)GetPrefInt(PrefKey.LastDifficulty); set => SetPrefInt(PrefKey.LastDifficulty, (int)value); }
        public static int LastCharacterID { get => GetPrefInt(PrefKey.LastCharacterID); set => SetPrefInt(PrefKey.LastCharacterID, value); }

        public enum PrefKey : byte
        {
            None,

            //general
            ShowFPS,

            //audio
            MasterVolume,
            MusicVolume,
            SoundEffectVolume,
            UIVolume,

            //controls
            UseMouse,
            UseIngameKeyboard,

            //graphics
            FullScreen,
            ScreenMode,
            UseCommasOnScores,

            //localization
            SelectedLanguage,

            //system
            SaveReplaysAsJson,
            UseLocalDataPath,

            //others
            GameHasEverStarted,
            Highscore,
            LastDifficulty,
            LastCharacterID
        }

        public static void ResetAllPrefs()
        {
            ResetGeneralPrefs();
            ResetSoundPrefs();
            ResetGraphicsPrefs();
            ResetControlsPrefs();

            ResetSystemPrefs();
        }

        public static void ResetGeneralPrefs()
        {
            ShowFPS = false;

            PlayerPrefs.Save();
        }

        public static void ResetSoundPrefs()
        {
            MasterVolume = 0.8f;
            MusicVolume = 0.8f;
            SoundEffectVolume = 0.8f;
            UIVolume = 0.8f;

            PlayerPrefs.Save();
        }

        public static void ResetGraphicsPrefs()
        {
            FullScreen = false;
            ScreenMode = FullScreenMode.Windowed;
            UseCommasOnScores = true;

            PlayerPrefs.Save();
        }

        public static void ResetControlsPrefs()
        {
            UseMouse = false;
            UseIngameKeyboard = true;

            if (SettingsPanel.instance != null)
            {
                SettingsPanel.ResetAllBinds();
            }

            PlayerPrefs.Save();
        }

        public static void ResetSystemPrefs()
        {
            SaveReplaysAsJson = true;
            UseLocalDataPath = false;

            Highscore = 0L;
            LastDifficulty = Difficulty.Normal;
            LastCharacterID = 0;

            PlayerPrefs.Save();
        }

        public const bool ALWAYS_SAVE_PREFS = true;

        public static bool GetPrefBool(PrefKey key) => PlayerPrefs.GetInt(GetPrefKeyString(key), 0) == 1;
        public static void SetPrefBool(PrefKey key, bool value, bool save = ALWAYS_SAVE_PREFS) { PlayerPrefs.SetInt(GetPrefKeyString(key), value ? 1 : 0); if (save) PlayerPrefs.Save(); }
        public static int GetPrefInt(PrefKey key) => PlayerPrefs.GetInt(GetPrefKeyString(key), 0);
        public static void SetPrefInt(PrefKey key, int value, bool save = ALWAYS_SAVE_PREFS) { PlayerPrefs.SetInt(GetPrefKeyString(key), value); if (save) PlayerPrefs.Save(); }
        public static float GetPrefFloat(PrefKey key) => PlayerPrefs.GetFloat(GetPrefKeyString(key), 0f);
        public static void SetPrefFloat(PrefKey key, float value, bool save = ALWAYS_SAVE_PREFS) { PlayerPrefs.SetFloat(GetPrefKeyString(key), value); if (save) PlayerPrefs.Save(); }
        public static string GetPrefString(PrefKey key) => PlayerPrefs.GetString(GetPrefKeyString(key), "");
        public static void SetPrefString(PrefKey key, string value, bool save = ALWAYS_SAVE_PREFS) { PlayerPrefs.SetString(GetPrefKeyString(key), value); if (save) PlayerPrefs.Save(); }

        public static bool HasPrefKey(PrefKey key) => PlayerPrefs.HasKey(GetPrefKeyString(key));
        private static string GetPrefKeyString(PrefKey key) => key.ToString().Prettify().ToUpper().Replace(' ', '_');

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

        #region UI

        public static string FormatAsScoreString(long value) => UseCommasOnScores ? value.ToString("000,000,000,000") : value.ToString("000000000000");

        public static void SelectIfMouseInactive(this Selectable selectable) { if (!UseMouse) { selectable.Select(); } }

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
