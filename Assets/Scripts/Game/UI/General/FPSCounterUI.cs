using System.Collections;
using System.Collections.Generic;
using System.Security;
using EditorTools;
using ObjectUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

namespace Main.UI
{
    public sealed class FPSCounterUI : MonoBehaviour
    {
        private TMP_Text fpsText;
        [SerializeField]
        private LocalizedString fpsString;
        private float deltaTime;
        private float fps = 0f;
        public bool writeAsInt = false;
        [ShowOnly]
        public string fpsValueText;

        private void Awake()
        {
            fpsText = GetComponent<TMP_Text>();
            fpsString.Arguments = new object[] { writeAsInt ? "00" : "0.00" };
            fpsString.StringChanged += FormatFPSText;
            fpsText.enabled = Vars.ShowFPS;
        }

        private void LateUpdate()
        {
            fpsText.enabled = Vars.ShowFPS;
            if (!Vars.ShowFPS)
                return;

            var eventSystemRaysastResults = UIGeneral.GetEventSystemRaycastResults();
            string info = "";

            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                info += curRaysastResult.gameObject.name;
            }
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            if (Vars.ShowFPS && !TimeManager.GameIsPaused)
            {
                fps = 1.0f / deltaTime;

                if (writeAsInt)
                    fpsString.Arguments[0] = ((int)fps).ToString();
                else
                    fpsString.Arguments[0] = fps.ToString("0.00");

                fpsString.RefreshString();
            }
        }

        public void FormatFPSText(string text)
        {
            fpsText.text = text;
        }
    }
}