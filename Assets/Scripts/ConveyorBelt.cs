using System.Collections;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Pontos da esteira")]
    [SerializeField] private Transform entryPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Configuração")]
    [SerializeField] private float speed = 2f;

    public void Transport(GameObject egg)
    {
        if (egg == null || entryPoint == null || exitPoint == null)
        {
            Debug.LogError("Esteira não configurada corretamente!", this);
            return;
        }

        egg.transform.position = entryPoint.position;
        StartCoroutine(MoveEgg(egg));
    }

    private IEnumerator MoveEgg(GameObject egg)
    {
        while (egg != null &&
               Vector3.Distance(egg.transform.position, exitPoint.position) > 0.01f)
        {
            egg.transform.position = Vector3.MoveTowards(
                egg.transform.position,
                exitPoint.position,
                speed * Time.deltaTime
            );

            yield return null;
        }

        
        if (egg != null)
        {
            EggBatch batch = egg.GetComponent<EggBatch>();

            if (batch != null && EconomyManager.Instance != null)
            {
                EconomyManager.Instance.SellEggs(batch.Quantity);
            }

            Destroy(egg);
        }

    }
}