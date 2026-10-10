
using UnityEngine;

namespace Factory
{
    public class FactoryGrid : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject slotPrefab;

        [Header("Dimensões")]
        [SerializeField] private int rows = 10;
        [SerializeField] private int columnsPerSide = 5;

        [Header("Espaçamento")]
        [SerializeField] private float slotSpacing = 2.3f;
        [SerializeField] private float centralCorridorWidth = 3f;

        [ContextMenu("Gerar Grid")]
        public void GenerateGrid()
        {
            if (slotPrefab == null)
            {
                Debug.LogError("Configure o Slot Prefab!");
                return;
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            int totalColumns = columnsPerSide * 2;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    float x = (col - (totalColumns - 1) / 2f)
                              * slotSpacing;

                    x += col < columnsPerSide
                        ? -centralCorridorWidth / 2f
                        : centralCorridorWidth / 2f;

                    float z = -row * slotSpacing;

                    GameObject slot = Instantiate(
                        slotPrefab,
                        transform
                    );

                    slot.name = $"Slot_{row:D2}_{col:D2}";

                    slot.transform.localPosition =
                        new Vector3(x, 0, z);

                    slot.transform.localRotation =
                        Quaternion.identity;

                    CreatureSlot creatureSlot =
                        slot.GetComponent<CreatureSlot>();

                    if (creatureSlot == null)
                    {
                        creatureSlot =
                            slot.AddComponent<CreatureSlot>();
                    }

                    creatureSlot.Initialize(row, col);
                }
            }

            Debug.Log(
                $"Grid gerado: {rows * totalColumns} slots."
            );
        }
    }
}
