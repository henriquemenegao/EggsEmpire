
using UnityEngine;

namespace Factory.Conveyor
{
    public class EggTransportManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EggPool eggPool;
        [SerializeField] private Transform grid;

        [Header("Conveyor Settings")]
        [SerializeField] private float centralConveyorX = 0f;
        [SerializeField] private float exitZ = -55;
        [SerializeField] private float eggHeight = 0.68f;
        [SerializeField] private float rowOffsetZ = -1.75f;

        [Header("Test Settings")]
        [SerializeField] private bool autoTest = true;
        [SerializeField] private float spawnInterval = 2f;

        private float timer;

        private void Update()
        {
            if (!autoTest || eggPool == null || grid == null)
                return;

            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnRandomEgg();
            }
        }

        [ContextMenu("Spawn Random Egg")]
        public void SpawnRandomEgg()
        {
            if (eggPool == null || grid == null)
                return;

            if (grid.childCount == 0)
                return;

            int index = Random.Range(0, grid.childCount);

            Transform slot = grid.GetChild(index);

            SpawnEgg(slot);
        }

        public void SpawnEgg(Transform slot)
        {
            SpawnEgg(slot, null);
        }

        public void SpawnEgg(Transform slot, GameObject eggPrefab)
        {
            if (slot == null || eggPool == null)
                return;

            Vector3 spawnPosition = new Vector3(
                slot.position.x,
                eggHeight,
                slot.position.z + rowOffsetZ
            );

            GameObject egg = eggPool.GetEgg(eggPrefab);

            if (egg == null)
                return;

            egg.transform.position = spawnPosition;
            egg.transform.rotation = Quaternion.identity;

            EggTransport transport =
                egg.GetComponent<EggTransport>();

            if (transport == null)
            {
                Debug.LogError(
                    "Egg prefab missing EggTransport component!"
                );

                eggPool.ReturnEgg(egg);
                return;
            }

            transport.Initialize(
                centralConveyorX,
                exitZ,
                eggPool
            );
        }
    }
}
