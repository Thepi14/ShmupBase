using System.Collections;
using System.Collections.Generic;
using Main.InputSystem;
using Main.UI;
using ObjectUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static Main.Vars;

namespace Main.UI
{
    public class SettingsUI : PanelBehaviour
    {
        public RectTransform subPanel;

        [Header("Exits")]
        [Space(20f)]
        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private Button exitButtonDown;

        [Header("Settings SubPanels")]
        [Space(20f)]
        [SerializeField]
        private RectTransform categoryButtonsLayout;

        [Space(20f)]
        [SerializeField]
        private Button generalButton;
        [SerializeField]
        private Button soundButton;
        [SerializeField]
        private Button graphicsButton;
        [SerializeField]
        private Button controlsButton;
        [SerializeField]
        private Button languageButton;

        [Space(20f)]
        [SerializeField]
        private RectTransform generalSubPanel;
        [SerializeField]
        private RectTransform soundSubPanel;
        [SerializeField]
        private RectTransform graphicsSubPanel;
        [SerializeField]
        private RectTransform controlsSubPanel;
        [SerializeField]
        private RectTransform languageSubPanel;

        #region Settings Elements

        [Header("General")]
        [Space(20f)]

        [SerializeField]
        private Toggle toggleFPS;

        [Header("Sound")]
        [Space(20f)]

        [SerializeField]
        private Slider masterVolume;
        [SerializeField]
        private Slider musicVolume;
        [SerializeField]
        private Slider soundEffectVolume;
        [SerializeField]
        private Slider UIVolume;
        [SerializeField]
        private Button resetVolumeButton;

        [Header("Graphics")]
        [Space(20f)]
        [SerializeField]
        private Button veryLowQualityButton;
        [SerializeField]
        private Button lowQualityButton;
        [SerializeField]
        private Button mediumQualityButton;
        [SerializeField]
        private Button highQualityButton;
        [SerializeField]
        private Button veryHighQualityButton;
        [SerializeField]
        private Button ultraQualityButton;

        [Space(20f)]
        [SerializeField]
        private List<Button> qualityButtonList;

        [Space(20f)]
        [SerializeField]
        private Toggle fullScreenToggle;
        [SerializeField]
        private RectTransform screenTypeToggleGroup;

        [SerializeField]
        private Toggle fullScreenWindowToggle;
        [SerializeField]
        private Toggle maximizedWindowToggle;
        //windows only
        [SerializeField]
        private Toggle fullScreenExclusiveToggle;

        [Header("Controls")]
        [Space(20f)]

        [SerializeField]
        private GameObject rebindPrefab;
        [SerializeField]
        private bool generatePrefabsOnRuntime = false;

        [SerializeField]
        private Selectable firstSelectableControlBinder;
        [SerializeField]
        private Toggle useMouseToggle;
        [SerializeField]
        private Toggle useIngameKeyboardToggle;

        [SerializeField]
        private RectTransform controlsLayout;
        [SerializeField]
        private List<RebindActionUI> rebinders;
        [SerializeField]
        private Button resetControlsButton;

        [Header("Localization")]
        [Space(20f)]

        [SerializeField]
        private GameObject languageButtonPrefab;
        [SerializeField]
        private RectTransform languageContent;
        [SerializeField]
        private List<Button> languageButtons;

        #endregion

