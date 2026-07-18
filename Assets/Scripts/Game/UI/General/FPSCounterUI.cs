using System.Collections;
using System.Collections.Generic;
using System.Security;
using ObjectUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Main
{
    public sealed class FPSCounterUI : MonoBehaviour
    {
        private TMP_Text fpsText;
        [SerializeField]
        private LocalizedString fpsString;
        private float deltaTime;
        private float fps = 0f;
        public bool writeAsInt = false;

        private void Awake()
        {
            fpsText = GetComponent<TMP_Text>();
            fpsString.Arguments = new object[] { writeAsInt ? "00" : "00,00" };
            fpsString.StringChanged += FormatFPSText;
        }

        private void LateUpdate()
        {
            fpsText.enabled = Vars.ShowFPS;
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            if (Vars.ShowFPS)
            {
                fps = 1.0f / deltaTime;
                var text = "0";

                if (writeAsInt)
                {
                    text = ((int)fps).ToString();
                }
                else
                {
                    text = fps.ToString()[..(text.Length < 5 ? text.Length - 1 : 5)];
                }

                fpsString.Arguments[0] = text;
                fpsString.RefreshString();

                /*if (fps.ToString().Length > 5)
                    fpsText.text = $"FPS: {fps.ToString().Remove(5)}";*/
            }
        }

        public void FormatFPSText(string text)
        {
            fpsText.text = text;
        }
    }
}