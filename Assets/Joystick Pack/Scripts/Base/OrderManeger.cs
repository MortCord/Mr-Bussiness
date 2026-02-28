using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    // Singleton - dostęp do menedżera z dowolnego skryptu
    public static OrderManager Instance;

    // Lista wszystkich aktywnych klientów w lokalu
    public List<NPC> activeCustomers = new List<NPC>();

    private void Awake()
    {
        // Inicjalizacja instancji Singleton
        Instance = this;
    }

    // Metoda sprawdzająca, czy jakikolwiek klient zamówił konkretny typ napoju
    public bool IsDrinkOrdered(GameItemType type)
    {
        foreach (NPC customer in activeCustomers)
        {
            // Sprawdzamy, czy obiekt klienta istnieje i czy jego zamówienie pasuje
            if (customer != null && customer.orderedDrink == type)
            {
                return true; // Zamówienie znalezione
            }
        }
        return false; // Nikt nie zamówił tego napoju
    }
}