        protected override void Awake()
        {
            base.Awake();

            //exit
            if (exitButton != null)
                exitButton.onClick.AddListener(() => { SetOpenPanel(false); });
            if (exitButtonDown != null)
                exitButtonDown.onClick.AddListener(() => { SetOpenPanel(false); });

            //selection
            generalButton.onClick.AddListener(() => { OpenSubPanel(generalSubPanel); SelectWhenMouseInactive(toggleFPS); });
            soundButton.onClick.AddListener(() => { OpenSubPanel(soundSubPanel); SelectWhenMouseInactive(masterVolume); });
            controlsButton.onClick.AddListener(() => { OpenSubPanel(controlsSubPanel); SelectWhenMouseInactive(firstSelectableControlBinder); });
            graphicsButton.onClick.AddListener(() => { OpenSubPanel(graphicsSubPanel); SelectWhenMouseInactive(qualityButtonList[0]); });
            languageButton.onClick.AddListener(() => { OpenSubPanel(languageSubPanel); SelectLastLocaleButton(); });

            SetOnSelectOnButtonsLayout(!UseMouse);

            //general
            toggleFPS.isOn = ShowFPS;

            toggleFPS.onValueChanged.AddListener((value) => { ShowFPS = value; });

            //sound
            masterVolume.value = MasterVolume;
            musicVolume.value = MusicVolume;
            soundEffectVolume.value = SoundEffectVolume;
            UIVolume.value = Vars.UIVolume;

            masterVolume.onValueChanged.AddListener((value) => { SetKeySoundVolume(PrefKey.MasterVolume, value); });
            musicVolume.onValueChanged.AddListener((value) => { SetKeySoundVolume(PrefKey.MusicVolume, value); });
            soundEffectVolume.onValueChanged.AddListener((value) => { SetKeySoundVolume(PrefKey.SoundEffectVolume, value); });
            UIVolume.onValueChanged.AddListener((value) => { SetKeySoundVolume(PrefKey.UIVolume, value); });

            resetVolumeButton.onClick.AddListener(() =>
            {
                ResetVolumePrefs();

                masterVolume.value = MasterVolume;
                musicVolume.value = MusicVolume;
                soundEffectVolume.value = SoundEffectVolume;
                UIVolume.value = Vars.UIVolume;
            });

            //graphics

            qualityButtonList = new List<Button>() { veryLowQualityButton, lowQualityButton, mediumQualityButton, highQualityButton, veryHighQualityButton, ultraQualityButton };

            int i = 0;
            foreach (Button button in qualityButtonList)
            {
                int j = i;
                button.onClick.AddListener(() => { QualitySettings.SetQualityLevel(j, true); });
                i++;
            }

            SetUpScreenToggles();

            //controls
            useMouseToggle.isOn = UseMouse;
            useIngameKeyboardToggle.isOn = UseIngameKeyboard;

            useMouseToggle.onValueChanged.AddListener((value) => { UseMouse = value; SetOnSelectOnButtonsLayout(!value); });
            useIngameKeyboardToggle.onValueChanged.AddListener((value) => { UseIngameKeyboard = value; });

            /*if (generatePrefabsOnRuntime)
            {
                foreach (var control in controlsLayout.GetGameObjectChildren())
                    Destroy(control);

                foreach (var bind in InputManager.playerInputSystem.asset.actionMaps[0])
                {
                    var newRebinder = Instantiate(rebindPrefab);
                    newRebinder.GetComponent<RectTransform>().SetParent(controlsLayout);
                    newRebinder.name = bind.name;

                    newRebinder.GetComponent<RebindActionUI>().actionReference.Set(bind);
                }
            }*/
            
            foreach (var control in controlsLayout.GetGameObjectChildren())
                rebinders.Add(control.GetComponent<RebindActionUI>());

            resetControlsButton.onClick.AddListener(() =>
            {
                foreach (var rebinder in rebinders)
                    rebinder.ResetToDefault();
            });

            //localization
            SetLocalizationButtons();

            //others
            //graphicsButton.gameObject.SetActive(false);

            //languageButton.navigation.selectOnDown = languageButtons[0];
        }

        public void SelectWhenMouseInactive(Selectable selectable)
        {
            if (!UseMouse)
            {
                selectable.Select();
            }
        }

        protected override void Start()
        {
            base.Start();

            CloseAllSubPanels();
        }

        #region General



        #endregion

        #region Sound

        public void SetKeySoundVolume(PrefKey key, float value)
        {
            SetPrefFloat(key, value);
            SetSoundVolumes();
        }

        #endregion
        
        #region Graphics

