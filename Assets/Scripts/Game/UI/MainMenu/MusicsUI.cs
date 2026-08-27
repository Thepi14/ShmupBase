using System;
using System.Collections.Generic;
using Main.Sound;
using ObjectUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Main.UI
{
    public class MusicsUI : GenericPanelBehaviour
    {
        [SerializeField]
        private GameObject musicButtonPrefab;
        [SerializeField]
        private RectTransform musicContent;
        [SerializeField]
        private List<Button> musicButtons;
        [SerializeField]
        private LocalizeStringEvent description;
        [SerializeField]
        private LocalizedString noDescriptionText;
        [SerializeField]
        private Button exitButton;

        protected override void Awake()
        {
            base.Awake();
            exitButton.onClick.AddListener(() => ReturnToMain());
            SetupMusicList();
        }

        public void SetupMusicList()
        {
            musicButtons = new List<Button>();

            foreach (var music in SoundManager.Singleton.musics)
            {
                var buttonObj = Instantiate(musicButtonPrefab);
                buttonObj.transform.SetParent(musicContent);
                buttonObj.GetGameObjectComponent<LocalizeStringEvent>("MusicNameText").StringReference = music.musicName;
                buttonObj.GetGameObjectComponent<TextMeshProUGUI>("DurationText").text = TimeSpan.FromSeconds(music.audioClip.length).ToString(@"mm\:ss");

                buttonObj.GetComponent<Button>().onClick.AddListener(() => 
                {
                    SoundManager.PlayMusic(music);
                    description.StringReference = music.musicDescription;
                });

                musicButtons.Add(buttonObj.GetComponent<Button>());
            }
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                musicButtons[0].SelectIfMouseInactive();
                description.StringReference = noDescriptionText;
            }
        }
    }
}
