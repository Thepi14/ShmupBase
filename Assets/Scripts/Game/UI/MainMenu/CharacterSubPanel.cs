using EditorTools;
using Main.EntitySystem;
using Main.ReplaySystem;
using Main.Stages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Main.UI
{
    public sealed class CharacterSubPanel : MonoBehaviour, IMoveHandler, ISubmitHandler
    {
        public GameObject characterPrefab;
        [ShowOnly]
        public byte characterID;

        [SerializeField]
        private LocalizeStringEvent characterName;
        [SerializeField]
        private LocalizeStringEvent characterDescription;
        [SerializeField]
        private Image characterPortrait;
        [SerializeField]
        private Button selectCharacterButton;

        private void Awake()
        {
            GetComponent<Selectable>().navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = CharactersPanel.Instance.exitButton
            };

            selectCharacterButton.onClick.AddListener(() =>
            {
                StartGame();
            });
        }

        private void OnEnable()
        {
            selectCharacterButton.gameObject.SetActive(Vars.UseMouse);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject)
                return;

            if (eventData.moveDir == MoveDirection.Left)
            {
                CharactersPanel.SelectLeft();
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                CharactersPanel.SelectRight();
            }
            else if (eventData.moveDir == MoveDirection.Down)
            {
                CharactersPanel.Instance.exitButton.SelectIfMouseInactive();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            StartGame();
        }

        public void SetupCharacter(PlayerEntity character, byte ID)
        {
            characterPrefab = character.gameObject;
            characterName.StringReference = character.characterName;
            characterDescription.StringReference = character.characterDescription;
            characterPortrait.sprite = character.characterPortrait;
            characterID = ID;
        }

        public void StartGame()
        {
            ReplayManagement.replayMode = false;
            Vars.LastDifficulty = StageManager.currentDifficulty;
            Vars.LastCharacterID = PlayerEntity.selectedCharacterID = characterID;
            StageManager.LoadStageScene(Vars.LastDifficulty);
        }
    }
}
