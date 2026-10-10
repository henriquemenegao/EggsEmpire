
using UnityEngine;

namespace Factory.Conveyor
{
    public class EggTransport : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;

        private enum TransportStage
        {
            Waiting,
            Horizontal,
            Central,
            Finished
        }

        private TransportStage stage = TransportStage.Waiting;

        private Vector3 target;
        private float centralX;
        private float destinationZ;
        private EggPool pool;

        public void Initialize(
            float centralConveyorX,
            float exitZ,
            EggPool eggPool)
        {
            centralX = centralConveyorX;
            destinationZ = exitZ;
            pool = eggPool;

            stage = TransportStage.Horizontal;

            target = new Vector3(
                centralX,
                transform.position.y,
                transform.position.z
            );
        }

        private void Update()
        {
            if (stage == TransportStage.Waiting ||
                stage == TransportStage.Finished)
                return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                NextStage();
            }
        }

        private void NextStage()
        {
            if (stage == TransportStage.Horizontal)
            {
                stage = TransportStage.Central;

                target = new Vector3(
                    centralX,
                    transform.position.y,
                    destinationZ
                );
            }
            else if (stage == TransportStage.Central)
            {
                stage = TransportStage.Finished;

                if (pool != null)
                    pool.ReturnEgg(gameObject);
                else
                    gameObject.SetActive(false);
            }
        }
    }
}
