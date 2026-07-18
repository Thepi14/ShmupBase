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
        protected float shield = 0f;
        public virtual float Shield { get => shield; set { shield = value >= 0 ? value : 0; } }

        [SerializeField]
        protected bool alive = true;
        public virtual bool Alive { get => alive; set => alive = value; }

        [SerializeField]
        protected bool immune = false;
        public virtual bool Immune { get => immune; set => immune = value; }

        public float startImmunityTime = 0f;
        protected float startImmunityTimer = 0f;

        [HideInInspector]
        public SpriteRenderer spriteRenderer;
        [HideInInspector]
        public new Collider2D collider;

        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
            immune = true;
        }

        protected override void Update()
        {
            base.Update();

            if (spriteRenderer != null)
            {
                if (immune)
                {
                    spriteRenderer.color = Color.yellow;
                }
                else if (shield > 0)
                {
                    spriteRenderer.color = Color.cyan;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            startImmunityTimer -= Time.fixedDeltaTime;

            if (startImmunityTimer < 0f)
            {
                immune = false;
            }

            if (Health <= 0 && alive)
            {
                Kill();
            }
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
