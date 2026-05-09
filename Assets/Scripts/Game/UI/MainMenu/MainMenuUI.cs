using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public Button startButton;
        public Button exitButton;

        protected void Start()
        {
            startButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
            exitButton.onClick.AddListener(() => { Application.Quit(); });
        }

        protected void LateUpdate()
        {

        }
    }
}
