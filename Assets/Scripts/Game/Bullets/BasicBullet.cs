using Main.EntitySystem;
using UnityEngine;

namespace Main.BulletSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class BasicBullet : Bullet
    {
        [HideInInspector]
        public new Collider2D collider;
        [HideInInspector]
        public SpriteRenderer spriteRenderer;

        public float acceleration = 0f;
        public float angularVelocity = 0f;

        protected float currentAcceleration = 0f;

        protected virtual void OnValidate()
        {
            collider = GetComponent<Collider2D>();
            collider.isTrigger = true;

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Bullet";
            }
        }

        protected override void Awake()
        {
            base.Awake();

            collider = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider.isTrigger = true;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdatePosition();
        }

        public virtual void UpdatePosition()
        {
            transform.position += transform.right * GetCurrentSpeedFixed();
            currentAcceleration += acceleration * Time.fixedDeltaTime;

            transform.rotation *= Quaternion.Euler(0f, 0f, angularVelocity * Time.fixedDeltaTime);
        }

        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            /*Debug.Log("hit");
            if (collision == null)
                return;*/
            if (collision.GetComponent<IHealth>() is var iHealth && iHealth != null)
            {
                iHealth.Damage(damage);
            }
        }

        public override float GetCurrentSpeed() => speed + currentAcceleration;

        public override Bounds GetBounds()
        { 
            var bounds = collider.bounds;
            bounds.size *= boundsMultiplier;
            return bounds;
        }

        public override bool InsideGameCanvas() => GameManager.bounds.Intersects(GetBounds());
    }
}
