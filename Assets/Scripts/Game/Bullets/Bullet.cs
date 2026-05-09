using System;
using System.Collections;
using UnityEngine;

namespace Main.BulletSystem
{
    public abstract class Bullet : MonoBehaviour
    {
        public float lifetime = 5f;
        public float speed = 1f;
        public float damage = 1f;

        public bool killByLifetime = false;
        [HideInInspector]
        public bool dead = false;
        protected float currentLifetime = 0f;

        public Coroutine customCoroutineInstance;
        public CustomCoroutine customCoroutine;

        public virtual void Set(Vector2 position, float rotation)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(0f, 0, rotation);
        }

        protected virtual void Start()
        {
            BulletManager.AddBullet(this);
            if (customCoroutine != null)
                customCoroutineInstance = StartCoroutine(customCoroutine(gameObject));
        }

        protected virtual void FixedUpdate()
        {
            VerifyKill();
        }

        protected virtual void VerifyKill()
        {
            currentLifetime += Time.fixedDeltaTime;
            if (currentLifetime > lifetime && killByLifetime)
            {
                Kill();
            }
        }

        public virtual void Kill()
        {
            dead = true;
            Destroy(gameObject);
        }
    }

    public interface IGravityBullet
    {
        public abstract void HandleGravity();
        public abstract void AddGravityForce(Vector2 force);
    }
}
