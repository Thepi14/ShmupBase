using System.Collections;
using UnityEngine;
using ObjectUtils;
using UnityEngine.Events;
using EditorTools;

namespace Main.EntitySystem
{
    public class MoveEntity : HealthEntity, IMove
    {
        [SerializeField]
        protected float speed = 1f;
        public virtual float Speed { get => speed; set => speed = value; }

        [SerializeField]
        protected bool canMove = true;
        public virtual bool CanMove { get => canMove; set => canMove = value; }

        protected new Rigidbody2D rigidbody;

        [HideInInspector]
        public UnityEvent onMove;

        [ShowOnly]
        public Vector2 previousPosition;

        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody2D>();
            SetPreviousPosition();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected void SetPreviousPosition()
        {
            previousPosition = transform.position;
        }

        public virtual void Move(Vector2 movement, float speed = -1f)
        {
            SetPreviousPosition();
            
            if (canMove)
            {
                if (speed < 0f)
                    speed = Speed;

                transform.position += (Vector3)PredictMovement(movement, speed);
                onMove.Invoke();
            }
        }

        public virtual Vector2 PredictMovement(Vector2 movement, float speed = -1f)
        {
            if (speed < 0f)
                speed = Speed;
            return movement * speed;
        }

        public virtual void MoveTo(Vector2 endPosition, float speed = -1f)
        {
            if (speed < 0f)
                speed = Speed;

            StartCoroutine(MoveToCoroutine());
            IEnumerator MoveToCoroutine()
            {
                while (Vector2.Distance((Vector2)transform.position, endPosition) > 0.01f)
                {
                    Move(MathEx.AngleVectors(endPosition, (Vector2)transform.position) * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                }
                transform.position = endPosition;

                yield return null;
            }
        }

        public virtual void MoveLerp(Vector2 endPosition, float speed = -1f)
        {
            if (speed < 0f)
                speed = Speed;

            StartCoroutine(MoveLerpCoroutine());
            IEnumerator MoveLerpCoroutine()
            {
                while ((Vector2)transform.position != endPosition)
                {
                    transform.position = MathEx.LerpFixedDelta((Vector2)transform.position, endPosition, speed);
                    yield return new WaitForFixedUpdate();
                }

                yield return null;
            }
        }
    }
}
