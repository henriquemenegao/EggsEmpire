
using System;
using System.Numerics;
using Economy;
using UnityEngine;

namespace Creatures
{
    public class CreaturePurchaseManager : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private CreatureManager creatureManager;

        [Header("Configuração de compra")]
        [SerializeField, Range(1, 18)]
        private int purchaseLevel = 1;

        [SerializeField] private string initialPrice = "1";
        [SerializeField] private string priceIncrease = "2";

        private BigInteger successfulPurchases = BigInteger.Zero;

        public int PurchaseLevel => purchaseLevel;
        
        public event Action OnPurchaseChanged;

        private void NotifyPurchaseChanged()
        {
            OnPurchaseChanged?.Invoke();
        }
        
        public bool TryUpgradePurchaseLevel(int targetLevel)
        {
            if (targetLevel != purchaseLevel + 1 || targetLevel > 18)
                return false;

            purchaseLevel = targetLevel;
            NotifyPurchaseChanged();

            Debug.Log(
                $"Nível inicial de compra atualizado para Nv.{purchaseLevel} " +
                $"| Preço unitário: {GetUnitPrice():N0} moedas"
            );

            return true;
        }

        public BigInteger CurrentBasePrice
        {
            get
            {
                BigInteger basePrice = BigInteger.Parse(initialPrice);
                BigInteger increase = BigInteger.Parse(priceIncrease);

                return basePrice + successfulPurchases * increase;
            }
        }

        public BigInteger GetUnitPrice()
        {
            BigInteger multiplier = BigInteger.Pow(
                new BigInteger(3),
                purchaseLevel - 1
            );

            return CurrentBasePrice * multiplier;
        }

        public BigInteger GetTotalPrice(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return GetUnitPrice() * amount;
        }

        public bool TryBuyCreature()
        {
            return TryBuyCreatures(1);
        }

        public bool TryBuyCreatures(int amount)
        {
            if (amount <= 0 ||
                currencyManager == null ||
                creatureManager == null)
            {
                return false;
            }

            BigInteger totalCost;

            try
            {
                totalCost = GetTotalPrice(amount);
            }
            catch (FormatException)
            {
                Debug.LogError(
                    "CreaturePurchaseManager: configuração de preço inválida."
                );
                return false;
            }

            if (!currencyManager.CanAfford(totalCost))
            {
                Debug.Log("Moedas insuficientes.");
                return false;
            }

            if (!CreatureMergeSimulator.TrySimulate(
                    creatureManager.GetCreatureLevels(),
                    purchaseLevel,
                    amount,
                    100,
                    out var finalLevels))
            {
                Debug.Log("Sem espaço para essa compra.");
                return false;
            }

            if (!creatureManager.ApplyCreatureLevels(finalLevels))
            {
                Debug.LogWarning("Não foi possível aplicar a compra.");
                return false;
            }

            if (!currencyManager.TrySpend(totalCost))
            {
                Debug.LogError("Falha inesperada ao cobrar a compra.");
                return false;
            }

            successfulPurchases += BigInteger.One;
            NotifyPurchaseChanged();

            Debug.Log(
                $"Compra realizada: {amount} criatura(s) Nv.{purchaseLevel} " +
                $"| Total: {totalCost:N0} moedas " +
                $"| Próximo preço base: {CurrentBasePrice:N0}"
            );

            return true;
        }

        [ContextMenu("Comprar 1 criatura")]
        private void Buy1() => TryBuyCreatures(1);

        [ContextMenu("Comprar 5 criaturas")]
        private void Buy5() => TryBuyCreatures(5);

        [ContextMenu("Comprar 25 criaturas")]
        private void Buy25() => TryBuyCreatures(25);

        [ContextMenu("Comprar 100 criaturas")]
        private void Buy100() => TryBuyCreatures(100);
    }
}
