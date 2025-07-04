using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configura��o de Inimigos")]
    public GameObject[] enemyPrefabs;
    public float spawnRadius = 20f;

    [Header("Zona de Seguran�a do Jogador")]
    public Transform playerTransform; // Arraste o objeto do jogador aqui
    public float playerSafeZoneRadius = 7f; // Inimigos n�o nascer�o dentro deste raio

    [Header("Dificuldade Progressiva")]
    public float initialSpawnInterval = 4.0f;
    public float minimumSpawnInterval = 0.5f;
    public float intervalDecreaseRate = 0.02f;
    public int initialSpawnCount = 1;
    public int maxSpawnCount = 10;
    public float timeToIncreaseCount = 30.0f;

    private float currentSpawnInterval;
    private int currentSpawnCount;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentSpawnCount = initialSpawnCount;

        if (playerTransform == null)
        {
            Debug.LogError("Refer�ncia do Jogador (PlayerTransform) n�o foi definida no EnemySpawner!", this.gameObject);
        }

        StartCoroutine(SpawnEnemiesRoutine());
        StartCoroutine(IncreaseDifficultyRoutine());
    }

    void Update()
    {
        if (currentSpawnInterval > minimumSpawnInterval)
        {
            currentSpawnInterval -= intervalDecreaseRate * Time.deltaTime;
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);
            for (int i = 0; i < currentSpawnCount; i++)
            {
                SpawnSingleEnemy();
            }
        }
    }

    IEnumerator IncreaseDifficultyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToIncreaseCount);
            if (currentSpawnCount < maxSpawnCount)
            {
                currentSpawnCount++;
            }
        }
    }

    // --- FUN��O MODIFICADA ---
    void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0 || playerTransform == null) return;

        Vector3 spawnPosition;
        int attempts = 0; // Um contador para evitar loops infinitos

        // Tenta encontrar uma posi��o v�lida por at� 20 vezes
        do
        {
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);
            attempts++;

            // Se tentarmos demais e n�o acharmos um lugar, desiste por este frame
            if (attempts > 20)
            {
                Debug.LogWarning("N�o foi poss�vel encontrar um local de spawn seguro. Pulando este spawn.");
                return;
            }
        }
        // A condi��o do loop: continue tentando enquanto a dist�ncia for MENOR que a zona de seguran�a
        while (Vector3.Distance(spawnPosition, playerTransform.position) < playerSafeZoneRadius);

        // Se encontrou um local v�lido, cria o inimigo
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }
}