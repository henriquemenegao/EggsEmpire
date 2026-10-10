using System.Numerics;
using Factory.Conveyor;
using UnityEngine;
using Warehouse;

namespace Creatures
{
    public class CreatureProductionSystem : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private CreatureManager creatureManager;
        [SerializeField] private WarehouseManager warehouseManager;
        [SerializeField] private EggTransportManager eggTransportManager;

        [Header("Produção")]
        [SerializeField, Min(0.1f)]
        private float productionInterval = 5f;
        public float ProductionInterval => productionInterval;

        private float timer;

        private void Update()
        {
            if (creatureManager == null ||
                warehouseManager == null ||
                eggTransportManager == null)
                return;

            if (warehouseManager.IsFull)
            {
                timer = 0f;
                return;
            }

            timer += Time.deltaTime;

            if (timer < productionInterval)
                return;

            timer -= productionInterval;
            Produce();
        }

        private void Produce()
        {
            creatureManager.ForEachCreature((level, slot) =>
            {
                if (warehouseManager.IsFull)
                    return;

                CreatureLevelData data =
                    creatureManager.GetLevelData(level);

                if (data == null)
                    return;

                BigInteger amount = data.EggsPerCycle;

                BigInteger stored = warehouseManager.StoreEggs(amount);

                if (stored > BigInteger.Zero)
                {
                    eggTransportManager.SpawnEgg(
                        slot.transform,
                        data.eggPrefab
                    );
                }
            });
        }
    }
}