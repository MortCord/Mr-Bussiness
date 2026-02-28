using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SellerNPC : MonoBehaviour
{
    [Header("Ustawienia Napoju")]
    public GameItemType drinkType = GameItemType.Cider;
    public int stationLevel = 1; // Poziom stacji (1-10)
    public bool isUnlocked = true;

    [Header("Referencje")]
    public GameObject cupVisual;
    public GameObject progressCanvas;
    public Image progressCircle;
    public float baseBrewTime = 3.0f; // Czas podstawowy

    private bool isBrewing = false;

    // Obliczanie czasu gotowania: im wyższy poziom, tym szybciej (min. 0.5s)
    public float CurrentBrewTime => Mathf.Max(0.5f, baseBrewTime - (stationLevel * 0.25f));

    // Cena ulepszenia: 3 monety na 2 lvl, potem 5 monet
    public int UpgradeCost => (stationLevel == 1) ? 3 : 5;

    void Start()
    {
        UpdateVisuals();
        if (progressCanvas) progressCanvas.SetActive(false);
    }

    // Wywoływane przez gracza lub beczkę (Trigger)
    public void StartBrewing()
    {
        if (isUnlocked && !isBrewing)
        {
            StartCoroutine(BrewRoutine());
        }
    }

    private IEnumerator BrewRoutine()
    {
        isBrewing = true;
        if (progressCanvas) progressCanvas.SetActive(true);

        float timer = 0;
        float timeToWait = CurrentBrewTime;

        while (timer < timeToWait)
        {
            timer += Time.deltaTime;
            if (progressCircle) progressCircle.fillAmount = timer / timeToWait;
            yield return null;
        }

        // Po ugotowaniu, gracz dostaje przedmiot (logika wydawania)
        isBrewing = false;
        if (progressCanvas) progressCanvas.SetActive(false);
        Debug.Log("Napój gotowy na poziomie: " + stationLevel);
    }

    // Metoda do ulepszania stacji (podepnij pod przycisk w UI)
    public void UpgradeStation()
    {
        if (stationLevel < 10)
        {
            if (MoneyManeger.Instance.TrySpendMoney(UpgradeCost))
            {
                stationLevel++;
                Debug.Log("Ulepszono stację do poziomu: " + stationLevel);
            }
        }
    }

    void UpdateVisuals()
    {
        if (cupVisual) cupVisual.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();
            // Jeśli gracz nie ma nic w ręku, daj mu gotowy napój
            if (inv != null && inv.currentHoldingType == GameItemType.None)
            {
                inv.PickUpItem(cupVisual, drinkType);
            }
        }
    }
}