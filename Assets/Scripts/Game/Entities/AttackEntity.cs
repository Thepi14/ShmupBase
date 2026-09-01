using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        [HideInInspector]
        public UnityEvent onAttack = new();

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
            onAttack.Invoke();
            StartCoroutine(AttackCoroutine(gameObject));
        }

        public virtual IEnumerator AttackCoroutine(GameObject gameObject)
        {
            Debug.LogWarning($"This {gameObject.name} entityBehaviour does not have a AttackCoroutine overrided and defined.");
            yield break;
        }
    }
}
