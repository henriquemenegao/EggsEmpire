using UnityEngine;

namespace Factory
{
    public class ConveyorBeltAnimation : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private Renderer beltRenderer;
        [SerializeField] private float speed = 2.078f;
        [SerializeField] private Vector2 direction = Vector2.right;

        private Material beltMaterial;
        private Vector2 offset;

        private void Start()
        {
            if (beltRenderer == null)
                return;

            beltMaterial = beltRenderer.material;
        }

        private void Update()
        {
            if (beltMaterial == null)
                return;

            offset += direction.normalized * speed * Time.deltaTime;
            beltMaterial.mainTextureOffset = offset;
        }

        private void OnDestroy()
        {
            if (beltMaterial != null)
                Destroy(beltMaterial);
        }
    }
}