using UnityEngine;

namespace Main.ItemSystem
{
    public class ScoreItem : Item
    {
        public long score;

        protected override void Collect()
        {
            GameManager.AddScore(score);
        }
    }
}
