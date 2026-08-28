using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class SettingsUI : GenericPanelBehaviour
    {
        [Header("Status")]
        [Space(20f)]
        public Selectable currentSectionButtonSelected;

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
        private Button generalCategoryButton;
        [SerializeField]
        private Button soundCategoryButton;
        [SerializeField]
        private Button graphicsCategoryButton;
        [SerializeField]
        private Button controlsCategoryButton;
        [SerializeField]
        private Button languageCategoryButton;

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
        [SerializeField]
        private Toggle saveAsJson;

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
                exitButton.onClick.AddListener(() => ReturnToMain());
            if (exitButtonDown != null)
                exitButtonDown.onClick.AddListener(() => ReturnToMain());

            //selection
            generalCategoryButton.onClick.AddListener(() => { OpenSubPanel(generalSubPanel); UpdateCategoryNavigation(generalCategoryButton); });
            soundCategoryButton.onClick.AddListener(() => { OpenSubPanel(soundSubPanel); UpdateCategoryNavigation(soundCategoryButton); });

            if (Application.isMobilePlatform)
                controlsCategoryButton.gameObject.SetActive(false);
            else
                controlsCategoryButton.onClick.AddListener(() => { OpenSubPanel(controlsSubPanel); UpdateCategoryNavigation(controlsCategoryButton); });

            graphicsCategoryButton.onClick.AddListener(() => { OpenSubPanel(graphicsSubPanel); UpdateCategoryNavigation(graphicsCategoryButton); });
            languageCategoryButton.onClick.AddListener(() => { OpenSubPanel(languageSubPanel); UpdateCategoryNavigation(languageCategoryButton); });

            SetOnSelectOnButtonsLayout(!UseMouse);

            //general
            toggleFPS.isOn = ShowFPS;
            saveAsJson.isOn = SaveReplaysAsJson;

            toggleFPS.onValueChanged.AddListener((value) => { ShowFPS = value; });
            saveAsJson.onValueChanged.AddListener((value) => { SaveReplaysAsJson = value; });

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
                button.onClick.AddListener(() => { QualitySettings.SetQualityLevel(j, true); SetAllCategoryNavigation(); });
                i++;
            }

            SetupScreenToggles();

            //controls
            if (!Application.isMobilePlatform)
                SetupControlSubPanel();

            //localization
            SetLocalizationButtons();

            //post configuration
            StartCoroutine(UpdateAllCategoryNavigationCoroutine());
        }

        IEnumerator UpdateAllCategoryNavigationCoroutine()
        {
            yield return LocalizationSettings.InitializationOperation;
            SetAllCategoryNavigation();
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

        public void SetupScreenToggles()
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

        public int GetLastQualityIndex()
        {
            return QualitySettings.GetQualityLevel();
        }

        #endregion

        #region Controls

        public void SetupControlSubPanel()
        {
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
        }

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

                Navigation languageButtonNavigation = new Navigation();
                languageButtonNavigation.mode = Navigation.Mode.Explicit;

                Button previousButton = null;

                // Generate list of available Locales
                selected = 0;
                for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
                {
                    var locale = LocalizationSettings.AvailableLocales.Locales[i];
                    int j = i;
                    if (LocalizationSettings.SelectedLocale == locale)
                        selected = j;

                    Button buttonObj = Instantiate(languageButtonPrefab).GetComponent<Button>();
                    buttonObj.onClick.AddListener(() => { SelectLocale(j); });
                    RawImage img = buttonObj.GetGameObjectComponent<RawImage>("Flag");
                    Texture2D texture = Resources.Load<Texture2D>(RESOURCE_FLAG_PATH + locale.Identifier.Code);

                    img.texture = texture;
                    var heightMul = img.rectTransform.sizeDelta.y;
                    var aspect = (float)texture.width / texture.height;

                    img.rectTransform.sizeDelta = new Vector2(aspect * heightMul, heightMul);

                    buttonObj.GetGameObjectComponent<TMP_Text>("Text").text = locale.Identifier.CultureInfo.NativeName;

                    buttonObj.transform.SetParent(languageContent);

                    if (languageButtons.Count == 0)
                    {
                        languageButtonNavigation.selectOnUp = languageCategoryButton;
                    }
                    else
                    {
                        //previous section
                        languageButtonNavigation.selectOnDown = buttonObj;
                        previousButton.navigation = languageButtonNavigation;

                        //current section
                        languageButtonNavigation = new();
                        languageButtonNavigation.mode = Navigation.Mode.Explicit;

                        languageButtonNavigation.selectOnUp = previousButton;
                    }

                    languageButtons.Add(buttonObj.GetComponent<Button>());
                    previousButton = buttonObj;
                }

                /*languageSectionButton.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnDown = languageButtons.First(),
                    selectOnUp = languageButtons.Last()
                };*/

                languageButtonNavigation.selectOnDown = languageButtons.First();
                previousButton.navigation = languageButtonNavigation;
            }
        }

        public Button GetLastLocaleButton() => languageButtons[selected];

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

        #region Categories

        public void SetOnSelectOnButtonsLayout(bool enable)
        {
            foreach (var button in categoryButtonsLayout.GetGameObjectChildren())
                button.GetComponent<EventTrigger>().enabled = enable;
        }

        public void UpdateCategoryNavigation(Button categoryButton)
        {
            currentSectionButtonSelected = categoryButton;

            Navigation exitNavigation = exitButton.navigation;

            exitNavigation.selectOnUp = currentSectionButtonSelected;
            exitNavigation.selectOnDown = currentSectionButtonSelected;
            exitNavigation.selectOnRight = currentSectionButtonSelected;
            exitNavigation.selectOnLeft = currentSectionButtonSelected;

            exitButton.navigation = exitNavigation;

            SetAllCategoryNavigation();

            if (InputManager.InputManagerInstance.mouseLocked)
                EventSystem.current.SetSelectedGameObject(GetFirstSelectable(categoryButton).gameObject);
        }

        private Dictionary<Button, Selectable> buttonFirstSelectablePairs;

        public void SetAllCategoryNavigation()
        {
            buttonFirstSelectablePairs = new();

            SetCategoryNavigation(generalCategoryButton, toggleFPS);
            SetCategoryNavigation(soundCategoryButton, masterVolume);
            SetCategoryNavigation(controlsCategoryButton, firstSelectableControlBinder);
            SetCategoryNavigation(graphicsCategoryButton, qualityButtonList[GetLastQualityIndex()]);
            SetCategoryNavigation(languageCategoryButton, GetLastLocaleButton());
        }

        public Selectable GetFirstSelectable(Button categoryButton)
        {
            return buttonFirstSelectablePairs[categoryButton];
        }

        public void SetCategoryNavigation(Button categoryButton, Selectable firstSelectable)
        {
            Navigation currentNavigation = categoryButton.navigation;

            Navigation newNavigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = currentNavigation.selectOnLeft,
                selectOnRight = currentNavigation.selectOnRight,
                selectOnUp = exitButton,
                selectOnDown = firstSelectable
            };

            categoryButton.navigation = newNavigation;

            buttonFirstSelectablePairs.Add(categoryButton, firstSelectable);
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                generalCategoryButton.SelectIfMouseInactive();
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

        #endregion
    }
}
