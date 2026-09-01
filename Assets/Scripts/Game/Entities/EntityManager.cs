using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

namespace Main.EntitySystem
{
    public sealed class EntityManager : MonoBehaviour
    {
        public static EntityManager Singleton { get; private set; }

        public List<HealthEntity> currentBosses = new();
        [SerializeField]
        private List<IHealth> healthEntities = new();
        [SerializeField]
        private List<Entity> entities = new();

        public UnityEvent<Entity> onEntityAdded = new(), onEntityRemoved = new();
        public UnityEvent<IHealth> onHealthEntityAdded = new(), onHealthEntityRemoved = new();
        public UnityEvent<HealthEntity> onBossAdded = new(), onBossRemoved = new();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
        }

        public static void AddBoss(HealthEntity entity)
        {
            Singleton.currentBosses.Add(entity);
            Singleton.onBossAdded.Invoke(entity);
        }

        public static void AddBosses(params HealthEntity[] entities)
        {
            Singleton.currentBosses.AddRange(entities);

            foreach (HealthEntity entity in entities)
            {
                Singleton.onBossAdded.Invoke(entity);
            }
        }

        public static void RemoveBoss(HealthEntity entity)
        {
            Singleton.currentBosses.Remove(entity);
            Singleton.onBossRemoved.Invoke(entity);
        }

        

        public static void AddEntity(Entity entity)
        {
            if (entity is IHealth health)
            {
                Singleton.healthEntities.Add(health);
                Singleton.onHealthEntityAdded.Invoke(health);
            }

            Singleton.entities.Add(entity);
            Singleton.onEntityAdded.Invoke(entity);
        }

        public static void RemoveEntity(Entity entity)
        {
            if (entity is IHealth health)
            {
                Singleton.healthEntities.Remove(health);
                Singleton.onHealthEntityRemoved.Invoke(health);
            }

            Singleton.entities.Remove(entity);
            Singleton.onEntityRemoved.Invoke(entity);
        }

        public static void KillAllEnemies() => KillAllHealthEntities(Vars.GetMask(Vars.Layer.Enemy));

        public static void KillAllHealthEntities(LayerMask? mask = null)
        {
            foreach (var health in GetAllHealthEntities(mask))
            {
                health.Kill();
            }
        }

        public static Entity[] GetAllEntities() => Singleton.entities.ToArray();

        public static Entity[] GetAllEntities(LayerMask? mask = null)
        {
            if (mask == null)
                mask = ~0;
            return Singleton.entities.FindAll((entity) => mask.Value.MaskContainsLayer(entity.gameObject.layer)).ToArray();
        }

        public static IHealth[] GetAllHealthEntities() => Singleton.healthEntities.ToArray();

        public static IHealth[] GetAllHealthEntities(LayerMask? mask = null)
        {
            if (mask == null)
                mask = ~0;
            return Singleton.healthEntities.FindAll((entity) => mask.Value.MaskContainsLayer((entity as Entity).gameObject.layer)).ToArray();
        }
    }

    public class WaitForAllBossesToDie : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                return EntityManager.Singleton.currentBosses.Count > 0;
            }
        }

        public WaitForAllBossesToDie()
        {

        }
    }

    public class WaitForOneBossToDie : CustomYieldInstruction
    {
        public bool bossDied { get; private set; } = false;

        public override bool keepWaiting
        {
            get
            {
                return !bossDied;
            }
        }

        public WaitForOneBossToDie()
        {
            EntityManager.Singleton.onBossRemoved.AddListener((h) => bossDied = true);
        }
    }
}
