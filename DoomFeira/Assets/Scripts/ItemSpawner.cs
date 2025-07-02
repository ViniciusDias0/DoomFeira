using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // --- CORREÇÃO: A VARIÁVEL DE PREFABS VOLTOU! ---
    [Header("Itens para Spawnar")]
    public GameObject[] itemPrefabs; // Arraste seus prefabs de Cura e Armadura aqui

    [Header("Configuração de Spawn")]
    public float spawnInterval = 10f;
    public Vector3 spawnAreaSize = new Vector3(20f, 2f, 20f);

    [Header("Controle de Colisão e Itens")]
    public int maxItemsOnMap = 5;
    public LayerMask obstacleLayer;
    public float itemCheckRadius = 1f;
    public int maxSpawnAttempts = 20; // Aumentei um pouco o número de tentativas

    // O contador agora é privado, pois sua lógica é interna ao script.
    private int currentItemCount = 0;

    void Start()
    {
        // Garante que os prefabs foram atribuídos no Inspector.
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogError("ItemSpawner: Nenhum prefab de item foi atribuído no Inspector!");
            this.enabled = false; // Desativa o spawner se não houver itens para criar.
            return;
        }
        StartCoroutine(SpawnItemsRoutine());
    }

    IEnumerator SpawnItemsRoutine()
    {
        while (true)
        {
            // O contador de itens será atualizado antes de cada tentativa de spawn.
            currentItemCount = FindObjectsByType<Powerup>(FindObjectsSortMode.None).Length;

            if (currentItemCount < maxItemsOnMap)
            {
                SpawnRandomItem();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomItem()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
            float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);

            Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, randomZ);

            if (!Physics.CheckSphere(spawnPosition, itemCheckRadius, obstacleLayer))
            {
                int randomIndex = Random.Range(0, itemPrefabs.Length);
                GameObject itemToSpawn = itemPrefabs[randomIndex];

                Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
                // Não precisamos mais incrementar manualmente, a rotina já atualiza o contador.
                return;
            }
        }

        Debug.LogWarning("ItemSpawner: Não foi possível encontrar um local válido para spawnar o item.");
    }

    // A função Update() não é mais necessária, a contagem foi movida para a Coroutine.

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position, spawnAreaSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}