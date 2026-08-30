using System;
using System.Collections.Generic;
using System.Linq;
using Main.ReplaySystem;
using ObjectUtils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class SaveReplayUI : PanelBehaviour
    {
        public static SaveReplayUI Instance;

        [SerializeField]
        private RectTransform title;

        [Header("Replay Name Sub Panel")]
        [SerializeField]
        private RectTransform nameSelectionSubPanel;
        [SerializeField]
        private TMP_InputField replayNameInput;

        [Header("Keyboard")]
        [SerializeField]
        private RectTransform keyboardGrid;
        [SerializeField]
        private GameObject charButtonPrefab;
        [SerializeField]
        private Button keyboardBackspaceButton, keyboardSaveButton;
        [SerializeField]
        private List<Button> keyButtons;
        [SerializeField]
        private Grid<Button> keyGrid;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            if (!Vars.UseIngameKeyboard)
            {
                keyboardGrid.gameObject.SetActive(false);
                replayNameInput.SelectIfMouseInactive();

                replayNameInput.onSubmit.AddListener((string text) => { SubmitAndSaveReplay(text); SceneManager.LoadScene(0); });
            }
            else
            {
                replayNameInput.interactable = false;
                replayNameInput.readOnly = true;
                replayNameInput.shouldActivateOnSelect = false;
                keyboardGrid.transform.GetChild(0).GetComponent<Button>().SelectIfMouseInactive();

                byte startOffset = 33, asciiSize = 94;
                keyButtons = new();

                for (int i = startOffset; i < asciiSize + startOffset; i++)
                {
                    var newChar = (char)i;
                    var newCharButton = Instantiate(charButtonPrefab);
                    newCharButton.transform.SetParent(keyboardGrid.transform);
                    newCharButton.GetComponent<Button>().onClick.AddListener(() => { replayNameInput.text += newChar; });
                    newCharButton.FindComponentInChild<TMP_Text>("CharText").text += newChar;

                    keyButtons.Add(newCharButton.GetComponent<Button>());
                }

                keyboardBackspaceButton.transform.SetAsLastSibling();
                keyboardSaveButton.transform.SetAsLastSibling();

                keyButtons.Add(keyboardBackspaceButton, keyboardSaveButton);

                GridLayoutGroup grid = keyboardGrid.GetComponent<GridLayoutGroup>();
                keyGrid.ListToGrid(keyButtons, grid.constraintCount);

                for (int y = 0; y < keyboardGrid.childCount / grid.constraintCount; y++)
                {
                    for (int x = 0; x < grid.constraintCount; x++)
                    {
                        keyGrid.Get(x, y).GetComponent<Button>().navigation = new()
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnLeft = x == 0 ? keyGrid.Get(keyGrid.GetWidth() - 1, y) : keyGrid.Get(x - 1, y),
                            selectOnRight = x == keyGrid.GetWidth() - 1 ? keyGrid.Get(0, y) : keyGrid.Get(x + 1, y),
                            selectOnUp = y == 0 ? keyGrid.Get(x, keyGrid.GetHeight() - 1) : keyGrid.Get(x, y - 1),
                            selectOnDown = y == keyGrid.GetHeight() - 1 ? keyGrid.Get(x, 0) : keyGrid.Get(x, y + 1),
                        };
                    }
                }

                keyboardBackspaceButton.onClick.AddListener(() => { replayNameInput.text.Remove(replayNameInput.text.Length - 1); });
                keyboardSaveButton.onClick.AddListener(() => { SubmitAndSaveReplay(replayNameInput.text); SceneManager.LoadScene(0); });
            }
        }

        protected override void Start()
        {
            base.Start();

            background.enabled = false;

            //TODO: put this piece of code in a more intuive place
            GameUI.Instance.SetOpenPanel(false);
        }

        public void SubmitAndSaveReplay(string text)
        {
            var gameManagerSingleton = GameManager.Singleton;

            if (gameManagerSingleton == null)
            {
                throw new NullReferenceException("Cant save a replay outside of the game scene!");
            }

            GameManager.SaveReplay(text);
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);
            title.gameObject.SetActive(open);
            nameSelectionSubPanel.gameObject.SetActive(open);

            if (open)
            {
                keyButtons[0].SelectIfMouseInactive();
                background.enabled = true;
            }
        }
    }
}
