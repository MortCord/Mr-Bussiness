using UnityEngine;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [Header("Zamówienie")]
    public GameItemType orderedDrink;
    private bool isRegistered = false;

    [Header("Ustawienia Wyglądu")]
    public GameObject orderUI; // Jeśli masz ikonkę nad głową

    private void Start()
    {
        // Додаємо цього клієнта в список активних замовлень
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.activeCustomers.Add(this);
        }
    }

    private void LateUpdate()
    {
        // Blokada rotacji: NPC zawsze stoi pionowo na osiach X i Z
        // To naprawia błąd z "wywracaniem się" klienta
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void RegisterInManager()
    {
        if (OrderManager.Instance != null && !isRegistered)
        {
            OrderManager.Instance.activeCustomers.Add(this);
            isRegistered = true;
        }
    }

    private void OnDestroy()
    {
        // Usuwanie z listy przy znikaniu
        if (OrderManager.Instance != null && isRegistered)
        {
            OrderManager.Instance.activeCustomers.Remove(this);
        }
    }

    // Wywoływane przez Spawner przy stworzeniu klienta
    // W skrypcie NPC.cs popraw metodę GenerateOrder:

    public void GenerateOrder(bool canCider, bool canMojito, bool canWine)
    {
        List<GameItemType> availableDrinks = new List<GameItemType>();
        if (canCider) availableDrinks.Add(GameItemType.Cider);
        if (canMojito) availableDrinks.Add(GameItemType.Mojito);
        if (canWine) availableDrinks.Add(GameItemType.Wine);

        if (availableDrinks.Count > 0)
        {
            // Poprawiony Random.Range - wybiera z listy dostępnych
            int randomIndex = Random.Range(0, availableDrinks.Count);
            orderedDrink = availableDrinks[randomIndex];
        }
        else
        {
            orderedDrink = GameItemType.Cider;
        }

        Debug.Log("Klient zamówił: " + orderedDrink);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();

            // SPRAWDZAMY: 1. Czy gracz ma coś w rękach? 2. Czy to mój napój?
            if (inv != null && inv.hasItem && inv.currentHoldingType == orderedDrink)
            {
                // Tylko jeśli oba warunki są prawdziwe, kupujemy!
                CompleteSale(inv);
            }
            else if (inv != null && inv.hasItem)
            {
                Debug.Log($"[NPC] Nie chcę tego! Mam ochotę na {orderedDrink}.");
            }
            // Jeśli gracz nie ma nic w rękach - NPC po prostu ignoruje go i czeka
        }
    }

    private void CompleteSale(PlayerInventory inv)
    {
        Debug.Log("[NPC] Sprzedaż udana! Dziękuję.");

        // Dodajemy pieniądze przez Twój MoneyManeger
        if (MoneyManeger.Instance != null)
        {
            MoneyManeger.Instance.AddMoney(15); // Cena za drink
        }

        // Zabieramy przedmiot z ręki gracza
        inv.ClearHand();

        // Klient odchodzi (usuwamy go ze sceny)
        Destroy(gameObject);
    }
}