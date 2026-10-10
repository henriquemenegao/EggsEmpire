using System.Globalization;
using System.Numerics;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(
        fileName = "CreaturePurchaseUpgrade",
        menuName = "EggsEmpire/Creature Purchase Upgrade"
    )]
    public class CreaturePurchaseUpgradeData : ScriptableObject
    {
        [Header("Upgrade")]
        [SerializeField, Range(2, 18)]
        private int targetLevel = 2;

        [Header("Requisitos")]
        [SerializeField]
        private string requiredEggsPerMinute = "0";

        [SerializeField]
        private string cost = "0";

        public int TargetLevel => targetLevel;

        public BigInteger RequiredEggsPerMinute =>
            ParseNonNegative(requiredEggsPerMinute);

        public BigInteger Cost =>
            ParseNonNegative(cost);

        private BigInteger ParseNonNegative(string value)
        {
            if (BigInteger.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out BigInteger result) &&
                result >= BigInteger.Zero)
            {
                return result;
            }

            Debug.LogError(
                $"CreaturePurchaseUpgradeData: valor inválido no upgrade para nível {targetLevel}."
            );

            return BigInteger.Zero;
        }
    }
}