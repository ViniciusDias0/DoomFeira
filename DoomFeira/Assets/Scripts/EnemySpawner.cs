using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuração de Inimigos")]
    public GameObject[] enemyPrefabs;

    // --- MUDANÇA: De raio para dimensões de uma caixa ---
    [Header("Área de Spawn (Paralelepípedo)")]
    public Vector3 spawnAreaSize = new Vector3(40, 1, 40); // Largura(X), Altura(Y), Comprimento(Z)

    [Header("Zona de Segurança do Jogador")]
    public Transform playerTransform;
    public float playerSafeZoneRadius = 7f;

    [Header("Dificuldade Progressiva")]
    public float initialSpawnInterval = 4.0f;
    public float minimumSpawnInterval = 0.5f;
    public float intervalDecreaseRate = 0.02f;
    public int initialSpawnCount = 1;
    public int maxSpawnCount = 10;
    public float timeToIncreaseCount = 30.0f;

    private float currentSpawnInterval;
    private int currentSpawnCount;

    // --- ADIÇÃO: A Função OnDrawGizmos ---
    // Esta função é chamada pela Unity apenas no Editor, e é onde desenhamos os Gizmos.
    void OnDrawGizmosSelected()
    {
        // Define a cor do Gizmo para a área de spawn
        Gizmos.color = new Color(0, 1, 0, 0.5f); // Verde semitransparente
        // Desenha um cubo (paralelepípedo) com o centro na posição do spawner e com o tamanho definido
        Gizmos.DrawCube(transform.position, spawnAreaSize);

        // Desenha a zona de segurança do jogador se ele estiver definido
        if (playerTransform != null)
        {
            // Define a cor do Gizmo para a zona de segurança
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Vermelho semitransparente
            // Desenha uma esfera na posição do jogador com o raio da zona de segurança
            Gizmos.DrawSphere(playerTransform.position, playerSafeZoneRadius);
        }
    }


    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentSpawnCount = initialSpawnCount;

        if (playerTransform == null)
        {
            Debug.LogError("Referência do Jogador (PlayerTransform) não foi definida no EnemySpawner!", this.gameObject);
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

    // ... As suas coroutines SpawnEnemiesRoutine e IncreaseDifficultyRoutine permanecem exatamente as mesmas ...

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


    // --- FUNÇÃO MODIFICADA PARA SPAWN EM CAIXA ---
    void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0 || playerTransform == null) return;

        Vector3 spawnPosition;
        int attempts = 0;

        do
        {
            // Calcula um ponto aleatório dentro do paralelepípedo
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);

            // A posição do spawn é o centro do spawner + o desvio aleatório
            spawnPosition = transform.position + new Vector3(randomX, 0, randomZ);

            attempts++;

            if (attempts > 20)
            {
                Debug.LogWarning("Não foi possível encontrar um local de spawn seguro. Pulando este spawn.");
                return;
            }
        }
        while (Vector3.Distance(spawnPosition, playerTransform.position) < playerSafeZoneRadius);

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }
}