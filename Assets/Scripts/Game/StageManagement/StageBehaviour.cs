using System;
using System.Collections;
using EditorTools;
using Main.BulletSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;

namespace Main.Stages
{
    public abstract class StageBehaviour : MonoBehaviour
    {
        public Coroutine stageCoroutine;
        public Coroutine backgroundCoroutine;

        /// <summary>
        /// Prefab that is instantiated when this one ends.
        /// </summary>
        public GameObject nextStage;
        /// <summary>
        /// Gives information about the previous stage, null if its first stage or practice stage. (do not write manually!)
        /// </summary>
        public StageBehaviour previousStage;

        [Space(10f)]
        public LocalizedString stageName;
        public LocalizedString stageDescription;
        [ShowOnly]
        public byte id = 1;

        [Space(10f)]
        public bool hasBoss = true;
        public bool finalStage = false;

        [Space(10f)]
        [ShowOnly]
        public bool ended = false;

        [Space(10f)]
        [ShowOnly]
        public int currentFrame = 0;
        [ShowOnly]
        public int currentFixedFrame = 0;

        public void OnValidate()
        {
            id = byte.Parse(gameObject.name.Filter(letters: false, whitespace: false, symbols: false, punctuation: false));
        }

        protected virtual void Start()
        {
            var result = BulletManager.KillAllBulletsEnemy();
            stageCoroutine = StartCoroutine(StageStartCoroutine());
            backgroundCoroutine = StartCoroutine(BackgroundCoroutine());
        }

        protected virtual void Update()
        {
            currentFrame++;
        }

        protected virtual void FixedUpdate()
        {
            currentFixedFrame++;
        }

        protected virtual IEnumerator StageCoroutine()
        {
            yield return null;
        }

        protected virtual IEnumerator BackgroundCoroutine()
        {
            yield return null;
        }

        [Obsolete("Don't really work i think, use TimeManager.WaitFixedFrames().")]
        public IEnumerator WaitFrames(int frames)
        {
            int endFrame = currentFrame + frames;
            yield return new WaitUntil(() => currentFrame > endFrame);
        }

        protected virtual IEnumerator StageStartCoroutine()
        {
            yield return StageCoroutine();

            EndStage();
        }

        protected virtual void EndStage()
        {
            ended = true;
            GameManager.onStageEnd.Invoke();

            if (StageManager.currentGameMode == GameMode.StagePractice || finalStage || nextStage == null)
            {
                GameManager.CompleteGame();

                if (finalStage && StageManager.currentGameMode == GameMode.MainGame && !GameManager.Continued())
                {
                    GameManager.StartEnding();
                }
            }
            else
            {
                var obj = Instantiate(nextStage);
                obj.GetComponent<StageBehaviour>().previousStage = this;
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }

        public byte GetID() => id = byte.Parse(gameObject.name.Filter(letters: false, whitespace: false, symbols: false, punctuation: false));
    }
}
