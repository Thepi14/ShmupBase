using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class SettingsPanel : GenericPanelBehaviour
    {
        public static SettingsPanel Instance { get; private set; }

        [Header("Status")]
        [Space(20f)]
        public Selectable currentCategoryButtonSelected;

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
        private Toggle FPSToggle;
        [SerializeField]
        private Toggle saveAsJsonToggle;
        [SerializeField]
        private Button resetToFactorySettingsButton;

        [SerializeField]
        private RectTransform resetSettingsOverlay;
        [SerializeField]
        private Button resetSettingsCancelationButton;
        [SerializeField]
        private Button resetSettingsConfirmationButton;

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
        private Toggle maximizeWindowToggle;
        //windows only
        [SerializeField]
        private Toggle fullScreenExclusiveToggle;

        [Header("Controls")]
        [Space(20f)]

        [SerializeField]
        private GameObject rebindPrefab;

        [SerializeField]
        private GridLayoutGroup controlRebindGridGroup;
        [SerializeField]
        private Grid<Selectable> controlRebindSelectablesGrid;

        [SerializeField]
        private Selectable _currentSelectedControlRebindSelectable;
        public static Selectable CurrentSelectedControlRebindSelectable
        { 
            get
            {
                return Instance._currentSelectedControlRebindSelectable;
            }
            set
            {
                Instance._currentSelectedControlRebindSelectable = value;
                Instance.UpdateSelectedRebinderSelectable();
            }
        }
        [SerializeField]
        private Selectable firstSelectableControlBinder;
        [SerializeField]
        private Selectable selectableBelowControlLayout;

        [SerializeField]
        private Toggle useMouseToggle;
        [SerializeField]
        private Toggle useIngameKeyboardToggle;

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

            Instance = this;

            //exit
            if (exitButton != null)
                exitButton.onClick.AddListener(() => ReturnToMain());
            if (exitButtonDown != null)
                exitButtonDown.onClick.AddListener(() => ReturnToMain());

            //selection
            generalCategoryButton.onClick.AddListener(() => { OpenSubPanel(generalSubPanel); UpdateCategoryNavigation(generalCategoryButton); SelectFirstSelectable(generalCategoryButton); });
            soundCategoryButton.onClick.AddListener(() => { OpenSubPanel(soundSubPanel); UpdateCategoryNavigation(soundCategoryButton); SelectFirstSelectable(soundCategoryButton); });

            if (Application.isMobilePlatform)
                controlsCategoryButton.gameObject.SetActive(false);
            else
                controlsCategoryButton.onClick.AddListener(() => { OpenSubPanel(controlsSubPanel); UpdateCategoryNavigation(controlsCategoryButton); SelectFirstSelectable(controlsCategoryButton); });

            graphicsCategoryButton.onClick.AddListener(() => { OpenSubPanel(graphicsSubPanel); UpdateCategoryNavigation(graphicsCategoryButton); SelectFirstSelectable(graphicsCategoryButton); });
            languageCategoryButton.onClick.AddListener(() => { OpenSubPanel(languageSubPanel); UpdateCategoryNavigation(languageCategoryButton); SelectFirstSelectable(languageCategoryButton); });

            SetOnSelectOnButtonsLayout(!UseMouse);

            //general
            FPSToggle.isOn = ShowFPS;
            saveAsJsonToggle.isOn = SaveReplaysAsJson;

            FPSToggle.onValueChanged.AddListener((value) => ShowFPS = value);
            saveAsJsonToggle.onValueChanged.AddListener((value) => SaveReplaysAsJson = value);
            resetToFactorySettingsButton.onClick.AddListener(() => { resetSettingsOverlay.gameObject.SetActive(true); resetSettingsCancelationButton.SelectIfMouseInactive(); });

            resetSettingsCancelationButton.onClick.AddListener(() => { resetToFactorySettingsButton.SelectIfMouseInactive(); resetSettingsOverlay.gameObject.SetActive(false); });
            resetSettingsConfirmationButton.onClick.AddListener(() => { resetToFactorySettingsButton.SelectIfMouseInactive(); ResetAllPrefs(); resetSettingsOverlay.gameObject.SetActive(false); Application.Quit(); });

            resetSettingsOverlay.gameObject.SetActive(false);

            //sound
            masterVolume.value = MasterVolume;
            musicVolume.value = MusicVolume;
            soundEffectVolume.value = SoundEffectVolume;
            UIVolume.value = Vars.UIVolume;

            masterVolume.onValueChanged.AddListener((value) => SetKeySoundVolume(PrefKey.MasterVolume, value));
            musicVolume.onValueChanged.AddListener((value) => SetKeySoundVolume(PrefKey.MusicVolume, value));
            soundEffectVolume.onValueChanged.AddListener((value) => SetKeySoundVolume(PrefKey.SoundEffectVolume, value));
            UIVolume.onValueChanged.AddListener((value) => SetKeySoundVolume(PrefKey.UIVolume, value));

            resetVolumeButton.onClick.AddListener(() =>
            {
                ResetSoundPrefs();

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
                button.onClick.AddListener(() => { QualitySettings.SetQualityLevel(j, true); SetAllCategoryNavigation(); UpdateSelectedGraphicsQuality(); });
                i++;
            }

            SetupScreenToggles();
            UpdateSelectedGraphicsQuality();

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

        public void SelectSoundCategoryButton()
        {
            EventSystem.current.SetSelectedGameObject(soundCategoryButton.gameObject);
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
                maximizeWindowToggle.SetIsOnWithoutNotify(ScreenMode == FullScreenMode.MaximizedWindow);

                fullScreenWindowToggle.onValueChanged.AddListener((value) => { if (value) { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; ScreenMode = FullScreenMode.FullScreenWindow; } });
                maximizeWindowToggle.onValueChanged.AddListener((value) => { if (value) { Screen.fullScreenMode = FullScreenMode.MaximizedWindow; ScreenMode = FullScreenMode.MaximizedWindow; } });

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

        public void UpdateSelectedGraphicsQuality()
        {
            foreach (var button in qualityButtonList)
                button.GetComponent<Outline>().enabled = false;

            qualityButtonList[GetLastQualityIndex()].GetComponent<Outline>().enabled = true;

            UpdateQualitySelectionNavigation();
        }

        public void UpdateQualitySelectionNavigation()
        {
            Selectable current = qualityButtonList[GetLastQualityIndex()];

            Navigation oldNavigation = graphicsCategoryButton.navigation,
                newNavigation = new()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = oldNavigation.selectOnUp,
                    selectOnLeft = oldNavigation.selectOnLeft,
                    selectOnRight = oldNavigation.selectOnRight,
                    selectOnDown = current
                };
            graphicsCategoryButton.navigation = newNavigation;

            oldNavigation = fullScreenToggle.navigation;
            newNavigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = oldNavigation.selectOnLeft,
                selectOnRight = oldNavigation.selectOnRight,
                selectOnDown = oldNavigation.selectOnDown,
                selectOnUp = current
            };
            fullScreenToggle.navigation = newNavigation;

            oldNavigation = fullScreenWindowToggle.navigation;
            newNavigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = oldNavigation.selectOnLeft,
                selectOnRight = oldNavigation.selectOnRight,
                selectOnDown = oldNavigation.selectOnDown,
                selectOnUp = current
            };
            fullScreenWindowToggle.navigation = newNavigation;

            oldNavigation = maximizeWindowToggle.navigation;
            newNavigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = oldNavigation.selectOnLeft,
                selectOnRight = oldNavigation.selectOnRight,
                selectOnDown = oldNavigation.selectOnDown,
                selectOnUp = current
            };
            maximizeWindowToggle.navigation = newNavigation;
        }

        #endregion

        #region Controls

        public void SetupControlSubPanel()
        {
            useMouseToggle.isOn = UseMouse;
            useIngameKeyboardToggle.isOn = UseIngameKeyboard;

            useMouseToggle.onValueChanged.AddListener((value) => { UseMouse = value; SetOnSelectOnButtonsLayout(!value); if (!value) EventSystem.current.SetSelectedGameObject(useMouseToggle.gameObject); });
            useIngameKeyboardToggle.onValueChanged.AddListener((value) => UseIngameKeyboard = value);

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

            var selectableList = new List<Selectable>();
            int halfColumn = controlRebindGridGroup.transform.childCount / 2;

            for (int i = 0; i < halfColumn; i++)
            {
                foreach (GameObject selectableObj in controlRebindGridGroup.transform.GetChild(i).GetGameObjectChildren())
                {
                    if (selectableObj.GetComponent<Selectable>() != null)
                        selectableList.Add(selectableObj.GetComponent<Selectable>());
                }

                foreach (GameObject selectableObj in controlRebindGridGroup.transform.GetChild(i + halfColumn).GetGameObjectChildren())
                {
                    if (selectableObj.GetComponent<Selectable>() != null)
                        selectableList.Add(selectableObj.GetComponent<Selectable>());
                }
            }

            controlRebindSelectablesGrid = new Grid<Selectable>();
            controlRebindSelectablesGrid.ListToGrid(selectableList, 4);

            int width = controlRebindSelectablesGrid.GetWidth(), height = controlRebindSelectablesGrid.GetHeight();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Selectable selectable = controlRebindSelectablesGrid.Get(x, y);

                    Navigation newNavigation = new Navigation()
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnUp = y == 0 ? controlsCategoryButton : controlRebindSelectablesGrid.Get(x, y - 1),
                        selectOnDown = y == height - 1 ? selectableBelowControlLayout : controlRebindSelectablesGrid.Get(x, y + 1),
                        selectOnLeft = x == 0 ? controlRebindSelectablesGrid.Get(width - 1, y) : controlRebindSelectablesGrid.Get(x - 1, y),
                        selectOnRight = x == width - 1 ? controlRebindSelectablesGrid.Get(0, y) : controlRebindSelectablesGrid.Get(x + 1, y)
                    };

                    selectable.navigation = newNavigation;
                }
            }

            foreach (var control in controlRebindGridGroup.GetGameObjectChildren())
                rebinders.Add(control.GetComponent<RebindActionUI>());

            resetControlsButton.onClick.AddListener(() => ResetAllBinds());
        }

        public static void ResetAllBinds()
        {
            foreach (var rebinder in Instance.rebinders)
                rebinder.ResetToDefault();
        }

        private void UpdateSelectedRebinderSelectable()
        {
            Navigation oldNavigation = selectableBelowControlLayout.navigation,
                newNavigation = new()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnDown = oldNavigation.selectOnDown,
                    selectOnUp = _currentSelectedControlRebindSelectable
                };
            selectableBelowControlLayout.navigation = newNavigation;
            oldNavigation = controlsCategoryButton.navigation;

            newNavigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = oldNavigation.selectOnUp,
                selectOnLeft = oldNavigation.selectOnLeft,
                selectOnRight = oldNavigation.selectOnRight,
                selectOnDown = _currentSelectedControlRebindSelectable
            };
            controlsCategoryButton.navigation = newNavigation;
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
                    buttonObj.onClick.AddListener(() => SelectLocale(j));
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
        public static void ChangeLocale(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            SelectedLanguage = index;
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
            currentCategoryButtonSelected = categoryButton;

            exitButton.navigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = categoryButton,
                selectOnDown = categoryButton,
                selectOnRight = categoryButton,
                selectOnLeft = categoryButton
            };

            SetAllCategoryNavigation();

            foreach (var button in categoryButtonsLayout.GetGameObjectChildren())
            {
                button.GetComponent<Outline>().enabled = false;
            }
            categoryButton.GetComponent<Outline>().enabled = true;
        }

        private void SelectFirstSelectable(Button categoryButton)
        {
            if (InputManager.InputManagerInstance.mouseLocked)
                EventSystem.current.SetSelectedGameObject(GetFirstSelectable(categoryButton).gameObject);
        }

        public Selectable GetFirstSelectable(Button categoryButton)
        {
            return buttonFirstSelectablePairs[categoryButton];
        }

        private Dictionary<Button, Selectable> buttonFirstSelectablePairs;

        public void SetAllCategoryNavigation()
        {
            buttonFirstSelectablePairs = new();

            SetCategoryNavigation(generalCategoryButton, FPSToggle);
            SetCategoryNavigation(soundCategoryButton, masterVolume);
            SetCategoryNavigation(controlsCategoryButton, firstSelectableControlBinder);
            SetCategoryNavigation(graphicsCategoryButton, qualityButtonList[GetLastQualityIndex()]);
            SetCategoryNavigation(languageCategoryButton, GetLastLocaleButton());
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
