using System;
using System.Globalization;
using System.Numerics;
using UnityEngine;

namespace Economy
{
    public class CurrencyManager : MonoBehaviour
    {
        [Header("Initial Settings")]
        [SerializeField] private string initialCoins = "1000";

        public BigInteger Coins { get; private set; }

        public event Action<BigInteger> OnCoinsChanged;

        private void Awake()
        {
            if (!BigInteger.TryParse(
                    initialCoins,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out BigInteger parsedCoins) ||
                parsedCoins < BigInteger.Zero)
            {
                Debug.LogWarning(
                    "CurrencyManager: invalid initial balance. Using zero."
                );

                parsedCoins = BigInteger.Zero;
            }

            Coins = parsedCoins;
        }

        public bool CanAfford(BigInteger amount)
        {
            return amount >= BigInteger.Zero && Coins >= amount;
        }

        public bool TrySpend(BigInteger amount)
        {
            if (amount <= BigInteger.Zero || !CanAfford(amount))
                return false;

            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);

            return true;
        }

        public void AddCoins(BigInteger amount)
        {
            if (amount <= BigInteger.Zero)
                return;

            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }

        [ContextMenu("Test: Add 100 Coins")]
        private void TestAddCoins()
        {
            AddCoins(100);
            Debug.Log($"Coins: {Coins}");
        }

        [ContextMenu("Test: Spend 50 Coins")]
        private void TestSpendCoins()
        {
            bool success = TrySpend(50);

            Debug.Log(
                $"Purchase: {success} | Coins: {Coins}"
            );
        }
    }
}