
using UnityEngine;

namespace Factory.Conveyor
{
    public class FactoryConveyorGenerator : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject conveyorPrefab;

        [Header("Configuração da fábrica")]
        [SerializeField] private int rows = 10;
        [SerializeField] private int columnsPerSide = 5;
        [SerializeField] private float slotSpacing = 3.5f;
        [SerializeField] private float centralCorridorWidth = 4f;

        [Header("Esteiras")]
        [SerializeField] private float conveyorWidth = 0.6f;
        [SerializeField] private float conveyorHeight = -0.15f;

        [ContextMenu("Gerar Esteiras")]
        public void GenerateConveyors()
        {
            if (conveyorPrefab == null)
            {
                Debug.LogError("Configure o Conveyor Prefab!");
                return;
            }

            // Remove somente as esteiras horizontais geradas.
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.name.StartsWith("HorizontalConveyor_"))
                    DestroyImmediate(child);
            }

            float length = columnsPerSide * slotSpacing;

            float innerSlotX = (slotSpacing + centralCorridorWidth) / 2f;

            float centerX = innerSlotX
                            + (columnsPerSide - 1) * slotSpacing / 2f
                            - 1f;

            for (int row = 0; row < rows; row++)
            {
                // Uma esteira atrás de cada fileira, no corredor seguinte.
                float z = -row * slotSpacing - slotSpacing / 2f;

                CreateConveyor("Left", row, -centerX, z, length);
                CreateConveyor("Right", row, centerX, z, length);
            }

            Debug.Log($"Esteiras geradas: {rows * 2}");
        }

        private void CreateConveyor(
            string side,
            int row,
            float x,
            float z,
            float length)
        {
            GameObject conveyor = Instantiate(conveyorPrefab, transform);

            conveyor.name = $"HorizontalConveyor_{side}_{row + 1:00}";

            conveyor.transform.localPosition =
                new Vector3(x, conveyorHeight, z);

            conveyor.transform.localRotation = side == "Right"
                ? Quaternion.Euler(0f, 180f, 0f)
                : Quaternion.identity;

            conveyor.transform.localScale = Vector3.one;
            conveyor.transform.localScale = conveyorPrefab.transform.localScale;
            CenterConveyorVisual(conveyor);
        }
        
        private void CenterConveyorVisual(GameObject conveyor)
        {
            Renderer[] renderers =
                conveyor.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
                return;

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            Vector3 offset = conveyor.transform.position - bounds.center;

            foreach (Transform child in conveyor.transform)
                child.position += new Vector3(offset.x, 0f, offset.z);
        }
    }
}
