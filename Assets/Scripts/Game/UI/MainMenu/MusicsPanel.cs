using System;
using System.Collections.Generic;
using System.Linq;
using Main.Sound;
using ObjectUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Main.UI
{
    public class MusicsPanel : GenericPanelBehaviour
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

        private Button selectedMusicButton;

        protected override void Awake()
        {
            base.Awake();
            exitButton.onClick.AddListener(() => ReturnToMain());
            SetupMusicList();
        }

        public void SetupMusicList()
        {
            musicButtons = new List<Button>();

            Navigation musicButtonNavigation = new();
            musicButtonNavigation.mode = Navigation.Mode.Explicit;

            Button previousButton = null;

            foreach (var music in SoundManager.Singleton.musics)
            {
                var buttonObj = Instantiate(musicButtonPrefab).GetComponent<Button>();
                buttonObj.transform.SetParent(musicContent);
                buttonObj.GetGameObjectComponent<LocalizeStringEvent>("MusicNameText").StringReference = music.musicName;
                buttonObj.GetGameObjectComponent<TextMeshProUGUI>("DurationText").text = TimeSpan.FromSeconds(music.audioClip.length).ToString(@"mm\:ss");
                buttonObj.GetComponent<Outline>().enabled = false;

                buttonObj.onClick.AddListener(() => 
                {
                    SoundManager.PlayMusic(music);
                    description.StringReference = music.musicDescription;
                    buttonObj.GetComponent<Outline>().enabled = true;

                    selectedMusicButton.GetComponent<Outline>().enabled = false;
                    selectedMusicButton = buttonObj;
                });

                if (musicButtons.Count == 0)
                {
                    musicButtonNavigation.selectOnUp = exitButton;
                }
                else
                {
                    //previous section
                    musicButtonNavigation.selectOnDown = buttonObj;
                    previousButton.navigation = musicButtonNavigation;

                    //current section
                    musicButtonNavigation = new();
                    musicButtonNavigation.mode = Navigation.Mode.Explicit;

                    musicButtonNavigation.selectOnUp = previousButton;
                }

                musicButtons.Add(buttonObj);
                previousButton = buttonObj;
            }

            exitButton.navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = musicButtons.First(),
                selectOnUp = musicButtons.Last()
            };

            musicButtonNavigation.selectOnDown = musicButtons.First();
            previousButton.navigation = musicButtonNavigation;
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                musicButtons[0].SelectIfMouseInactive();
                description.StringReference = noDescriptionText;

                selectedMusicButton = musicButtons.First();
                selectedMusicButton.GetComponent<Outline>().enabled = true;
            }
        }
    }
}
