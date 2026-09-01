using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Main.EntitySystem
{
    public abstract class Entity : MonoBehaviour
    {
        public Coroutine customCoroutineInstance;
        public CustomCoroutine customCoroutine;
        [HideInInspector]
        public UnityEvent onEntityCreate = new();

        protected virtual void Start()
        {
            if (customCoroutine != null)
                customCoroutineInstance = StartCoroutine(customCoroutine(gameObject));
        }

        protected virtual void Awake()
        {
            EntityManager.AddEntity(this);

            /*{
                customCoroutine = Wait;
                IEnumerator Wait(GameObject gameObject)
                {
                    yield return new WaitForSeconds(1f);
                }
            }*/

            //func = Wait;
        }

        protected virtual void FixedUpdate()
        {

        }
    }

    public interface IHealth
    {
        public abstract float Health { get; set; }
        public abstract bool Alive { get; set; }
        public abstract bool Immune { get; set; }
        public abstract float Shield { get; set; }

        public abstract void Damage(float damage);

        public abstract void DamageShield(float damage);

        public abstract void Kill();
    }

    public interface IMove
    {
        public abstract float Speed { get; set; }
        public abstract bool CanMove { get; set; }
        public abstract void Move(Vector2 movement, float speed = -1f);
    }

    public interface IAttack
    {
        public abstract bool CanAttack { get; set; }
        public abstract float ReloadSpeedMultiplier { get; set; }
        public abstract void Attack();
        public abstract IEnumerator AttackCoroutine(GameObject gameObject);
    }
}
