using Main.EntitySystem;
using UnityEngine;

namespace Main.ItemSystem
{
    public class ArgumentItem : Item
    {
        public string argument = "";

        protected override void Collect()
        {
            PlayerEntity.AddArgument(argument);
        }
    }
}
