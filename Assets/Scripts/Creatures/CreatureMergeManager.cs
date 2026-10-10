using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures
{
    public class CreatureMergeManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CreatureManager creatureManager;

        public event Action<bool> OnMergeAvailabilityChanged;

        private bool _lastAvailability;

        public bool CanMerge =>
            creatureManager != null &&
            CreatureMergeSimulator.HasAvailableMerge(
                creatureManager.GetCreatureLevels()
            );

        private void Start()
        {
            RefreshAvailability();
        }

        private void Update()
        {
            // Por enquanto, acompanha alterações nas criaturas.
            // Depois podemos substituir por eventos.
            RefreshAvailability();
        }

        public bool TryMerge()
        {
            if (!CanMerge)
                return false;

            List<int> finalLevels =
                CreatureMergeSimulator.SimulateManualMerge(
                    creatureManager.GetCreatureLevels()
                );

            if (!creatureManager.ApplyCreatureLevels(finalLevels))
                return false;

            RefreshAvailability();

            Debug.Log(
                $"Merge manual concluído! " +
                $"Criaturas restantes: {creatureManager.CreatureCount}"
            );

            return true;
        }

        private void RefreshAvailability()
        {
            bool available = CanMerge;

            if (available == _lastAvailability)
                return;

            _lastAvailability = available;
            OnMergeAvailabilityChanged?.Invoke(available);
        }

        [ContextMenu("Testar Merge Manual")]
        private void TestMerge()
        {
            TryMerge();
        }
    }
}