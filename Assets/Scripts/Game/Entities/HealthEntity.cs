using System.Collections;
using UnityEngine;

namespace Main.EntitySystem
{
    public class HealthEntity : Entity, IHealth
    {
        [SerializeField]
        protected float health = 5f;
        public virtual float Health { get => health; set => health = value; }
        [SerializeField]
        protected bool alive = true;
        public virtual bool Alive { get => alive; set => alive = value; }

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
            if (Health <= 0)
            {
                Kill();
            }
        }

        public virtual void Damage(float damage)
        {
            Health -= damage;
        }

        public virtual void Kill()
        {
            StartCoroutine(KillCoroutine());
        }

        public virtual IEnumerator KillCoroutine()
        {
            alive = false;
            yield return new WaitForFixedUpdate();
            Destroy(gameObject);
        }
    }
}
