using System.Collections;
using Main.BulletSystem;
using UnityEngine;

namespace Main.Stages
{
    public abstract class StageBehaviour : MonoBehaviour
    {
        public Coroutine stageCoroutine;
        public Coroutine backgroundCoroutine;

        public StageBehaviour nextStage;

        public byte stageIndex = 0;

        public int currentFrame = 0;
        public int currentFixedFrame = 0;

        public bool ended = false;

        protected virtual void Start()
        {
            var result = BulletManager.KillAllBulletsEnemy();
            Debug.Log("Stage started, result: " + result);
            stageCoroutine = StartCoroutine(StageCoroutine());
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
            Debug.Log("(Stage Coroutine invoked)");
        }

        protected virtual IEnumerator BackgroundCoroutine()
        {
            yield return null;
            Debug.Log("(Background Coroutine invoked)");
        }

        public IEnumerator WaitFrames(int frames)
        {
            int endFrame = currentFrame + frames;
            yield return new WaitUntil(() => currentFrame > endFrame);
        }

        protected virtual void EndStage()
        {
            ended = true;

            if (Vars.practiceMode)
            {
                GameManager.EndGame();
            }
            else if (nextStage == null)
            {
                GameManager.EndGame();
            }
            else
            {
                Destroy(gameObject);
                Instantiate(nextStage);
            }
        }
    }
}
