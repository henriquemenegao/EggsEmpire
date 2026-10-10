
using System.Collections.Generic;
using UnityEngine;

namespace Factory.Conveyor
{
    public class EggPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject eggPrefab;
        [SerializeField] private int initialSize = 30;

        private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
        private readonly Dictionary<GameObject, GameObject> prefabByEgg = new();

        private void Awake()
        {
            if (eggPrefab == null)
            {
                Debug.LogError("EggPool: prefab padrão não configurado!");
                return;
            }

            // Mantém os 30 ovos iniciais do modelo padrão.
            for (int i = 0; i < initialSize; i++)
            {
                GameObject egg = CreateEgg(eggPrefab);
                ReturnEgg(egg);
            }
        }

        private GameObject CreateEgg(GameObject prefab)
        {
            GameObject egg = Instantiate(prefab, transform);
            egg.SetActive(false);

            prefabByEgg[egg] = prefab;

            return egg;
        }

        public GameObject GetEgg()
        {
            // Compatibilidade com o transporte atual.
            return GetEgg(eggPrefab);
        }

        public GameObject GetEgg(GameObject prefab)
        {
            if (prefab == null)
                prefab = eggPrefab;

            if (prefab == null)
                return null;

            if (!pools.TryGetValue(prefab, out Queue<GameObject> queue))
            {
                queue = new Queue<GameObject>();
                pools.Add(prefab, queue);
            }

            GameObject egg = queue.Count > 0
                ? queue.Dequeue()
                : CreateEgg(prefab);

            egg.SetActive(true);
            return egg;
        }

        public void ReturnEgg(GameObject egg)
        {
            if (egg == null)
                return;

            if (!prefabByEgg.TryGetValue(egg, out GameObject prefab))
            {
                Debug.LogWarning("EggPool: ovo desconhecido!");
                return;
            }

            egg.SetActive(false);
            egg.transform.SetParent(transform);

            if (!pools.TryGetValue(prefab, out Queue<GameObject> queue))
            {
                queue = new Queue<GameObject>();
                pools.Add(prefab, queue);
            }

            queue.Enqueue(egg);
        }
    }
}
