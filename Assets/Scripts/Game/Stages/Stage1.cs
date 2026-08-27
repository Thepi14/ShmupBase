using System.Collections;
using UnityEngine;
using Main.BulletSystem;
using Main.EntitySystem;
using static Main.TimeManager;

namespace Main.Stages
{
    public class Stage1 : StageBehaviour
    {
        public GameObject scenario;
        public GameObject enemy;
        public GameObject bullet;

        protected override IEnumerator StageCoroutine()
        {
            //na classe herdada há um código de eliminação de balas, é recomendado invocar esse código
            yield return base.StageCoroutine();

            var str = "";
            for (int i = 0; i < 10; i++)
            {
                str += GameManager.random.Next(0, 10) + ", ";
            }

            Debug.Log(str);

            //chaves usadas assim podem ser usadas para isolar variáveis/funções locais
            {
                //coisas aqui
            }

            //criar uma bala na posição (0, 0), girar 90 graus para a direita (apontar para baixo) e gerar uma corotina na bala
            {
                for (int i = 0; i < 3; i++)
                {
                    var bulletObj = Instantiate(bullet).GetComponent<Bullet>();
                    bulletObj.Set(Vector2.zero, -90f);
                    bulletObj.customCoroutine = ExampleCoroutine;
                    yield return WaitFixedFrames(30);
                }

                //depois de 60 frames (1 segundo), alterar o ângulo lentamente e acelerar 1 unidade a cada segundo
                IEnumerator ExampleCoroutine(GameObject thisBulletObj) {
                    if ((thisBulletObj.GetComponent<BasicBullet>() is BasicBullet basicBullet) && basicBullet != null)
                    {
                        basicBullet.angularVelocity = 3f;
                        basicBullet.acceleration = 1f;
                    }
                    yield return WaitFixedFrames(60);
                }
            }

            //criar um inimigo na posição (1, 1), mover ele para (0, 0)
            {
                var enemyObj = Instantiate(enemy).GetComponent<BasicEntity>();
                enemyObj.transform.position = Vector2.one;
                enemyObj.MoveTo(new Vector2(0f, 0f));
                enemyObj.customCoroutine = ExampleCoroutine;

                //depois de meio segundo, começar uma função genérica de atirar no player
                IEnumerator ExampleCoroutine(GameObject thisEntityObj)
                {
                    var entity = enemyObj;
                    yield return WaitFixedFrames(30);
                    while (entity.Alive)
                    {
                        yield return WaitFixedFrames(20);
                        var bulletObj = Instantiate(bullet).GetComponent<Bullet>();
                        var pos = (Vector2)enemyObj.transform.position;
                        bulletObj.Set(pos, PlayerEntity.AngleToPlayer(pos));
                    }
                }
            }

            yield break;
        }

        protected override IEnumerator BackgroundCoroutine()
        {
            yield break;
        }
    }
}
