using System.Collections;
using Main.BulletSystem;
using UnityEngine;
using static Main.InputManager;

namespace Main.EntitySystem
{
    public class PlayerEntity : AttackEntity
    {
        public override float Speed { get => speed * (slow ? slowSpeedMultiplier : 1f); set => speed = value; }
        public float slowSpeedMultiplier = 0.5f;

        [Header("Controls")]
        public bool slow;
        public bool attack;
        public Vector2 moveDirection;

        protected override void Awake()
        {
            base.Awake();
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
            Move(moveDirection * Time.fixedDeltaTime);
        }

        protected virtual void UpdateControls()
        {
            slow = InputManagerInstance.slow;
            attack = InputManagerInstance.attack;
            moveDirection = InputManagerInstance.moveInput;
        }

        public override void Damage(float damage)
        {
            base.Damage(damage);
        }

        public override IEnumerator KillCoroutine()
        {
            alive = false;
            yield return new WaitForFixedUpdate();
            BulletManager.DestroyAllBulletsEnemy();
            transform.position = GameManager.startPlayerPosition;
        }
    }
}
