using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Creatures
{
    public class CreatureManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform grid;

        [Header("Creature Levels")] [SerializeField]
        private List<CreatureLevelData> levels = new();

        private readonly List<CreatureSlot> slots = new();
        private readonly List<CreatureInstance> creatures = new();

        public int CreatureCount => creatures.Count;
        public int AvailableSlots => slots.Count - creatures.Count;

        private class CreatureInstance
        {
            public int Level;
            public GameObject GameObject;
        }

        private void Awake()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            slots.Clear();

            if (grid == null)
            {
                Debug.LogError("CreatureManager: Grid não configurado!");
                return;
            }

            slots.AddRange(
                grid.GetComponentsInChildren<CreatureSlot>()
            );

            slots.Sort((a, b) =>
            {
                // Fileira inferior primeiro.
                int rowComparison = b.Row.CompareTo(a.Row);

                return rowComparison != 0
                    ? rowComparison
                    : a.Column.CompareTo(b.Column);
            });

            Debug.Log(
                $"CreatureManager: {slots.Count} slots registrados."
            );
        }


        public void ForEachCreature(System.Action<int, CreatureSlot> action)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                action(creatures[i].Level, slots[i]);
            }
        }

        public CreatureLevelData GetLevelData(int level)
        {
            return levels.Find(data => data != null && data.level == level);
        }


        public List<int> GetCreatureLevels()
        {
            List<int> result = new();

            foreach (var creature in creatures)
            {
                result.Add(creature.Level);
            }

            return result;
        }


        public bool AddCreature(int level)
        {
            CreatureLevelData data =
                levels.Find(x => x != null && x.level == level);

            if (data == null || data.prefab == null)
            {
                Debug.LogError(
                    $"Configuração não encontrada para nível {level}!"
                );
                return false;
            }

            if (creatures.Count >= slots.Count)
            {
                Debug.LogWarning("Fábrica cheia!");
                return false;
            }

            GameObject instance = Instantiate(data.prefab);

            creatures.Add(new CreatureInstance
            {
                Level = level,
                GameObject = instance
            });

            ReorganizeCreatures();

            return true;
        }

        private void ReorganizeCreatures()
        {
            creatures.Sort((a, b) =>
                b.Level.CompareTo(a.Level)
            );

            foreach (CreatureSlot slot in slots)
                slot.Clear();

            for (int i = 0; i < creatures.Count; i++)
            {
                slots[i].PlaceCreature(
                    creatures[i].GameObject
                );
            }
        }


        public bool ApplyCreatureLevels(List<int> targetLevels)
        {
            if (targetLevels == null || targetLevels.Count > slots.Count)
                return false;

            // Valida os prefabs antes de modificar a fábrica.
            foreach (int level in targetLevels)
            {
                CreatureLevelData data = GetLevelData(level);

                if (data == null || data.prefab == null)
                {
                    Debug.LogWarning(
                        $"CreatureManager: prefab do nível {level} não configurado."
                    );
                    return false;
                }
            }

            // Organiza as criaturas existentes por nível.
            Dictionary<int, Queue<CreatureInstance>> available = new();

            foreach (CreatureInstance creature in creatures)
            {
                if (!available.TryGetValue(creature.Level, out var queue))
                {
                    queue = new Queue<CreatureInstance>();
                    available[creature.Level] = queue;
                }

                queue.Enqueue(creature);
            }

            List<CreatureInstance> updated = new();

            // Reaproveita criaturas do mesmo nível.
            // Cria somente os objetos que estiverem faltando.
            foreach (int level in targetLevels)
            {
                if (available.TryGetValue(level, out var queue) &&
                    queue.Count > 0)
                {
                    updated.Add(queue.Dequeue());
                    continue;
                }

                CreatureLevelData data = GetLevelData(level);
                GameObject instance = Instantiate(data.prefab);

                updated.Add(new CreatureInstance
                {
                    Level = level,
                    GameObject = instance
                });
            }

            // Remove os objetos consumidos pelos merges.
            foreach (var queue in available.Values)
            {
                while (queue.Count > 0)
                {
                    CreatureInstance removed = queue.Dequeue();
                    Destroy(removed.GameObject);
                }
            }

            creatures.Clear();
            creatures.AddRange(updated);

            ReorganizeCreatures();

            Debug.Log(
                $"CreatureManager: fábrica atualizada. " +
                $"Total de criaturas: {creatures.Count}"
            );

            return true;
        }


        [ContextMenu("Adicionar Criatura Nível 1")]
        private void AddTestCreature()
        {
            AddCreature(1);
        }


        [ContextMenu("Testar Simulação de Merge")]
        private void TestMergeSimulation()
        {
            List<int> currentLevels = new();

            // Simula 99 criaturas nível 1.
            for (int i = 0; i < 99; i++)
                currentLevels.Add(1);

            bool success = CreatureMergeSimulator.TrySimulate(
                currentLevels,
                purchaseLevel: 1,
                purchaseAmount: 25,
                slotCapacity: 100,
                out List<int> result
            );

            if (!success)
            {
                Debug.LogWarning("Compra bloqueada!");
                return;
            }

            int level1 = result.FindAll(x => x == 1).Count;
            int level2 = result.FindAll(x => x == 2).Count;

            Debug.Log(
                $"Simulação concluída! " +
                $"Total: {result.Count} | " +
                $"Nível 1: {level1} | " +
                $"Nível 2: {level2}"
            );
        }


        [ContextMenu("Testar Aplicação de Merge")]
        private void TestApplyMerge()
        {
            bool success = CreatureMergeSimulator.TrySimulate(
                GetCreatureLevels(),
                purchaseLevel: 1,
                purchaseAmount: 100,
                slotCapacity: slots.Count,
                out List<int> result
            );

            if (!success)
            {
                Debug.LogWarning("Simulação não permitiu a compra.");
                return;
            }

            if (ApplyCreatureLevels(result))
            {
                Debug.Log(
                    $"Merge aplicado! Criaturas finais: {CreatureCount}"
                );
            }
        }
    }
}