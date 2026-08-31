using System.Collections.Generic;
using Main.EntitySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public sealed class CharactersPanel : GenericPanelBehaviour
    {
        public static CharactersPanel instance;

        [SerializeField]
        private GameObject characterSubPanelPrefab;
        [Space(10f)]
        [SerializeField]
        private PanelBehaviour difficultyPanel;
        [SerializeField]
        private RectTransform characterSelectionLayout;
        [SerializeField]
        private Button leftButton, rightButton;

        [SerializeField]
        private List<CharacterSubPanel> characterSubPanels;

        public int currentPanelIndex;

        public Button exitButton;

        protected override void Awake()
        {
            base.Awake();

            instance = this;
            characterSubPanels = new List<CharacterSubPanel>();
            currentPanelIndex = (byte)Vars.LastCharacterID;

            GameObject[] charactersPrefabs = Resources.LoadAll<GameObject>(PlayerEntity.CHARACTERS_PREFABS_PATH);

            int i = 0;
            foreach (GameObject characterPrefab in charactersPrefabs)
            {
                var characterSubPanel = Instantiate(characterSubPanelPrefab).GetComponent<CharacterSubPanel>();
                byte j = (byte)i;
                characterSubPanel.SetupCharacter(characterPrefab.GetComponent<PlayerEntity>(), j);
                characterSubPanel.transform.SetParent(characterSelectionLayout);
                characterSubPanels.Add(characterSubPanel);
                i++;
            }

            exitButton.onClick.AddListener(() =>
            {
                difficultyPanel.SetOpenPanel(true);
            });

            leftButton.onClick.AddListener(() =>
            {
                SelectLeft();
            });

            rightButton.onClick.AddListener(() =>
            {
                SelectRight();
            });
        }

        public static void SelectLeft()
        {
            instance.currentPanelIndex--;
            instance.currentPanelIndex = instance.currentPanelIndex == -1 ? instance.characterSubPanels.Count - 1 : instance.currentPanelIndex;

            instance.SelectCharacterPanelByIndex();
        }

        public static void SelectRight()
        {
            instance.currentPanelIndex++;
            instance.currentPanelIndex = instance.currentPanelIndex == instance.characterSubPanels.Count ? 0 : instance.currentPanelIndex;

            instance.SelectCharacterPanelByIndex();
        }

        private void SelectCharacterPanelByIndex()
        {
            foreach (var characterPanel in instance.characterSubPanels)
            {
                characterPanel.gameObject.SetActive(false);
            }

            var selected = characterSubPanels[currentPanelIndex];
            selected.gameObject.SetActive(true);

            exitButton.navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = selected.GetComponent<Selectable>()
            };

            selected.GetComponent<Selectable>().SelectIfMouseInactive();
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                currentPanelIndex = (byte)Vars.LastCharacterID;
                SelectCharacterPanelByIndex();
            }
        }
    }
}
