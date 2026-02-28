using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Barrel : MonoBehaviour
{
    [Header("Ustawienia Stacji")]
    public GameItemType drinkType;
    public int stationLevel = 1;
    public bool isUnlocked = false;
    public int maxLevel = 15;

    [Header("UI Elementy")]
    public GameObject unlockButton; // Przypisz "Button" (ten od kupowania)
    public GameObject upgradeButton; // Przypisz "Button (1)" (ten od ulepszeń)
    public TextMeshProUGUI unlockPriceText; // Tekst ceny na przycisku odblokowania
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI priceText;  // Tekst ceny na przycisku ulepszenia
    public Image progressCircle;

    [Header("Modele i Prefaby")]
    public GameObject cupVisual;
    public GameObject cupPrefab;

    [Header("Balans")]
    public float baseBrewTime = 4f;
    public int baseUpgradePrice = 8;

    [Header("Interakcja")]
    public float interactionRadius = 2.0f;
    public LayerMask playerLayer;

    private Coroutine currentBrewCoroutine;
    private bool isBrewing = false;
    private bool hasFinishedDrink = false;
    private PlayerInventory playerInRange = null;

    public float CurrentBrewTime => Mathf.Max(0.5f, baseBrewTime - (stationLevel * 0.3f));
    public int CurrentUpgradePrice => baseUpgradePrice + (stationLevel * 5);

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (!isUnlocked) return;

        // Szukamy gracza w promieniu
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, playerLayer);

        if (colliders.Length > 0)
        {
            if (playerInRange == null)
            {
                playerInRange = colliders[0].GetComponent<PlayerInventory>();
            }

            // Sprawdzamy czy DOKŁADNIE ten napój jest zamówiony
            bool isOrdered = OrderManager.Instance != null && OrderManager.Instance.IsDrinkOrdered(drinkType);

            if (playerInRange != null && !playerInRange.hasItem && isOrdered)
            {
                if (!isBrewing && !hasFinishedDrink)
                {
                    if (currentBrewCoroutine != null) StopCoroutine(currentBrewCoroutine);
                    currentBrewCoroutine = StartCoroutine(BrewRoutine());
                }
                else if (hasFinishedDrink)
                {
                    GiveToPlayer(playerInRange);
                }
            }
        }
        else
        {
            ResetBrewing();
        }
    }

    private IEnumerator BrewRoutine()
    {
        isBrewing = true;
        hasFinishedDrink = false;
        if (cupVisual) cupVisual.SetActive(false);

        float timer = 0;
        while (timer < CurrentBrewTime)
        {
            timer += Time.deltaTime;
            if (progressCircle) progressCircle.fillAmount = timer / CurrentBrewTime;
            yield return null;
        }

        isBrewing = false;
        hasFinishedDrink = true;
        if (progressCircle) progressCircle.fillAmount = 1f;
        if (cupVisual) cupVisual.SetActive(true);
        Debug.Log($"[Barrel] {drinkType} gotowy!");
    }

    private void ResetBrewing()
    {
        playerInRange = null;
        if (isBrewing)
        {
            if (currentBrewCoroutine != null) StopCoroutine(currentBrewCoroutine);
            isBrewing = false;
            if (progressCircle) progressCircle.fillAmount = 0f;
            Debug.Log("[Barrel] Przerwano nalewanie.");
        }
    }

    private void GiveToPlayer(PlayerInventory inv)
    {
        if (cupPrefab == null) return;

        inv.PickUpItem(cupPrefab, drinkType); 
        hasFinishedDrink = false;
        if (cupVisual) cupVisual.SetActive(false);
        if (progressCircle) progressCircle.fillAmount = 0f;
    }

  

    public void BuyButton() // Podepnij pod pierwszy przycisk
    {
        if (MoneyManeger.Instance.TrySpendMoney(10))
        {
            isUnlocked = true;
            UpdateUI();
        }
    }

    // Metoda wywoływana po kliknięciu przycisku ulepszenia (podepnij w Inspektorze pod Button)
    public void UpgradeButton()
    {
        // Sprawdzamy, czy stacja jest odblokowana i czy nie osiągnęliśmy max poziomu
        if (!isUnlocked || stationLevel >= maxLevel) return;

        // Próbujemy wydać pieniądze na ulepszenie
        if (MoneyManeger.Instance.TrySpendMoney(CurrentUpgradePrice))
        {
            stationLevel++; // Zwiększamy poziom
            UpdateUI();     // KLUCZOWE: Natychmiast odświeżamy teksty na przyciskach
            Debug.Log($"[Barrel] Ulepszono {drinkType} do poziomu {stationLevel}. Nowa cena: {CurrentUpgradePrice}$");
        }
    }

    // Metoda aktualizująca wszystkie elementy interfejsu stacji
    public void UpdateUI()
    {
        // Kontrola widoczności przycisków w zależności od statusu odblokowania
        if (unlockButton) unlockButton.SetActive(!isUnlocked);
        if (upgradeButton) upgradeButton.SetActive(isUnlocked);

        // Aktualizacja tekstów poziomu i ceny
        if (levelText) levelText.text = $"Lvl {stationLevel}";

        // To ten fragment odpowiada za zmianę ceny na przycisku
        if (priceText) priceText.text = $"{CurrentUpgradePrice}$";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}