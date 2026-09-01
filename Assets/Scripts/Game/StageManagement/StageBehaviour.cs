using System;
using System.Collections;
using EditorTools;
using Main.BulletSystem;
using UnityEngine;

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

        public int currentFrame = 0;
        public int currentFixedFrame = 0;

        public bool finalStage = false;
        [ShowOnly]
        public bool ended = false;

        protected virtual void Start()
        {
            var result = BulletManager.KillAllBulletsEnemy();
            //Debug.Log("Stage started, result: " + result);
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
            //Debug.Log("(Stage Coroutine invoked)");
        }

        protected virtual IEnumerator BackgroundCoroutine()
        {
            yield return null;
            Debug.Log("(Background Coroutine invoked)");
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

            if (StageManager.currentGameMode == GameMode.Practice || finalStage || nextStage == null)
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
    }
}
