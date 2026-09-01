using System;
using System.Collections;
using System.Collections.Generic;
using EditorTools;
using Main.BulletSystem;
using Main.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace Main.EntitySystem
{
    public class PlayerEntity : AttackEntity
    {
        public const string CHARACTERS_PREFABS_PATH = "Prefabs/Entities/Players/";
        public static readonly Vector2 startPlayerPosition = new(0, -3);

        public static PlayerEntity PlayerInstance;
        public static GameObject[] playerEntityPrefabs;
        public static byte selectedCharacterID;

        public override float Speed { get => speed * (input.slow ? slowSpeedMultiplier : 1f); set => speed = value; }

        [Space(10f)]
        [Header("Character")]
        public LocalizedString characterName;
        public LocalizedString characterDescription;
        public Sprite characterPortrait;

        [Space(10f)]
        [Header("Player")]
        public int lifes = 3;
        public int startingLifes = 3;
        public int maxLifes = 10;
        [ShowOnly]
        public int lostLifes = 0;

        public float slowSpeedMultiplier = 0.5f;

        public List<CustomFloatValue> customValues = new();
        public PlayerInput input;

        public static UnityEvent onPlayerLoseLife = new(), oPlayerLostAllLifes = new();

        public virtual void OnValidate()
        {
            Health = 1;
            if (!Application.isPlaying)
            {
                lifes = startingLifes;
            }
        }

        protected override void Awake()
        {
            PlayerInstance = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, PlayerInstance);
            base.Awake();

            Health = 1;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateControls();

            if (GameManager.bounds.Contains((Vector2)transform.position + PredictMovement(input.moveInput * Time.fixedDeltaTime, speed)))
                Move(input.moveInput * Time.fixedDeltaTime);
        }

        protected virtual void UpdateControls()
        {
            /*
            if (GameManager.Singleton.gameEnded)
                return;
            */
            input = GameManager.GetCurrentPlayerInput();

            //Debug.Log("Replay: " + ReplayManagement.replayMode + ", Controls: " + playerInput.ToString());
        }

        public override void Kill()
        {
            base.Kill();
            immune = true;
            canMove = false;
            startImmunityTimer = 3f;
        }

        public override IEnumerator KillCoroutine()
        {
            /*if (!GameManager.Singleton.gameEnded)
            {*/
                yield return new WaitForFixedUpdate();

                BulletManager.DestroyAllBulletsEnemy();
                Health = 1;
                transform.position = startPlayerPosition;
                alive = true;
                canMove = true;
            //}

            yield break;
        }

        public static PlayerEntity GeneratePlayer()
        {
            LoadCharacterList();
            PlayerInstance = Instantiate(playerEntityPrefabs[selectedCharacterID]).GetComponent<PlayerEntity>();
            PlayerInstance.transform.position = startPlayerPosition;

            PlayerInstance.onDie.AddListener(() =>
            {
                LoseLife();
            });

            return PlayerInstance;
        }

        public List<string> arguments = new();

        public static void AddArgument(string argument)
        {
            PlayerInstance.arguments.Add(argument);
        }

        public static void AddLifes(int amount = 1) => PlayerInstance.lifes = Mathf.Clamp(PlayerInstance.lifes + amount, 0, PlayerInstance.maxLifes);
        public static void SetLifes(int amount = 1) => PlayerInstance.lifes = Mathf.Clamp(amount, 0, PlayerInstance.maxLifes);

        public static void LoseLife(bool invokeEvents = true)
        {
            PlayerInstance.lifes = Mathf.Clamp(PlayerInstance.lifes - 1, 0, PlayerInstance.maxLifes);
            PlayerInstance.lostLifes++;

            if (PlayerInstance.lifes == 0)
            {
                if (invokeEvents)
                    oPlayerLostAllLifes.Invoke();
                //CompleteGame();
            }
            else if (invokeEvents)
                onPlayerLoseLife.Invoke();
        }

        public static bool PlayerLostLastLife() => PlayerInstance.lifes == 0;

        public static void LoadCharacterList() => playerEntityPrefabs = Resources.LoadAll<GameObject>(CHARACTERS_PREFABS_PATH);

        public static Vector2 PlayerPosition() => PlayerInstance.transform.position;

        public static float AngleToPlayer(Vector2 position) => Mathf.Atan2(PlayerInstance.transform.position.y - position.y, PlayerInstance.transform.position.x - position.x) * Mathf.Rad2Deg;
    }
}

namespace Main
{
    [Serializable]
    public struct CustomFloatValue
    {
        public string name;
        public float value;
    }
}