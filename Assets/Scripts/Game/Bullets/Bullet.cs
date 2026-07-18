using System;
using System.Collections;
using UnityEngine;

namespace Main.BulletSystem
{
    public abstract class Bullet : MonoBehaviour
    {
        [Header("Default Informations")]
        public float lifetime = 5f;
        public bool killByLifetime = false;
        public float speed = 1f;
        public float damage = 1f;

        [Header("Others")]
        public ExitScreenFunction destroyOnExitScreen = ExitScreenFunction.Destroy;
        public ClearMode clearMode = ClearMode.Clear;
        public float boundsMultiplier = 1.1f;

        [HideInInspector]
        public bool dead = false;
        [HideInInspector]
        public float currentLifetime = 0f;

        public Coroutine customCoroutineInstance;
        public CustomCoroutine customCoroutine;

        public virtual void Set(Vector2 position, float rotation)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(0f, 0, rotation);
        }

        protected virtual void Start()
        {
            if (customCoroutine != null)
                customCoroutineInstance = StartCoroutine(customCoroutine(gameObject));
        }

        protected virtual void Awake()
        {
            BulletManager.AddBullet(this);
        }

        protected virtual void FixedUpdate()
        {
            VerifyKill();
        }

        protected virtual void VerifyKill()
        {
            currentLifetime += Time.fixedDeltaTime;
            if ((currentLifetime > lifetime && killByLifetime) || (byte)destroyOnExitScreen < 2 && !InsideGameCanvas())
            {
                Kill();
            }
        }

        public virtual float LifeScale() => currentLifetime / lifetime;

        public virtual void Kill()
        {
            dead = true;
            Destroy(gameObject);
        }

        public virtual void OnDestroy()
        {
            BulletManager.RemoveBullet(this);
        }

        public virtual float GetCurrentSpeed() => speed;
        public virtual float GetCurrentSpeedFixed() => GetCurrentSpeed() * Time.fixedDeltaTime;

        public abstract Bounds GetBounds();

        protected virtual Collider2D[] GetAllCollidersInBounds()
        {
            Bounds bounds = GetBounds();
            return Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);
        }

        public abstract bool InsideGameCanvas();

        public enum ExitScreenFunction : byte
        {
            Destroy,
            Kill,
            Nothing
        }

        public enum ClearMode : byte
        {
            /// <summary>
            /// Destrói a bala em qualquer chamada padrão das funções "BulletManager.DestroyAllBullets()".
            /// </summary>
            Clear,
            /// <summary>
            /// Não destrói a bala em qualquer chamada padrão das funções "BulletManager.DestroyAllBullets()", mas é destruída quando "affectExceptions" é verdadeiro.
            /// </summary>
            Exception,
            /// <summary>
            /// Não destrói a bala em absolutamente nenhuma chamada, recomendado somente quando a bala não pode ser eliminadad de jeito nenhum.
            /// </summary>
            NeverClear
        }
    }

    public interface IGravityBullet
    {
        public abstract void HandleGravity();
        public abstract void AddGravityForce(Vector2 force);
    }
}
