using System;
using System.Collections;
using System.Collections.Generic;
using EditorTools;
using Main.BulletSystem;
using Main.InputSystem;
using Main.ReplaySystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Main.EntitySystem
{
    public class PlayerEntity : AttackEntity
    {
        public const string playerPrefabsPath = "Prefabs/Entities/Players/";
        public static readonly Vector2 startPlayerPosition = new(0, -3);

        public static PlayerEntity playerEntity;
        public static byte playerTypeID = 0;
        public static GameObject[] playerEntityPrefabs;

        public override float Speed { get => speed * (input.slow ? slowSpeedMultiplier : 1f); set => speed = value; }

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

        public static UnityEvent PlayerLostLifeEvent = new(), PlayerLostAllLifesEvent = new();

        public virtual void OnValidate()
        {
            LoadPlayerList();
            Health = 1;
            if (!Application.isPlaying)
            {
                lifes = startingLifes;
            }
        }

        protected override void Awake()
        {
            LoadPlayerList();
            playerEntity = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, playerEntity);
            base.Awake();
            Health = 1;
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
            LoadPlayerList();
            playerEntity = Instantiate(playerEntityPrefabs[playerTypeID]).GetComponent<PlayerEntity>();
            playerEntity.transform.position = startPlayerPosition;

            playerEntity.diedEvent.AddListener(() =>
            {
                LoseLife();
            });

            return playerEntity;
        }

        public static void AddLifes(int amount = 1) => playerEntity.lifes = Mathf.Clamp(playerEntity.lifes + amount, 0, playerEntity.maxLifes);
        public static void SetLifes(int amount = 1) => playerEntity.lifes = Mathf.Clamp(amount, 0, playerEntity.maxLifes);

        public static void LoseLife(bool invokeEvents = true)
        {
            playerEntity.lifes = Mathf.Clamp(playerEntity.lifes - 1, 0, playerEntity.maxLifes);
            playerEntity.lostLifes++;

            if (playerEntity.lifes == 0)
            {
                if (invokeEvents)
                    PlayerLostAllLifesEvent.Invoke();
                //CompleteGame();
            }
            else if (invokeEvents)
                PlayerLostLifeEvent.Invoke();
        }

        public static bool PlayerLostLastLife() => playerEntity.lifes == 0;

        public static void LoadPlayerList() => playerEntityPrefabs = Resources.LoadAll<GameObject>(playerPrefabsPath);

        public static float AngleToPlayer(Vector2 position) => Mathf.Atan2(playerEntity.transform.position.y - position.y, playerEntity.transform.position.x - position.x) * Mathf.Rad2Deg;
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