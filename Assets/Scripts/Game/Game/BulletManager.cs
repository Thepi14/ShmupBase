using System.Collections.Generic;
using ObjectUtils;
using UnityEngine;

namespace Main.BulletSystem
{
    public sealed class BulletManager : MonoBehaviour
    {
        public static BulletManager Singleton { get; private set; }

        public static int LayerBulletPlayer { get; private set; }
        public static int LayerBulletEnemy { get; private set; }

        [SerializeField]
        private List<Bullet> bullets = new List<Bullet>();
        [SerializeField]
        private List<Bullet> iGravityBullets = new List<Bullet>();

        private void Awake()
        {
            Singleton = MonoBehaviourGeneral.DeclareSingleton<BulletManager>(this, Singleton);
            LayerBulletPlayer = LayerMask.GetMask("BulletPlayer");
            LayerBulletEnemy = LayerMask.GetMask("BulletEnemy");
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

        private static bool CanDestroyBullet(Bullet bullet, bool affectExceptions) => bullet.clearMode == Bullet.ClearMode.Clear || (bullet.clearMode == Bullet.ClearMode.Exception && affectExceptions);

        public static bool KillAllBulletsPlayer(bool affectExceptions = false) => KillAllBullets(LayerBulletPlayer);
        public static bool DestroyAllBulletsPlayer(bool affectExceptions = false) => DestroyAllBullets(LayerBulletPlayer);
        public static bool DestroyImmediateAllBulletsPlayer(bool affectExceptions = false) => DestroyImmediateAllBullets(LayerBulletPlayer);

        public static bool KillAllBulletsEnemy(bool affectExceptions = false) => KillAllBullets(LayerBulletEnemy);
        public static bool DestroyAllBulletsEnemy(bool affectExceptions = false) => DestroyAllBullets(LayerBulletEnemy);
        public static bool DestroyImmediateAllBulletsEnemy(bool affectExceptions = false) => DestroyImmediateAllBullets(LayerBulletEnemy);

        /// <summary>
        /// Mata todas as balas.
        /// </summary>
        /// <param name="mask">Máscara de layers que serão incluídas na operação, se esse valor for nulo todas as layers serão incluídas.</param>
        /// <returns>Retorna true se ele matou alguma bala, false se nenhuma.</returns>
        public static bool KillAllBullets(LayerMask? mask = null, bool affectExceptions = false)
        {
            bool killAny = false;
            foreach (Bullet bullet in GetAllBullets(mask))
            {
                if (!CanDestroyBullet(bullet, affectExceptions))
                    continue;
                killAny = true;
                bullet.Kill();
            }

            return killAny;
        }

        /// <summary>
        /// Destrói todas as balas.
        /// </summary>
        /// <param name="mask">Máscara de layers que serão incluídas na operação, se esse valor for nulo todas as layers serão incluídas.</param>
        /// <returns>Retorna true se ele destruiu alguma bala, false se nenhuma.</returns>
        public static bool DestroyAllBullets(LayerMask? mask = null, bool affectExceptions = false)
        {
            bool destroyedAny = false;
            foreach (Bullet bullet in GetAllBullets(mask))
            {
                if (!CanDestroyBullet(bullet, affectExceptions))
                    continue;
                destroyedAny = true;
                Destroy(bullet.gameObject);
            }

            return destroyedAny;
        }

        /// <summary>
        /// Destrói imediatamente todas as balas. NÃO É RECOMENDADO.
        /// </summary>
        /// <param name="mask">Máscara de layers que serão incluídas na operação, se esse valor for nulo todas as layers serão incluídas.</param>
        /// <returns>Retorna true se ele destruiu alguma bala, false se nenhuma.</returns>
        public static bool DestroyImmediateAllBullets(LayerMask? mask = null, bool affectExceptions = false)
        {
            bool destroyedAny = false;
            foreach (Bullet bullet in GetAllBullets(mask))
            {
                if (!CanDestroyBullet(bullet, affectExceptions))
                    continue;
                destroyedAny = true;
                DestroyImmediate(bullet.gameObject);
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
