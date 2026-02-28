using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("Ustawienie pieniędzy")]
    public TextMeshProUGUI moneyText;
    public int money = 0;

    [Header("Ustawienie ręki")]
    public Transform handPivot;

    [Header("Ustawienie wyglądu przedmiota")]
    public Vector3 itemScale = new Vector3(1f, 1f, 1f);
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Potoczny stan")]
    public GameItemType currentHoldingType = GameItemType.None;
    public GameObject currentItemModel; // To musi być puste w Inspektorze!

    public bool hasItem => currentHoldingType != GameItemType.None;

    void Start()
    {
        // Fix: Jeśli w slocie był prefab, odpinamy go, by go nie usunąć
        if (currentItemModel != null && !currentItemModel.scene.IsValid())
        {
            currentItemModel = null;
        }
        UpdateMoneyUI();
    }

    public void PickUpItem(GameObject prefab, GameItemType type)
    {
        if (prefab == null || handPivot == null) return;

        ClearHand();

        // Tworzymy NOWĄ KOPIĘ (Instancję) - to jest bezpieczne do usunięcia
        currentItemModel = Instantiate(prefab, handPivot.position, handPivot.rotation);

        currentItemModel.transform.SetParent(handPivot);
        currentItemModel.transform.localPosition = positionOffset;
        currentItemModel.transform.localRotation = Quaternion.Euler(rotationOffset);
        currentItemModel.transform.localScale = itemScale;

        currentHoldingType = type;
    }

    public void ClearHand()
    {
        if (currentItemModel != null)
        {
            // Sprawdzamy, czy to nie jest plik z folderu (Asset)
            if (currentItemModel.scene.IsValid())
            {
                Destroy(currentItemModel);
            }
        }

        currentItemModel = null;
        currentHoldingType = GameItemType.None;
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null) moneyText.text = money.ToString() + " $";
    }
}