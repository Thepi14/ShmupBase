using System.Collections;
using Main.BulletSystem;
using Main.ReplaySystem;
using UnityEngine;
using static Main.InputSystem.InputManager;

namespace Main.EntitySystem
{
    public class PlayerEntity : AttackEntity
    {
        public override float Speed { get => speed * (slow ? slowSpeedMultiplier : 1f); set => speed = value; }
        public float slowSpeedMultiplier = 0.5f;

        [Header("Controls")]
        public Vector2 moveDirection;
        public bool attack;
        public bool bomb;
        public bool slow;

        public virtual void OnValidate()
        {
            Health = 1;
        }

        protected override void Awake()
        {
            base.Awake();
            Health = 1;
            EntityManager.Singleton.playerEntity = this;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateControls();

            if (GameManager.bounds.Contains((Vector2)transform.position + PredictMovement(moveDirection * Time.fixedDeltaTime, speed)))
                Move(moveDirection * Time.fixedDeltaTime);
        }

        protected virtual void UpdateControls()
        {
            if (GameManager.Singleton.gameEnded)
                return;

            if (ReplayManagement.replayMode)
            {
                var controls = GameManager.GetCurrentPlayerInput();
            }
            else
            {
                var controls = playerInput;
                moveDirection = controls.moveInput;
                attack = controls.attack;
                bomb = controls.bomb;
                slow = controls.slow;
            }

            Debug.Log("Replay: " + ReplayManagement.replayMode + ", Controls: " + playerInput.ToString());
        }

        public override IEnumerator KillCoroutine()
        {
            GameManager.LoseLife();
            alive = false;
            immune = true;
            canMove = false;
            startImmunityTimer = 3f;

            if (!GameManager.Singleton.gameEnded)
            {
                yield return new WaitForFixedUpdate();

                BulletManager.DestroyAllBulletsEnemy();
                Health = 1;
                transform.position = GameManager.startPlayerPosition;
                alive = true;
                canMove = true;
            }

            yield break;
        }
    }
}
