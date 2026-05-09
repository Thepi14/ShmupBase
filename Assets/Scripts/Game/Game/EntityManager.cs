using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Main.EntitySystem
{
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager Singleton { get; private set; }

        public static int LayerPlayer { get; private set; }
        public static int LayerEnemy { get; private set; }

        [SerializeField]
        protected List<Entity> entities = new List<Entity>();

        [SerializeField]
        public PlayerEntity playerEntity;
        [SerializeField]
        protected GameObject playerEntityPrefab;

        public static PlayerEntity GetPlayer() => Singleton.playerEntity;
        public static PlayerEntity GeneratePlayer() => Singleton.playerEntity = Instantiate(Singleton.playerEntityPrefab).GetComponent<PlayerEntity>();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingletonDontDestroyOnLoad<EntityManager>(this, Singleton);
            LayerPlayer = LayerMask.GetMask("Player");
            LayerEnemy = LayerMask.GetMask("Enemy");
        }

        private void Update()
        {

        }

        public static void KillAllPlayer() => Singleton.playerEntity.Kill();
        public static void KillAllEnemies() => KillAllHealtyEntities(LayerEnemy);

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

        public static void SetPlayerEntity(PlayerEntity player) => Singleton.playerEntity = player;
        public static PlayerEntity GetPlayerEntity() => Singleton.playerEntity;
    }
}
