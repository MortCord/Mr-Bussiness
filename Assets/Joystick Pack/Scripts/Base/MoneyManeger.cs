using UnityEngine;
using TMPro;

public class MoneyManeger : MonoBehaviour
{
    public static MoneyManeger Instance;

    [Header("Ustawienie Balansu")]
    public int currentMoney = 0;

    [Header("UI element")]
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        // Налаштування Singleton
        if (Instance == null)
        {
            Instance = this;
            // Якщо цей менеджер має зберігатися між рівнями:
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    // Метод для додавання грошей (викликається з NPC або інших скриптів)
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log("Dodano pieniądze! Aktyalny balans: " + currentMoney);
        UpdateUI();
    }

    // Метод для зняття грошей (наприклад, для покупок)
    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void UpdateUI()
    {
        if (moneyText != null)
        {
            // Викликаємо функцію форматування для тексту
            moneyText.text = FormatMoney(currentMoney) + " $";
        }
    }

    // Нова функція для скорочення великих чисел
    private string FormatMoney(int value)
    {
        if (value >= 1000000000) // Мільярд
            return (value / 1000000000f).ToString("F1") + "B";
        if (value >= 1000000)    // Мільйон
            return (value / 1000000f).ToString("F1") + "M";
        if (value >= 1000)       // Тисяча
            return (value / 1000f).ToString("F1") + "k";

        return value.ToString();
    }
}