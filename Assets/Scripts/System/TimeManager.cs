using System;
using System.Collections;
using UnityEngine;

namespace Main
{
    public static class TimeManager
    {
        public const ushort FIXED_UPDATES_PER_SECOND = 60;
        public const float FIXED_TIMESTEP = 0.0167f, MAXIMUM_ALLOWED_TIMESTEP = 0.0167f;

        public static float timeScale = 1f;
        public static bool GameIsPaused { get; private set; } = false;

        public static void Start()
        {
            Pause(false);
        }

        public static void SetTimeScale(float scale)
        {
            scale = Mathf.Clamp01(scale);
            timeScale = scale;
            Time.timeScale = scale;
            Time.fixedDeltaTime = FIXED_TIMESTEP * Time.timeScale;
            Time.maximumDeltaTime = MAXIMUM_ALLOWED_TIMESTEP * Time.timeScale;
        }

        public static void Pause(bool pause)
        {
            Time.timeScale = pause ? 0f : timeScale;
            GameIsPaused = pause;
            Time.fixedDeltaTime = FIXED_TIMESTEP * Time.timeScale;
            Time.maximumDeltaTime = MAXIMUM_ALLOWED_TIMESTEP * Time.timeScale;

            /*GC.Collect();
            GC.WaitForPendingFinalizers();*/
        }

        public static void TickPause()
        {
            Pause(!GameIsPaused);
        }

        public static IEnumerator WaitFixedFrames(int frames)
        {
            for (int i = 0; i < frames; i += GameIsPaused ? 0 : 1)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
