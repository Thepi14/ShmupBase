using System.Collections.Generic;
using System.Linq;
using Main.EntitySystem;
using ObjectUtils;
using UnityEngine;

namespace Main.BulletSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineBullet : CurvedLaserBullet
    {
        /// <summary>
        /// Conecta o último ponto dado ao primeiro ponto.
        /// </summary>
        [Header("Line Settings")]
        public bool loop = false;
        /// <summary>
        /// How the positionValueFunction value will be used distribute the points.
        /// </summary>
        public PositionsGenerationMode generationMode = PositionsGenerationMode.MaxPositions;
        /// <summary>
        /// Define o filtro de distribuição para a curva quando generationMode = PositionsGenerationMode.Interval.
        /// </summary>
        public float minInterval = 0.05f;
        /// <summary>
        /// Precição do cálculo de subdivisões da curva quando generationMode = PositionsGenerationMode.DistributedMaxPositions.
        /// </summary>
        public int precision = 1000;
        /// <summary>
        /// If true change the position of points using the gameObject position.
        /// </summary>
        public bool includeLocalPositionInFunction = true;

        protected override bool Loop => loop;

        [HideInInspector]
        public PositionCurveFunction positionValueFunction = (lineBullet, value) => Vector2.zero;
        /// <summary>
        /// Delegado que retorna uma posição 2D de acordo com a função dada.
        /// <para>Se generationMode = Interval, a quantidade de pontos será pela distância entre os pontos.</para>
        /// <para>Se generationMode = MaxPositions, a quantidade de pontos será definido por maxPositions.</para>
        /// <para>Se generationMode = DefinedInEditor, não muda a quantidade definida pelo editor.</para>
        /// </summary>
        /// <param name="lineBullet">O monobehaviour que usará esse delegado.</param>
        /// <param name="value">Um valor customizado para a função que vai de 0 a 1.</param>
        /// <returns></returns>
        public delegate Vector2 PositionCurveFunction(LineBullet lineBullet, float value);


        protected override void OnValidate()
        {
            base.OnValidate();

            lineRenderer = GetComponent<LineRenderer>();
            if (minInterval > interval || minInterval <= 0.0005f)
            {
                minInterval = interval * 0.5f;
            }

            if (lineRenderer != null)
            {
                lineRenderer.loop = loop;
            }
        }

        protected override void Start()
        {
            base.Start();

            //TODO: a lot of examples (maybe not the cruciform one)

            //positionValueFunction = (lineBullet, value) => MathEx.CosSinDegPos(value * 360f, 2f);
            //positionValueFunction = (lineBullet, value) => MathEx.LemniscateDeg(2f, value * 360f);
            //positionValueFunction = (lineBullet, value) => MathEx.Cruciform(1f, 1f, value * 360f, 50f, 50f);
            //positionValueFunction = (lineBullet, value) => MathEx.Hypocycloid(3f, 1f, value);
        }

        public virtual Vector2 GetPosition(float value) => (positionValueFunction(this, value) * transform.localScale) + (includeLocalPositionInFunction ? transform.position : Vector2.zero);

        protected override void FixedUpdate()
        {
            VerifyKill();

            HandlePositions();
            UpdatePosition();

            HandleCollision();
        }

        protected override void HandlePositions()
        {
            if (generationMode != PositionsGenerationMode.DefinedInEditor)
                storedPositions.Clear();

            switch (generationMode)
            {
                case PositionsGenerationMode.MaxPositions:
                    for (int i = 0; i < maxPositions; i++)
                    {
                        storedPositions.Add(GetPosition(i / (float)maxPositions));
                    }
                    break;
                case PositionsGenerationMode.Interval:
                    {
                        Vector2 currentPosition = GetPosition(0f),
                                nextPosition = GetPosition(0f);

                        storedPositions.Add(GetPosition(0f));
                        for (float f = 0f; f <= 1f; f += minInterval)
                            {
                                nextPosition = GetPosition(f);
                                if (Vector2.Distance(currentPosition, nextPosition) > interval)
                                {
                                    currentPosition = nextPosition;
                                    storedPositions.Add(currentPosition);
                                }
                            }
                    }
                    break;
                case PositionsGenerationMode.DistributedMaxPositions:
                    {
                        Vector2 currentPosition = GetPosition(0f),
                                nextPosition = GetPosition(0f);

                        float curveLength = MathEx.CurveLength((value) => positionValueFunction(this, value), precision);
                        float distance = curveLength / maxPositions;

                        storedPositions.Add(GetPosition(0f));
                        for (float f = 0f; f <= 1f; f += minInterval)
                        {
                            nextPosition = GetPosition(f);
                            if (Vector2.Distance(currentPosition, nextPosition) > distance)
                            {
                                currentPosition = nextPosition;
                                storedPositions.Add(currentPosition);
                            }
                        }
                    }
                    break;
            }

            if (Loop && ((byte)generationMode < 3))
            {
                storedPositions.Add(GetPosition(0f));
            }
        }

        public enum PositionsGenerationMode : byte
        {
            MaxPositions,
            Interval,
            DistributedMaxPositions,
            DefinedInEditor
        }
    }
}
