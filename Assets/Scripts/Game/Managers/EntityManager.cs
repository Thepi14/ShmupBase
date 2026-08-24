using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Main.EntitySystem
{
    public sealed class EntityManager : MonoBehaviour
    {
        public static EntityManager Singleton { get; private set; }

        public static int LayerPlayer { get; private set; }
        public static int LayerEnemy { get; private set; }

        [SerializeField]
        private List<Entity> entities = new List<Entity>();

        [SerializeField]
        public PlayerEntity playerEntity;
        [SerializeField]
        private GameObject playerEntityPrefab;

        public static PlayerEntity GeneratePlayer() => Singleton.playerEntity = Instantiate(Singleton.playerEntityPrefab).GetComponent<PlayerEntity>();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
            LayerPlayer = LayerMask.GetMask("Player");
            LayerEnemy = LayerMask.GetMask("Enemy");
        }

        public static float AngleToPlayer(Vector2 position) => Mathf.Atan2(GetPlayer().transform.position.y - position.y, GetPlayer().transform.position.x - position.x) * Mathf.Rad2Deg;

        public static void KillPlayer() => Singleton.playerEntity.Kill();
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

        public static PlayerEntity GetPlayer() => Singleton.playerEntity;
        public static void SetPlayer(PlayerEntity player) => Singleton.playerEntity = player;
    }
}
