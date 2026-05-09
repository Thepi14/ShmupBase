using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;

namespace Main.BulletSystem
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager Singleton { get; private set; }

        public static int LayerBulletPlayer { get; private set; }
        public static int LayerBulletEnemy { get; private set; }

        [SerializeField]
        protected List<Bullet> bullets = new List<Bullet>();
        [SerializeField]
        protected List<Bullet> iGravityBullets = new List<Bullet>();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingletonDontDestroyOnLoad<BulletManager>(this, Singleton);
            LayerBulletPlayer = LayerMask.GetMask("BulletPlayer");
            LayerBulletEnemy = LayerMask.GetMask("BulletEnemy");
        }

        private void Update()
        {

        }

        #region BULLETS

        public static void AddBullet(Bullet bullet)
        {
            if (bullet == null) return;

            Singleton.bullets.Add(bullet);
            if (bullet is IGravityBullet)
            {
                Singleton.iGravityBullets.Add(bullet);
            }
        }

        public static void RemoveBullet(Bullet bullet)
        {
            Singleton.bullets.Remove(bullet);
            if (bullet is IGravityBullet)
            {
                Singleton.iGravityBullets.Remove(bullet);
            }
        }

        public static bool KillAllBulletsPlayer() => KillAllBullets(LayerBulletPlayer);
        public static bool DestroyAllBulletsPlayer() => DestroyAllBullets(LayerBulletPlayer);
        public static bool DestroyImmediateAllBulletsPlayer() => DestroyImmediateAllBullets(LayerBulletPlayer);

        public static bool KillAllBulletsEnemy() => KillAllBullets(LayerBulletEnemy);
        public static bool DestroyAllBulletsEnemy() => DestroyAllBullets(LayerBulletEnemy);
        public static bool DestroyImmediateAllBulletsEnemy() => DestroyImmediateAllBullets(LayerBulletEnemy);

        /// <summary>
        /// Mata todas as balas.
        /// </summary>
        /// <param name="mask">Máscara de layers que serão incluídas na operação, se esse valor for nulo todas as layers serão incluídas.</param>
        /// <returns>Retorna true se ele matou alguma bala, false se nenhuma.</returns>
        public static bool KillAllBullets(LayerMask? mask = null)
        {
            bool killAny = false;
            foreach (Bullet bullet in GetAllBullets(mask))
            {
                killAny = true;
                RemoveBullet(bullet);
                bullet.Kill();
            }

            return killAny;
        }

        /// <summary>
        /// Destrói todas as balas.
        /// </summary>
        /// <param name="mask">Máscara de layers que serão incluídas na operação, se esse valor for nulo todas as layers serão incluídas.</param>
        /// <returns>Retorna true se ele destruiu alguma bala, false se nenhuma.</returns>
        public static bool DestroyAllBullets(LayerMask? mask = null)
        {
            bool destroyedAny = false;
            foreach (Bullet bullet in GetAllBullets(mask))
            {
                destroyedAny = true;
                RemoveBullet(bullet);
                Destroy(bullet);
            }

            return destroyedAny;
        }

        /// <summary>
        /// Destrói imediatamente todas as balas. NÃO É RECOMENDADO.
        /// </summary>
        /// <param name="mask">Máscara de layers que serão incluídas na operação, se esse valor for nulo todas as layers serão incluídas.</param>
        /// <returns>Retorna true se ele destruiu alguma bala, false se nenhuma.</returns>
        public static bool DestroyImmediateAllBullets(LayerMask? mask = null)
        {
            bool destroyedAny = false;
            foreach (Bullet bullet in GetAllBullets(mask))
            {
                destroyedAny = true;
                RemoveBullet(bullet);
                DestroyImmediate(bullet);
            }

            return destroyedAny;
        }

        public static Bullet[] GetAllBullets(LayerMask? mask = null)
        {
            if (mask == null)
                mask = ~0;
            return Singleton.bullets.FindAll((bullet) => mask.Value.MaskContainsLayer(bullet.gameObject.layer)).ToArray();
        }

        public static Bullet[] GetAllBullets() => Singleton.bullets.ToArray();
        public static Bullet[] GetAllGravitationalBullets() => Singleton.iGravityBullets.ToArray();

        #endregion
    }
}
