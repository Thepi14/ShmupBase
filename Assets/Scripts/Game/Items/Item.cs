using EditorTools;
using Main.EntitySystem;
using UnityEngine;

namespace Main.ItemSystem
{
    public abstract class Item : MonoBehaviour
    {
        public float fallingAccelerationScale = 1f;
        public Vector2 startingFallingVelocity = new(0f, 0.25f);
        public float maxFallingVelocity = -0.5f;

        [ShowOnly]
        [SerializeField]
        protected Vector2 currentFallingVelocity;

        public float approachVelocity = 1f;
        [ShowOnly]
        public float currentApproachVelocity = 0f;
        public float playerApproachRadius = 0.6f;

        public float itemRadius = 0.1f;

        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        protected virtual void Awake()
        {
            ItemManager.AddItem(this);

            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            currentFallingVelocity = startingFallingVelocity;
            currentApproachVelocity = approachVelocity;
        }

        protected virtual void FixedUpdate()
        {
            Move();

            if (CanBeCollectedByPlayer())
                Collect();
        }

        protected virtual void Move()
        {
            if (Vector2.Distance(transform.position, PlayerEntity.PlayerPosition()) < playerApproachRadius)
            {
                currentFallingVelocity = Vector2.SmoothDamp(transform.position, PlayerEntity.PlayerPosition(), ref currentFallingVelocity, currentApproachVelocity);

                transform.position = currentFallingVelocity;
            }
            else
            {
                currentFallingVelocity = Vector2.ClampMagnitude(currentFallingVelocity + (Physics2D.gravity * Time.fixedDeltaTime * fallingAccelerationScale), maxFallingVelocity);

                transform.position += (Vector3)currentFallingVelocity;
            }
        }

        protected virtual bool CanBeCollectedByPlayer()
        {
            return Vector2.Distance(transform.position, PlayerEntity.PlayerPosition()) < itemRadius;
        }

        protected abstract void Collect();

        protected virtual void OnDestroy()
        {
            ItemManager.RemoveItem(this);
        }
    }
}