        public void SetUpScreenToggles()
        {
            //if mobile then there is no screen configuration buttons
            if (!Application.isMobilePlatform)
            {
                fullScreenToggle.SetIsOnWithoutNotify(FullScreen);

                fullScreenToggle.onValueChanged.AddListener((value) =>
                {
                    Screen.fullScreen = value;
                    FullScreen = value;
                    //Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

                    screenTypeToggleGroup.GetComponent<CanvasGroup>().alpha = value ? 1f : 0f;
                    screenTypeToggleGroup.GetComponent<CanvasGroup>().interactable = value;
                });

                fullScreenWindowToggle.SetIsOnWithoutNotify(ScreenMode == FullScreenMode.FullScreenWindow);
                fullScreenExclusiveToggle.SetIsOnWithoutNotify(ScreenMode == FullScreenMode.ExclusiveFullScreen);
                maximizedWindowToggle.SetIsOnWithoutNotify(ScreenMode == FullScreenMode.MaximizedWindow);

                fullScreenWindowToggle.onValueChanged.AddListener((value) => { if (value) { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; ScreenMode = FullScreenMode.FullScreenWindow; } });
                maximizedWindowToggle.onValueChanged.AddListener((value) => { if (value) { Screen.fullScreenMode = FullScreenMode.MaximizedWindow; ScreenMode = FullScreenMode.MaximizedWindow; } });

                //windows only
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    fullScreenExclusiveToggle.onValueChanged.AddListener((value) => { if (value) { Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; ScreenMode = FullScreenMode.ExclusiveFullScreen; } });
                }
                else
                {
                    fullScreenExclusiveToggle.gameObject.SetActive(false);
                }

                screenTypeToggleGroup.GetComponent<CanvasGroup>().alpha = FullScreen ? 1f : 0f;
                screenTypeToggleGroup.GetComponent<CanvasGroup>().interactable = FullScreen;
            }
            else
            {
                fullScreenToggle.gameObject.SetActive(false);
                screenTypeToggleGroup.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Controls



        #endregion

        #region Localization

        public const string RESOURCE_FLAG_PATH = "Sprites/OutsideAtlas/Flags/";
        private int selected = 0;

        public void SetLocalizationButtons()
        {
            StartCoroutine(StartLocalizationButtons());
            languageButtons = new();

            IEnumerator StartLocalizationButtons()
            {
                // Wait for the localization system to initialize
                yield return LocalizationSettings.InitializationOperation;

                ChangeLocale(SelectedLanguage);

                // Generate list of available Locales
                selected = 0;
                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
                {
                    var locale = LocalizationSettings.AvailableLocales.Locales[i];
                    int j = i;
                    if (LocalizationSettings.SelectedLocale == locale)
                        selected = j;

                    GameObject buttonObj = Instantiate(languageButtonPrefab);
                    buttonObj.GetComponent<Button>().onClick.AddListener(() => { SelectLocale(j); });
                    RawImage img = buttonObj.GetGameObjectComponent<RawImage>("Flag");
                    Texture2D texture = Resources.Load<Texture2D>(RESOURCE_FLAG_PATH + locale.Identifier.Code);

                    img.texture = texture;
                    var heightMul = img.rectTransform.sizeDelta.y;
                    var aspect = (float)texture.width / texture.height;

                    img.rectTransform.sizeDelta = new Vector2(aspect * heightMul, heightMul);

                    buttonObj.GetGameObjectComponent<TMP_Text>("Text").text = locale.Identifier.CultureInfo.NativeName;

                    buttonObj.transform.SetParent(languageContent);

                    languageButtons.Add(buttonObj.GetComponent<Button>());
                }

                Navigation languageButtonNavigation = new Navigation();
                languageButtonNavigation.mode = Navigation.Mode.Explicit;
                languageButtonNavigation.selectOnLeft = controlsButton;
                languageButtonNavigation.selectOnRight = generalButton;
                languageButtonNavigation.selectOnDown = languageButtons[0];
                languageButton.navigation = languageButtonNavigation;
            }
        }

        public void SelectLastLocaleButton() => SelectWhenMouseInactive(languageButtons[selected]);

        [HideInInspector]
        public bool languageSelectionActive = false;

        public void SelectLocale(int index)
        {
            if (languageSelectionActive)
                return;
            StartCoroutine(SetLocale());

            IEnumerator SetLocale()
            {
                languageSelectionActive = true;
                foreach (var button in languageButtons)
                    button.gameObject.SetActive(false);

                yield return LocalizationSettings.InitializationOperation;
                ChangeLocale(index);

                foreach (var button in languageButtons)
                    button.gameObject.SetActive(true);
                languageSelectionActive = false;
            }
        }

        #endregion

        public void SetOnSelectOnButtonsLayout(bool enable)
        {
            foreach (var button in categoryButtonsLayout.GetGameObjectChildren())
                button.GetComponent<EventTrigger>().enabled = enable;
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (background != null)
                background.enabled = open;

            subPanel.gameObject.SetActive(open);

            if (open)
            {
                generalButton.Select();
                OpenSubPanel(generalSubPanel);
            }
        }

        public void OpenSubPanel(RectTransform subPanel)
        {
            CloseAllSubPanels();
            subPanel.gameObject.SetActive(true);
        }

        public void CloseAllSubPanels()
        {
            generalSubPanel.gameObject.SetActive(false);
            soundSubPanel.gameObject.SetActive(false);
            controlsSubPanel.gameObject.SetActive(false);
            graphicsSubPanel.gameObject.SetActive(false);
            languageSubPanel.gameObject.SetActive(false);
        }
    }
}
