using System.Collections;
using UnityEngine;

namespace Main.EntitySystem
{
    public class BasicEntity : AttackEntity
    {
        public float reload = 1f;
        protected float reloadTimer = 0f;

        public bool attacking = false;
        public float initialAttackDelay = 1f;
        protected float timeAlive = 0f;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            timeAlive += Time.fixedDeltaTime;
            if (timeAlive > initialAttackDelay && !attacking)
            {
                attacking = true;
                Attack();
            }
        }

        public override IEnumerator AttackCoroutine(GameObject gameObject)
        {
            yield return null;
        }
    }
}
