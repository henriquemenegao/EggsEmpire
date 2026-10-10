
using System.Globalization;
using System.Numerics;
using UnityEngine;

namespace Warehouse
{
    public class WarehouseManager : MonoBehaviour
    {
        [Header("Armazém")]
        [SerializeField]
        private string capacity = "1000";

        [SerializeField]
        private string initialStoredEggs = "0";

        public BigInteger Capacity { get; private set; }
        public BigInteger StoredEggs { get; private set; }

        public BigInteger AvailableSpace =>
            BigInteger.Max(BigInteger.Zero, Capacity - StoredEggs);

        public bool IsFull => StoredEggs >= Capacity;

        private void Awake()
        {
            if (!BigInteger.TryParse(
                    capacity,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out BigInteger parsedCapacity) ||
                parsedCapacity <= BigInteger.Zero)
            {
                Debug.LogWarning(
                    "WarehouseManager: capacidade inválida. Usando 1000."
                );

                parsedCapacity = new BigInteger(1000);
            }

            Capacity = parsedCapacity;

            if (!BigInteger.TryParse(
                    initialStoredEggs,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out BigInteger parsedEggs) ||
                parsedEggs < BigInteger.Zero)
            {
                Debug.LogWarning(
                    "WarehouseManager: quantidade inicial inválida. Usando zero."
                );

                parsedEggs = BigInteger.Zero;
            }

            StoredEggs = BigInteger.Min(parsedEggs, Capacity);
        }

        public BigInteger StoreEggs(BigInteger amount)
        {
            if (amount <= BigInteger.Zero || IsFull)
                return BigInteger.Zero;

            BigInteger accepted = BigInteger.Min(amount, AvailableSpace);

            StoredEggs += accepted;

            Debug.Log(
                $"Armazém: +{accepted} ovos | " +
                $"{StoredEggs}/{Capacity}"
            );

            return accepted;
        }

        public BigInteger RemoveEggs(BigInteger amount)
        {
            if (amount <= BigInteger.Zero)
                return BigInteger.Zero;

            BigInteger removed = BigInteger.Min(amount, StoredEggs);

            StoredEggs -= removed;

            return removed;
        }
    }
}
