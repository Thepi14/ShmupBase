using System;
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

        [Header("Question Sub Panel")]
        [SerializeField]
        private RectTransform questionSubPanel;
        [SerializeField]
        private Button saveReplayButton;
        [SerializeField]
        private Button dontSaveReplayButton;

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
        //[SerializeField]
        //private char[] allAscii;
        [SerializeField]
        private Button keyboardBackspaceButton, keyboardSaveButton;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            dontSaveReplayButton.onClick.AddListener(() => { SetOpenPanel(false); SceneManager.LoadScene(0); });
            saveReplayButton.onClick.AddListener(() => { questionSubPanel.gameObject.SetActive(false); nameSelectionSubPanel.gameObject.SetActive(true);
                if (!Vars.UseIngameKeyboard)
                {
                    keyboardGrid.gameObject.SetActive(false);
                    replayNameInput.Select();
                }
                else
                {
                    replayNameInput.interactable = false;
                    replayNameInput.readOnly = true;
                    replayNameInput.shouldActivateOnSelect = false;
                    keyboardGrid.transform.GetChild(0).GetComponent<Button>().Select();
                }
            });

            byte startOffset = 33, asciiSize = 94;
            //allAscii = new char[94];

            for (int i = startOffset; i < asciiSize + startOffset; i++)
            {
                //allAscii[i - startOffset] = (char)i;
                var newChar = (char)i;
                var newCharButton = Instantiate(charButtonPrefab);
                newCharButton.transform.SetParent(keyboardGrid.transform);
                newCharButton.GetComponent<Button>().onClick.AddListener(() => { replayNameInput.text += newChar; });
                newCharButton.FindComponentInChild<TMP_Text>("CharText").text += newChar;
            }

            keyboardBackspaceButton.transform.SetAsLastSibling();
            keyboardSaveButton.transform.SetAsLastSibling();

            keyboardBackspaceButton.onClick.AddListener(() => { replayNameInput.text.Remove(replayNameInput.text.Length - 1); });
            keyboardSaveButton.onClick.AddListener(() => { SubmitName(replayNameInput.text); SceneManager.LoadScene(0); });

            replayNameInput.onSubmit.AddListener((string text) => { SubmitName(text); SceneManager.LoadScene(0); });
            //GetCompatibleFileNameForReplay("abc 123 <>:\"/\\|?* . .");

            GameManager.gameEndedEvent.AddListener(() => SetOpenPanel(true));
        }

        protected override void Start()
        {
            base.Start();

            SetOpenPanel(false);
            background.enabled = false;
        }

        public void SubmitName(string text)
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
            questionSubPanel.gameObject.SetActive(open);

            if (open)
            {
                saveReplayButton.Select();
                background.enabled = true;
            }
            else
            {
                nameSelectionSubPanel.gameObject.SetActive(open);
            }
        }
    }
}
