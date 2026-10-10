using UnityEngine;

namespace Factory
{
    public class CreatureSlot : MonoBehaviour
    {
        [SerializeField] private int row;
        [SerializeField] private int column;

        private GameObject currentCreature;

        public int Row => row;
        public int Column => column;

        public bool IsOccupied => currentCreature != null;
        public GameObject CurrentCreature => currentCreature;

        public void Initialize(int rowIndex, int columnIndex)
        {
            row = rowIndex;
            column = columnIndex;
        }

        
        public bool PlaceCreature(GameObject creature)
        {
            if (IsOccupied || creature == null)
                return false;

            currentCreature = creature;

            // Mantém a criatura fora da hierarquia do Cube,
            // evitando herdar sua escala.
            creature.transform.SetParent(null);

            // Posiciona a criatura sobre o slot.
            creature.transform.position =
                transform.position + Vector3.up * 0.1f;

            creature.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            return true;
        }


        public void Clear()
        {
            currentCreature = null;
        }
    }
}