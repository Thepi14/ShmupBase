using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;

namespace Main.ItemSystem
{
    public sealed class ItemManager : MonoBehaviour
    {
        public static ItemManager Singleton { get; private set; }

        [SerializeField]
        private List<Item> items = new();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
        }

        public static void AddItem(Item item)
        {
            Singleton.items.Add(item);
        }

        public static void RemoveItem(Item item)
        {
            Singleton.items.Remove(item);
        }
    }
}
