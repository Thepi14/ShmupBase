using System.Collections;
using ObjectUtils;
using TMPro;
using UnityEngine;

namespace Main.UI
{
    /// <summary>
    /// Configurador para mensagens de aviso.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class WarningTextManager : MonoBehaviour
    {
        public static readonly Color defaultColor = Color.white;
        public static Color currentColor = defaultColor;

        /// <summary>
        /// Referência única do configurador para mensagens de aviso.
        /// </summary>
        public static WarningTextManager WarningTextManagerInstance { get; private set; }
        public TMP_Text warningText;
        private const float TIME_SCALE = 0.01f;
        private string currentMsg = "";
        private int msgRepetition = 0;

        private void Awake()
        {
            WarningTextManagerInstance = MonoBehaviourGeneral.DeclareSingletonDontDestroyOnLoad<WarningTextManager>(this, WarningTextManagerInstance);

            warningText.color = Color.clear;
        }

        private void Start()
        {
            //GetComponent<RectTransform>().position = Vector3.zero;
            //GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        public static void SetColor(Color? color) => currentColor = color == null ? defaultColor : color.Value;

        /// <summary>
        /// Mostra uma mensagem de aviso na tela por um dado tempo.
        /// </summary>
        /// <param name="text">Texto exibido.</param>
        /// <param name="time">Tem em que o texto ficará na tela, tirando o tempo do fade.</param>
        /// <param name="fade">Tempo do efeito de fade in e fade out.</param>
        /// <param name="color">Cor do texto.</param>
        public static void ShowText(string text, float time, float fade = 0, bool fadeIn = true, Color? color = null)
        {
            WarningTextManagerInstance.StopAllCoroutines();

            SetColor(color);

            if (WarningTextManagerInstance.currentMsg != text)
            {
                WarningTextManagerInstance.msgRepetition = 1;
                WarningTextManagerInstance.currentMsg = text;
            }
            else
            {
                WarningTextManagerInstance.msgRepetition++;
            }
            WarningTextManagerInstance.StartCoroutine(WarningTextManagerInstance._ShowText(text, time, fade, fadeIn));
        }

        private IEnumerator _ShowText(string text, float time, float fade, bool fadeIn = true)
        {
            warningText.text = text + (msgRepetition > 1 ? (" (x" + msgRepetition + ")") : "");

            if (fade > 0 && fadeIn)
                for (float i = warningText.color.a; i <= 1; i += TIME_SCALE / fade)
                {
                    warningText.color = new Color(currentColor.r, currentColor.g, currentColor.b, i);
                    yield return new WaitForSeconds(TIME_SCALE);
                }
            warningText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
            for (int i = 0; i < 1; i++)
            {
                yield return new WaitForSeconds(time);
            }
            if (fade > 0)
                for (float i = 0; i <= 1; i += TIME_SCALE / fade)
                {
                    warningText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1 - i);
                    yield return new WaitForSeconds(TIME_SCALE);
                }
            warningText.color = new Color(1, 1, 1, 0);
            warningText.text = "";
            msgRepetition = 0;
        }

        /// <summary>
        /// Mostra uma mensagem de aviso na tela até que a função HideWarning() seja chamada.
        /// </summary>
        /// <param name="text">Texto exibido.</param>
        /// <param name="fade">Tempo do efeito do fade in.</param>
        /// <param name="color">Cor do texto.</param>
        public static void ShowAndKeepText(string text, float fade = 0, Color? color = null)
        {
            WarningTextManagerInstance.StopAllCoroutines();

            SetColor(color);

            if (WarningTextManagerInstance.currentMsg != text)
            {
                WarningTextManagerInstance.msgRepetition = 1;
                WarningTextManagerInstance.currentMsg = text;
            }
            else
            {
                WarningTextManagerInstance.msgRepetition++;
            }

            WarningTextManagerInstance.StartCoroutine(WarningTextManagerInstance._ShowAndKeepText(text, fade));
        }

        private IEnumerator _ShowAndKeepText(string text, float fade)
        {
            warningText.text = text + (msgRepetition > 1 ? (" (x" + msgRepetition + ")") : "");

            if (fade > 0)
                for (float i = warningText.color.a; i <= 1; i += TIME_SCALE / fade)
                {
                    warningText.color = new Color(currentColor.r, currentColor.g, currentColor.b, i);
                    yield return new WaitForSeconds(TIME_SCALE);
                }
            warningText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }

        /// <summary>
        /// Retira a mensagem de aviso da tela.
        /// </summary>
        /// <param name="fade">Tempo do efeito do fade out.</param>
        public static void HideText(float fade = 0)
        {
            if (fade > 0)
            {
                WarningTextManagerInstance.StopAllCoroutines();
                WarningTextManagerInstance.StartCoroutine(WarningTextManagerInstance.FadeOut(fade));
            }
            else
            {
                WarningTextManagerInstance.StopAllCoroutines();
                WarningTextManagerInstance.warningText.color = new Color(1, 1, 1, 0);
                WarningTextManagerInstance.warningText.text = "";
                WarningTextManagerInstance.msgRepetition = 0;
            }
        }

        private IEnumerator FadeOut(float fade)
        {
            if (fade > 0)
                for (float i = warningText.color.a; i <= 1; i += TIME_SCALE / fade)
                {
                    warningText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1 - i);
                    yield return new WaitForSeconds(TIME_SCALE);
                }
            warningText.color = new Color(1, 1, 1, 0);
            warningText.text = "";
            msgRepetition = 0;
        }
    }
}

