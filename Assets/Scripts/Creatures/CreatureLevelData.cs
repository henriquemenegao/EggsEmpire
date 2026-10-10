using System.Globalization;
using System.Numerics;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(
        fileName = "CreatureLevel",
        menuName = "EggsEmpire/Creature Level"
    )]
    public class CreatureLevelData : ScriptableObject
    {
        [Header("Identificação")]
        [Min(1)]
        public int level = 1;

        public string creatureName;

        [Header("Visual")]
        public GameObject prefab;
        public GameObject eggPrefab;

        [Header("Produção")]
        [SerializeField]
        private string eggsPerCycleValue = "1";

        public BigInteger EggsPerCycle
        {
            get
            {
                if (BigInteger.TryParse(
                        eggsPerCycleValue,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out BigInteger value) &&
                    value > BigInteger.Zero)
                {
                    return value;
                }

                Debug.LogWarning(
                    $"CreatureLevelData: produção inválida no nível {level}."
                );

                return BigInteger.One;
            }
        }
    }
}