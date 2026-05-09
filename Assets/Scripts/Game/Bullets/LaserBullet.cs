using UnityEngine;
using static ObjectUtils.GameObjectGeneral;

namespace Main.BulletSystem
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class LaserBullet : BasicBullet
    {
        protected CapsuleCollider2D Collider => collider as CapsuleCollider2D;
        /// <summary>
        /// <para>ActiveOnWidthCompleted = Ativa o collider só quando a largura completa.</para>
        /// <para>ActiveOnLengthCompleted = Ativa o collider só quando o comprimento completa.</para>
        /// <para>AlwaysActive = Força o collider a ficar ativo sempre.</para>
        /// <para>DontChange = Não muda o bool, recomendado para lasers com mecânicas mais complexas.</para>
        /// </summary>
        public ColliderActivationMode activationMode;

        [Header("Sprite")]
        public LaserSpriteDrawMode drawMode;
        public float spriteWidthScaleMultiplier = 1f;
        public float spriteLengthScaleMultiplier = 1f;

        [Header("Comprimento")]
        public float length = 5f;
        public float growLengthDelay = 0f;
        [Range(0f, 1f)]
        public float growLengthDelayThreshold = 1f;

        [Header("Largura")]
        public float width = 1f;
        public float growWidthDelay = 1f;
        [Range(0f, 1f)]
        public float growWidthDelayThreshold = 1f;

        protected bool completedWidthGrow = false;
        protected float growWidthDelayProgress = 0f;

        protected bool completedLengthGrow = false;
        protected float growLengthDelayProgress = 0f;

        protected virtual void OnValidate()
        {
            collider = GetComponent<CapsuleCollider2D>();
            Collider.direction = CapsuleDirection2D.Horizontal;

            CreateSpriteRenderer();
        }

        protected override void Start()
        {
            base.Start();

            Collider.direction = CapsuleDirection2D.Horizontal;
            Collider.enabled = false;
            Collider.offset = new Vector2(length * .5f, 0f);

            if (growLengthDelay <= 0f)
            {
                completedLengthGrow = true;
                growLengthDelayProgress = 1f;
            }

            if (growWidthDelay <= 0f)
            {
                completedWidthGrow = true;
                growWidthDelayProgress = 1f;
            }

            Collider.size = new Vector2(length, width * .5f);

            CreateSpriteRenderer();
        }

        protected virtual void CreateSpriteRenderer()
        {
            spriteRenderer = gameObject.GetGameObjectComponent<SpriteRenderer>("SpriteRenderer");
            if (spriteRenderer == null)
            {
                spriteRenderer = new GameObject("SpriteRenderer").AddComponent<SpriteRenderer>();
                spriteRenderer.transform.parent = transform;
                spriteRenderer.transform.localPosition = Vector3.zero;
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (growWidthDelayProgress < 1f)
                growWidthDelayProgress += Time.fixedDeltaTime / growWidthDelay;

            if (growLengthDelayProgress < 1f)
                growLengthDelayProgress += Time.fixedDeltaTime / growLengthDelay;

            UpdateSize();
            UpdateCollision();
            DrawLaserSprite();
        }

        public virtual void UpdateSize()
        {
            if (growWidthDelayProgress >= 1f)
            {
                growWidthDelayProgress = 1f;
            }
            if (growWidthDelayProgress >= growWidthDelayThreshold)
            {
                completedWidthGrow = true;
            }

            if (growLengthDelayProgress >= 1f)
            {
                growLengthDelayProgress = 1f;
            }
            if (growLengthDelayProgress >= growLengthDelayThreshold)
            {
                completedLengthGrow = true;
            }

            transform.localScale = new Vector3(growLengthDelayProgress, growWidthDelayProgress, 1f);
        }

        public virtual void UpdateCollision()
        {
            switch (activationMode)
            {
                case ColliderActivationMode.ActiveOnWidthCompleted:
                    Collider.enabled = completedWidthGrow;
                    break;
                case ColliderActivationMode.ActiveOnLengthCompleted:
                    Collider.enabled = completedLengthGrow;
                    break;
                case ColliderActivationMode.AlwaysActive:
                    Collider.enabled = true;
                    break;
            }
        }

        public virtual void DrawLaserSprite()
        {
            spriteRenderer.transform.localPosition = new Vector3(length * .5f, 0f, 0f);
            switch (drawMode)
            {
                case LaserSpriteDrawMode.Stretch:
                    spriteRenderer.drawMode = SpriteDrawMode.Simple;
                    spriteRenderer.transform.localScale = new Vector3(spriteLengthScaleMultiplier * length, spriteWidthScaleMultiplier, 1f);
                    break;
                case LaserSpriteDrawMode.Repeat:
                    spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                    spriteRenderer.tileMode = SpriteTileMode.Adaptive;
                    spriteRenderer.size = new Vector2(length, 1f);
                    spriteRenderer.transform.localScale = new Vector3(spriteLengthScaleMultiplier, spriteWidthScaleMultiplier, 1f);
                    break;
            }
        }

        public enum LaserSpriteDrawMode
        {
            /// <summary>
            /// Estica o sprite.
            /// </summary>
            Stretch,
            /// <summary>
            /// Repete o sprite usando o sistema adaptável, tente mudar o scaling de largura usando spriteWidthScaleMultiplier, pois o sistema adaptável pode chegar no limite e duplicar o sprite.
            /// </summary>
            Repeat
        }

        public enum ColliderActivationMode
        {
            /// <summary>
            /// Ativa o collider só quando a largura completa.
            /// </summary>
            ActiveOnWidthCompleted,
            /// <summary>
            /// Ativa o collider só quando o comprimento completa.
            /// </summary>
            ActiveOnLengthCompleted,
            /// <summary>
            /// Força o collider a ficar ativo sempre.
            /// </summary>
            AlwaysActive,
            /// <summary>
            /// Não muda o bool, recomendado para lasers com mecânicas mais complexas.
            /// </summary>
            DontChange
        }
    }
}
