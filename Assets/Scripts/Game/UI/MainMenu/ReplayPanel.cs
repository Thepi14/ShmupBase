using System.Collections.Generic;
using Main.ReplaySystem;
using Main.UI;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class ReplayPanel : GenericPanelBehaviour
    {
        [SerializeField]
        public Button exitButton;

        [Header("Replays Settings")]
        [SerializeField]
        private GameObject replayButtonPrefab;
        [SerializeField]
        private RectTransform replayContent;
        [SerializeField]
        private List<Button> replayButtons;

        private bool setup = false;

        protected override void Awake()
        {
            base.Awake();

            if (exitButton != null)
                exitButton.onClick.AddListener(() => ReturnToMain());
        }

        public void SetupReplaysList()
        {
            if (setup)
                return;
            setup = true;

            ReplayManagement.LoadAllReplayFiles();
            for (int i = 0; i < ReplayManagement.replays.Count; i++)
            {
                GameObject buttonObj = Instantiate(replayButtonPrefab);
                buttonObj.transform.SetParent(replayContent);
                buttonObj.GetComponent<ReplayButton>().SetupButton(i);
                replayButtons.Add(buttonObj.GetComponent<Button>());
            }
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            SetupReplaysList();
            if (open)
            {
                replayButtons[0].SelectIfMouseInactive();
            }
        }
    }
}
