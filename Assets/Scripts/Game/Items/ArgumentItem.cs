using Main.EntitySystem;
using UnityEngine;

namespace Main.ItemSystem
{
    public class ArgumentItem : ScoreItem
    {
        public string argument = "";

        protected override void Collect()
        {
            base.Collect();
            PlayerEntity.AddArgument(argument);
        }
    }
}
