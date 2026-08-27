using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Main.EntitySystem
{
    public sealed class EntityManager : MonoBehaviour
    {
        public static EntityManager Singleton { get; private set; }

        [SerializeField]
        private List<Entity> entities = new List<Entity>();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
        }
        public static void KillAllEnemies() => KillAllHealtyEntities(Vars.GetMask(Vars.Layer.Enemy));

        public static void KillAllHealtyEntities(LayerMask? mask = null)
        {
            var entities = GetAllEntities(mask);
            foreach (var entity in entities)
            {
                if (entity is IHealth health)
                {
                    health.Kill();
                }
            }
        }

        public static Entity[] GetAllEntities(LayerMask? mask = null)
        {
            if (mask == null)
                mask = ~0;
            return Singleton.entities.FindAll((bullet) => mask.Value.MaskContainsLayer(bullet.gameObject.layer)).ToArray();
        }

        public static Entity[] GetAllEntities() => Singleton.entities.ToArray();
    }
}
