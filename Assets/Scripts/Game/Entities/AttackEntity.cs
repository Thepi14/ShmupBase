using System.Collections;
using UnityEngine;

namespace Main.EntitySystem
{
    public class AttackEntity : MoveEntity, IAttack
    {
        [SerializeField]
        protected bool canAttack;
        public virtual bool CanAttack { get => canAttack; set => canAttack = value; }

        [SerializeField]
        protected float reloadSpeedMultiplier = 1f;
        public virtual float ReloadSpeedMultiplier { get => reloadSpeedMultiplier; set => reloadSpeedMultiplier = value; }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public virtual void Attack()
        {
            StartCoroutine(AttackCoroutine(gameObject));
        }

        public virtual IEnumerator AttackCoroutine(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }
    }
}
