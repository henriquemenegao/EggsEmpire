using System.Collections;
using UnityEngine;

public class CreatureProducer : MonoBehaviour
{
    [Header("Produção")]
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private float productionInterval = 5f;
    [SerializeField] private long eggsPerCycle = 1;

    [Header("Transporte")]
    [SerializeField] private ConveyorBelt conveyor;

    private IEnumerator Start()
    {
        if (eggPrefab == null || conveyor == null)
        {
            Debug.LogError("Configure o ovo e a esteira!", this);
            yield break;
        }

        while (true)
        {
            yield return new WaitForSeconds(productionInterval);
            ProduceEgg();
        }
    }

    
    private void ProduceEgg()
    {
        GameObject egg = Instantiate(eggPrefab);

        EggBatch batch = egg.GetComponent<EggBatch>();

        if (batch == null)
            batch = egg.AddComponent<EggBatch>();

        batch.Initialize(eggsPerCycle);

        conveyor.Transport(egg);

        Debug.Log($"Produzido um lote de {eggsPerCycle:N0} ovos!");
    }

}