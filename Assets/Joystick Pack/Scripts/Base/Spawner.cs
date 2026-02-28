using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Ustawienia Klienta")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    public int maxCustomers = 2; // Zwiększ do 2 dla testu

    [Header("Beczki (Referencje)")]
    public Barrel ciderBarrel;
    public Barrel MojitoBarrel;
    public Barrel WinoBarrel;

    private void Start()
    {
        if (OrderManager.Instance == null)
        {
            Debug.LogError("[Spawner] ERROR: Nie znaleziono OrderManager na scenie! Postaw pusty obiekt i daj mu skrypt OrderManager.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[Spawner] ERROR: Nie przypisano Spawn Point w Inspektorze!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            int currentCount = 0;
            if (OrderManager.Instance != null)
            {
                currentCount = OrderManager.Instance.activeCustomers.Count;
            }

            // Debug, który pomoże Ci zrozumieć dlaczego nie spawnuje
            Debug.Log($"[Spawner] Aktywni klienci: {currentCount} / {maxCustomers}");

            if (currentCount < maxCustomers)
            {
                Debug.Log("[Spawner] Przygotowanie do spawnu...");
                yield return new WaitForSeconds(spawnInterval);
                SpawnCustomer();
            }

            yield return new WaitForSeconds(2f); // Sprawdzaj co 2 sekundy
        }
    }

    public void SpawnCustomer()
    {
        if (customerPrefab == null) return;

        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        NPC npcScript = newCustomer.GetComponent<NPC>();

        if (npcScript != null)
        {
            // Перевіряємо стан кожної бочки
            bool canCider = (ciderBarrel != null && ciderBarrel.isUnlocked);
            bool canMojito = (MojitoBarrel != null && MojitoBarrel.isUnlocked);
            bool canWino = (WinoBarrel != null && WinoBarrel.isUnlocked);

            // Якщо гра тільки почалася і нічого не куплено, даємо хоча б Сидр
            if (!canCider && !canMojito && !canWino) canCider = true;

            // ТЕПЕР ПЕРЕДАЄМО ПРАВИЛЬНІ ЗМІННІ
            npcScript.GenerateOrder(canCider, canMojito, canWino);

            Debug.Log($"[Spawner] Nowy klient! Może kupić: Cider({canCider}), Mojito({canMojito}), Wino({canWino})");
        }
    }
}