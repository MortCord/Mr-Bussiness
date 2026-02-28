using UnityEngine;
using TMPro;

public class StationUnlocker : MonoBehaviour
{
    [Header("Ustawienia odblokowania")]
    public Barrel barrelToUnlock; // Beczka, którą chcemy odblokować (np. Mojito)
    public Barrel requiredBarrel; // Beczka, która musi być rozwinięta (np. Cider)

    public int unlockPrice = 50; // Cena odblokowania
    public int requiredLevel = 2; // Wymagany poziom poprzedniej beczki (Cider)

    [Header("UI Elementy")]
    public GameObject unlockCanvas; // Panel z przyciskiem "Odblokuj" (znajduje się nad beczką)
    public TextMeshProUGUI priceLabel; // Tekst z ceną odblokowania

    private void Start()
    {
        // Ustawienie tekstu z ceną na starcie gry
        if (priceLabel) priceLabel.text = unlockPrice + " $";

        // Jeśli beczka jest już odblokowana, od razu ukrywamy panel zakupu
        if (barrelToUnlock != null && barrelToUnlock.isUnlocked)
        {
            if (unlockCanvas) unlockCanvas.SetActive(false);
        }
    }

    // Metoda przypisywana do przycisku (Button -> OnClick)
    public void ActionUnlock()
    {
        if (barrelToUnlock == null || requiredBarrel == null)
        {
            Debug.LogError("[Unlocker] Brak przypisanej beczki w Inspektorze!");
            return;
        }

        // 1. Sprawdzamy, czy poprzednia beczka ma odpowiedni poziom
        if (requiredBarrel.stationLevel >= requiredLevel)
        {
            // 2. Sprawdzamy, czy gracz ma wystarczająco pieniędzy i pobieramy je
            if (MoneyManeger.Instance != null && MoneyManeger.Instance.TrySpendMoney(unlockPrice))
            {
                // Odblokowujemy beczkę z Mojito
                barrelToUnlock.isUnlocked = true;

                // Aktualizujemy jej UI (zniknie kłódka, pojawi się przycisk upgrade)
                barrelToUnlock.UpdateUI();

                Debug.Log($"[Unlocker] Sukces! Odblokowano: {barrelToUnlock.drinkType}");

                // Ukrywamy panel z przyciskiem "Odblokuj"
                if (unlockCanvas) unlockCanvas.SetActive(false);
                else gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("[Unlocker] Gracz ma za mało pieniędzy!");
            }
        }
        else
        {
            Debug.LogWarning($"[Unlocker] Zbyt niski poziom! Wymagany poziom {requiredLevel} na stacji {requiredBarrel.drinkType}.");
        }
    }
}