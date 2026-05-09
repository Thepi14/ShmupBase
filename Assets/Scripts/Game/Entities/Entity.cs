using System;
using System.Collections;
using UnityEngine;

namespace Main.EntitySystem
{
    public abstract class Entity : MonoBehaviour
    {
        public Coroutine customCoroutineInstance;
        public CustomCoroutine customCoroutine;

        protected virtual void Start()
        {
            if (customCoroutine != null)
                customCoroutineInstance = StartCoroutine(customCoroutine(gameObject));
        }

        protected virtual void Awake()
        {
            /*{
                customCoroutine = Wait;
                IEnumerator Wait(GameObject gameObject)
                {
                    yield return new WaitForSeconds(1f);
                }
            }*/

            //func = Wait;
        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }
    }

    public interface IHealth
    {
        public float Health { get; set; }
        public bool Alive { get; set; }
        public abstract void Damage(float damage);
        public abstract void Kill();
    }

    public interface IMove
    {
        public float Speed { get; set; }
        public bool CanMove { get; set; }
        public abstract void Move(Vector2 movement, float speed = -1f);
    }

    public interface IAttack
    {
        public bool CanAttack { get; set; }
        public float ReloadSpeedMultiplier { get; set; }
        public abstract void Attack();
        public abstract IEnumerator AttackCoroutine(GameObject gameObject);
    }
}
