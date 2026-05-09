using TMPro;
using UnityEngine;
using static Main.GameManager;

namespace Main.UI
{
    public class GameUI : MonoBehaviour
    {
        public TextMeshPro highScoreText;
        public TextMeshPro scoreText;

        public GameObject lifeIconPrefab;
        public GameObject lifesHorizontalLayout;

        public bool commas = true;

        private void OnValidate()
        {
            highScoreText.text = "Highscore: " + (commas ? "000,000,000" : "000000000");
            scoreText.text = "Score: " + (commas ? "000,000,000" : "000000000");
        }

        private void Start()
        {

        }

        private void LateUpdate()
        {
            FormatScoreText();
        }

        public void FormatScoreText()
        {
            scoreText.text = "Score: " + Singleton.score.ToString(commas ? "000,000,000" : "000000000");
        }
        public void FormatHighScoreText()
        {
            highScoreText.text = "Score: " + Singleton.highScore.ToString(commas ? "000,000,000" : "000000000");
        }
    }
}
