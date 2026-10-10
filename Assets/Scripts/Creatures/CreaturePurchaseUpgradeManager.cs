
using System;
using System.Collections.Generic;
using System.Numerics;
using Economy;
using UnityEngine;

namespace Creatures
{
    public class CreaturePurchaseUpgradeManager : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private CreatureManager creatureManager;
        [SerializeField] private CreaturePurchaseManager purchaseManager;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private CreatureProductionSystem productionSystem;

        [Header("Upgrades disponíveis")]
        [SerializeField]
        private List<CreaturePurchaseUpgradeData> upgrades = new();

        public event Action OnUpgradeChanged;

        private bool lastCanPurchase;
        private bool availabilityInitialized;

        private void Update()
        {
            bool canPurchase = CanPurchaseNextUpgrade();

            if (!availabilityInitialized || canPurchase != lastCanPurchase)
            {
                availabilityInitialized = true;
                lastCanPurchase = canPurchase;

                OnUpgradeChanged?.Invoke();
            }
        }

        public BigInteger GetEggsPerMinute()
        {
            if (creatureManager == null || productionSystem == null)
                return BigInteger.Zero;

            BigInteger eggsPerCycle = BigInteger.Zero;

            creatureManager.ForEachCreature((level, slot) =>
            {
                CreatureLevelData data = creatureManager.GetLevelData(level);

                if (data != null)
                    eggsPerCycle += data.EggsPerCycle;
            });

            // Produção potencial, independentemente do armazém.
            const long timeScale = 1000000;

            long intervalUnits = (long)System.Math.Round(
                productionSystem.ProductionInterval * timeScale
            );

            if (intervalUnits <= 0)
                return BigInteger.Zero;

            return eggsPerCycle * (60L * timeScale) / intervalUnits;
        }

        public CreaturePurchaseUpgradeData GetNextUpgrade()
        {
            if (purchaseManager == null)
                return null;

            int nextLevel = purchaseManager.PurchaseLevel + 1;

            return upgrades.Find(upgrade =>
                upgrade != null && upgrade.TargetLevel == nextLevel
            );
        }

        public bool CanPurchaseNextUpgrade()
        {
            CreaturePurchaseUpgradeData upgrade = GetNextUpgrade();

            if (upgrade == null || currencyManager == null)
                return false;

            BigInteger requiredProduction = upgrade.RequiredEggsPerMinute;
            BigInteger cost = upgrade.Cost;

            if (requiredProduction <= BigInteger.Zero ||
                cost <= BigInteger.Zero)
                return false;

            return GetEggsPerMinute() >= requiredProduction &&
                   currencyManager.CanAfford(cost);
        }

        public bool TryPurchaseNextUpgrade()
        {
            if (!CanPurchaseNextUpgrade())
            {
                Debug.Log("Upgrade indisponível: confira produção e moedas.");
                return false;
            }

            CreaturePurchaseUpgradeData upgrade = GetNextUpgrade();
            int previousLevel = purchaseManager.PurchaseLevel;
            BigInteger cost = upgrade.Cost;

            if (!currencyManager.TrySpend(cost))
                return false;

            if (!purchaseManager.TryUpgradePurchaseLevel(upgrade.TargetLevel))
            {
                currencyManager.AddCoins(cost);

                Debug.LogError("Falha ao aplicar upgrade de compra.");
                return false;
            }

            OnUpgradeChanged?.Invoke();

            Debug.Log(
                $"Upgrade comprado! Nv.{previousLevel} → Nv.{upgrade.TargetLevel} " +
                $"| Custo: {cost:N0} moedas " +
                $"| Produção: {GetEggsPerMinute():N0} ovos/min"
            );

            return true;
        }

        [ContextMenu("Testar próximo upgrade")]
        private void TestNextUpgrade()
        {
            CreaturePurchaseUpgradeData upgrade = GetNextUpgrade();

            if (upgrade == null)
            {
                Debug.Log("Nenhum próximo upgrade configurado.");
                return;
            }

            Debug.Log(
                $"Próximo upgrade: Nv.{upgrade.TargetLevel} " +
                $"| Exige: {upgrade.RequiredEggsPerMinute:N0} ovos/min " +
                $"| Produção atual: {GetEggsPerMinute():N0} ovos/min " +
                $"| Custo: {upgrade.Cost:N0} moedas " +
                $"| Disponível: {CanPurchaseNextUpgrade()}"
            );
        }

        [ContextMenu("Comprar próximo upgrade")]
        private void TestPurchaseUpgrade()
        {
            TryPurchaseNextUpgrade();
        }
    }
}
