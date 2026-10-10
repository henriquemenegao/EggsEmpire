using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Economia")]
    [SerializeField] private long coins = 0;

    [Header("Interface")]
    [SerializeField] private TMP_Text coinsText;

    public long Coins => coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateCoinsUI();
    }

    public void SellEggs(long quantity)
    {
        if (quantity <= 0)
            return;

        coins += quantity;

        UpdateCoinsUI();

        Debug.Log($"Venda: +{quantity:N0} moedas | Total: {coins:N0}");
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = $"{coins:N0} Moedas";
        }
    }
}