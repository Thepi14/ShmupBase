using System.Collections.Generic;
using System.Linq;
using Main.EntitySystem;
using ObjectUtils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Main.BulletSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class CurvedLaserBullet : BasicBullet
    {
        /// <summary>
        /// A distância entre um ponto e outro.
        /// </summary>
        [Header("Laser Settings")]
        public float interval = 0.1f;
        /// <summary>
        /// O máximo de posições que essa bala irá guardar.
        /// </summary>
        public int maxPositions = 10;
        public float width = 0.1f;
        public bool onlyCalculatePlayerCollision = true;

        [Header("Line Renderer Settings")]
        public float lineRendererWidthMultiplier = 1.1f;
        public float lineRendererWidthAdd = 0f;
        public LineRendererUseType lineRendererUseType = LineRendererUseType.ChangeLineRendererCurve;

        [HideInInspector]
        public LineRenderer lineRenderer;
        public List<Vector2> storedPositions = new List<Vector2>();

        [HideInInspector]
        public CurveFunction widthValueFunction = (curvedLaser, index) => MathEx.SinDeg(((index + 1) / (float)curvedLaser.storedPositions.Count) * 180f);
        public delegate float CurveFunction(CurvedLaserBullet curvedLaser, int index);

        protected virtual bool Loop => false;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (interval < 0.001f)
                interval = 0.001f;

            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.alignment = LineAlignment.TransformZ;
                lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lineRenderer.sortingLayerName = "Bullet";
            }
        }

        protected override void Awake()
        {
            base.Awake();
            storedPositions = new List<Vector2>() { transform.position, transform.position };
            lineRenderer = GetComponent<LineRenderer>();

            if (interval < 0.001f)
                interval = 0.001f;
            //collider.enabled = false;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected virtual void Update()
        {
            UpdateLineRenderer();
        }

        protected virtual void UpdateLineRenderer()
        {
            lineRenderer.positionCount = storedPositions.Count;

            //aberração
            var positions = new Vector3[storedPositions.Count];
            for (int i = 0; i < storedPositions.Count; i++)
                positions[i] = storedPositions[i];
            lineRenderer.SetPositions(positions);
            lineRenderer.loop = Loop;

            if (lineRendererUseType == LineRendererUseType.ChangeLineRendererCurve)
            {
                List<Keyframe> frames = new List<Keyframe>();
                for (int i = 0; i < (Loop ? storedPositions.Count - 1 : storedPositions.Count); i++)
                {
                    frames.Add(new Keyframe(i / (float)storedPositions.Count, (GetWidth(i) * lineRendererWidthMultiplier) + lineRendererWidthAdd) { weightedMode = WeightedMode.None });
                }

                lineRenderer.widthCurve = new(frames.ToArray());
            }
        }

        protected override void FixedUpdate()
        {
            VerifyKill();

            //Debug.Log(previousPositions[^1] + ", " + GetWidth(previousPositions.Count - 1));
            if (Vector2.Distance(storedPositions[^1], storedPositions[^2]) > interval)
            {
                HandlePositions();
                //Debug.Log("New point added, position: " + previousPositions[^1] + ", " + GetWidth(previousPositions.Count - 1));
            }

            UpdatePosition();
            storedPositions[^1] = (Vector2)transform.position;

            if (storedPositions.Count >= maxPositions)
            {
                Vector2 first = storedPositions[0], afterFirst = storedPositions[1];
                first += MathEx.AngleVectors(afterFirst, first) * GetCurrentSpeedFixed();

                storedPositions[0] = first;
            }

            HandleCollision();
        }

        protected virtual void HandlePositions()
        {
            storedPositions.Add(transform.position);

            if (storedPositions.Count > maxPositions)
            {
                storedPositions.RemoveAt(0);
            }
        }

        protected virtual void HandleCollision()
        {
            if (onlyCalculatePlayerCollision)
            {
                if (CollidesWithPlayer())
                {
                    PlayerEntity.PlayerInstance.Damage(damage);
                }
            }
            else
            {
                /*var collidedObjs = new List<GameObject>();

                for (int i = 0; i < storedPositions.Count - 1; i++)
                {
                    foreach (var collision in GetCollidersAtPoint(i))
                    {
                        if (!collidedObjs.Contains(collision.gameObject))
                            collidedObjs.Add(collision.gameObject);
                    }
                }*/

                var collidedObjs = CollideAll();

                foreach (var obj in collidedObjs)
                {
                    if (obj.GetComponent<IHealth>() is var iHealth && iHealth != null)
                    {
                        iHealth.Damage(damage);
                    }
                }
            }
        }

        protected virtual Collider2D[] CollideAll()
        {
            var referenceList = GetAllCollidersInBounds();
            var collidedObjs = referenceList.ToList();

            foreach (var collider in collidedObjs)
            {
                if (!IsWithinDistance(collider.transform.position))
                    collidedObjs.Remove(collider);
            }

            return collidedObjs.ToArray();
        }

        protected virtual Collider2D[] GetCollidersAtPoint(int index)
        {
            if (index >= storedPositions.Count - 1)
                return null;

            Vector2 position1 = storedPositions[index], position2 = storedPositions[index + 1];
            Collider2D[] collided =
                Physics2D.OverlapCapsuleAll
                    ((position1 + position2) * .5f, 
                    new(Vector2.Distance(position1, position2), 
                    Mathf.Max(GetWidth(index), GetWidth(index + 1))), 
                    CapsuleDirection2D.Horizontal,
                    MathEx.AngleDegrees(position1, position2), 
                    PhysicsCollisionMatrixLayerMasks.MaskForLayer(gameObject.layer));

            List<Collider2D> collidedCopy = collided.ToList();

            foreach (var col in collided)
            {
                Vector2 linePoint = GetNearestPointBetweenIndexAndNext(col.transform.position, index);
                if (Vector3.Distance((Vector2)col.transform.position, linePoint) > GetWidthBetweenIndexAndNext(linePoint, index))
                {
                    collidedCopy.Remove(col);
                }
            }

            return collidedCopy.ToArray();
        }

        public virtual Vector2 GetBorderPoint(Vector2 position, out float width)
        {
            var distance = float.MaxValue;
            width = 0f;
            var closestPoint = Vector2.zero;

            var closestIndex = 0;

            for (int i = 0; i < storedPositions.Count - 1; i++)
            {
                Vector2 linePoint = GetNearestPointBetweenIndexAndNext(position, i);
                var currentDistance = Vector3.Distance(position, linePoint);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    width = GetWidthBetweenIndexAndNext(linePoint, i);
                    closestPoint = linePoint;
                    closestIndex = i;
                }
            }

            return closestPoint;
        }

        public virtual bool IsWithinDistance(Vector2 position) => Vector2.Distance(GetBorderPoint(position, out float width), position) < width;
        public virtual bool IsWithinDistanceToPlayer()
        {
            var position = PlayerEntity.PlayerInstance.transform.position;
            return Vector2.Distance(GetBorderPoint(position, out float width), position) < width;
        }

        public virtual bool CollidesWith(Collider2D collider)
        {
            var point = GetBorderPoint(collider.transform.position, out float width);
            var closest = collider.ClosestPoint(point);
            return Vector2.Distance(point, closest) < width;
        }
        public virtual bool CollidesWithPlayer() => CollidesWith(PlayerEntity.PlayerInstance.collider);

        public virtual Vector2 GetBorderPointToPlayer(out float width) => GetBorderPoint(PlayerEntity.PlayerInstance.transform.position, out width);

        public virtual Vector2 GetClosestPositionFromPlayer()
        {
            var borderPointPosition = GetBorderPointToPlayer(out float width);
            return borderPointPosition + (MathEx.AngleVectors(PlayerEntity.PlayerInstance.transform.position, borderPointPosition) * width);
        }

        public virtual float GetWidth(int index) => (lineRendererUseType == LineRendererUseType.UseLineRendererCurve ? lineRenderer.widthCurve.Evaluate(index / (float)storedPositions.Count) : (widthValueFunction(this, index) * width)) * transform.localScale.AbsoluteDivided();
        public Vector3 GetNearestPointBetweenIndexAndNext(Vector3 position, int index) => MathEx.NearestPointOnFiniteLine(storedPositions[index], storedPositions[index + 1], position);
        public float GetWidthBetweenIndexAndNext(Vector3 point, int index) => Mathf.Lerp(GetWidth(index), GetWidth(index + 1), Vector2.Distance(storedPositions[index], point) / Vector2.Distance(storedPositions[index], storedPositions[index + 1])) * .5f;

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (storedPositions.Count > 1)
            {
                for (int i = 0; i < storedPositions.Count; i++)
                {
                    Gizmos.DrawWireSphere(storedPositions[i], GetWidth(i) * .5f);

                    if (i >= storedPositions.Count - 1)
                        continue;
                    Vector2 position1 = storedPositions[i], position2 = storedPositions[i + 1];

                    Gizmos.matrix = Matrix4x4.TRS((position1 + position2) * .5f, Quaternion.identity, Vector3.one) * Matrix4x4.Rotate(Quaternion.Euler(0, 0, MathEx.AngleDegrees(position1, position2) + 0.001f)); // o 0.001 é para evitar o erro ‘CompareApproximately(SqrMagnitude(result), 1.0F)’

                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(Vector2.Distance(position1, position2), Mathf.Max(GetWidth(i), GetWidth(i + 1)), 0f));
                    Gizmos.matrix = Matrix4x4.identity;
                }
                Gizmos.matrix = Matrix4x4.identity;

                var bounds = GetBounds();
                bounds.size = new Vector3(bounds.size.x, bounds.size.y, 1f);
                Gizmos.DrawWireCube(bounds.center, bounds.size);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(GetClosestPositionFromPlayer(), 0.05f);
            }
            Gizmos.matrix = Matrix4x4.identity;
        }

        public override Bounds GetBounds()
        {
            var f = GetSidesPoints(0);
            float rightX = f.x, leftX = f.z, 
                upperY = f.y, lowerY = f.w;

            for (int i = 1; i < storedPositions.Count; i++)
            {
                var sides = GetSidesPoints(i);
                // Right X
                if (sides.x > rightX)
                    rightX = sides.x;
                // Upper Y
                if (sides.y > upperY)
                    upperY = sides.y;

                // Left X
                if (sides.z < leftX)
                    leftX = sides.z;
                // Lower Y
                if (sides.w < lowerY)
                    lowerY = sides.w;
            }

            float width = Mathf.Abs(rightX - leftX),
                  height = Mathf.Abs(upperY - lowerY);

            Bounds bounds = new Bounds(new Vector2(leftX + (width * .5f), lowerY + (height * .5f)), new Vector3(width, height, float.PositiveInfinity) * boundsMultiplier);

            Vector4 GetSidesPoints(int index)
            {
                var point = storedPositions[index];
                var size = GetWidth(index) * .5f;
                                // rightX,         upperY,         leftX,         lowerY
                return new Vector4(point.x + size, point.y + size, point.x - size, point.y - size);
            }

            //Debug.Log("BX = " + Mathf.Abs(rightX) + ", BY = " + Mathf.Abs(upperY) + ", LX = " + Mathf.Abs(leftX) + ", LY = " + Mathf.Abs(lowerY) + ", Width = " + Mathf.Abs(width) + ", Height = " + Mathf.Abs(height));

            return bounds;
        }

        public override bool InsideGameCanvas()
        {
            bool inside = false;

            for (int i = 0; i < storedPositions.Count; i++)
            {
                if (GameManager.bounds.Intersects(new Bounds(storedPositions[i], Vector3.one * GetWidth(i) * boundsMultiplier)))
                    inside = true;
            }

            return inside;
        }

        public enum LineRendererUseType : byte
        {
            ChangeLineRendererCurve,
            UseLineRendererCurve,
            DontChangeLineRendererCurve
        }
    }
}
