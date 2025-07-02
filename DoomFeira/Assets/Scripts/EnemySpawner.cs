using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configura��o de Inimigos")]
    public GameObject[] enemyPrefabs; // Array para todos os tipos de inimigos
    public float spawnRadius = 20f;

    [Header("Dificuldade Progressiva")]
    [Tooltip("O tempo inicial entre cada onda de inimigos.")]
    public float initialSpawnInterval = 4.0f; // Come�a mais devagar

    [Tooltip("O tempo m�nimo que o intervalo pode atingir. Evita spawns absurdos.")]
    public float minimumSpawnInterval = 0.5f;

    [Tooltip("Qu�o r�pido o tempo de spawn diminui a cada segundo.")]
    public float intervalDecreaseRate = 0.02f;

    [Space]
    [Tooltip("Quantos inimigos nascem no in�cio.")]
    public int initialSpawnCount = 1;

    [Tooltip("O n�mero m�ximo de inimigos que podem nascer de uma s� vez.")]
    public int maxSpawnCount = 10;

    [Tooltip("A cada quantos segundos o n�mero de inimigos por onda aumenta.")]
    public float timeToIncreaseCount = 30.0f;

    // Vari�veis privadas para controlar o estado atual
    private float currentSpawnInterval;
    private int currentSpawnCount;

    void Start()
    {
        // Inicializa os valores de dificuldade
        currentSpawnInterval = initialSpawnInterval;
        currentSpawnCount = initialSpawnCount;

        // Inicia as duas rotinas que controlar�o a dificuldade
        StartCoroutine(SpawnEnemiesRoutine());
        StartCoroutine(IncreaseDifficultyRoutine());
    }

    void Update()
    {
        // Diminui continuamente o intervalo de spawn a cada frame, at� o limite m�nimo
        if (currentSpawnInterval > minimumSpawnInterval)
        {
            currentSpawnInterval -= intervalDecreaseRate * Time.deltaTime;
        }
    }

    // A rotina principal que spawna as ondas de inimigos
    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            // Espera pelo tempo ATUAL do intervalo, que est� sempre diminuindo
            yield return new WaitForSeconds(currentSpawnInterval);

            // Spawna uma "onda" de inimigos
            for (int i = 0; i < currentSpawnCount; i++)
            {
                SpawnSingleEnemy();
            }
        }
    }

    // Rotina separada que aumenta a dificuldade em "degraus"
    IEnumerator IncreaseDifficultyRoutine()
    {
        while (true)
        {
            // Espera pelo tempo definido para aumentar a contagem
            yield return new WaitForSeconds(timeToIncreaseCount);

            // Se ainda n�o atingimos o m�ximo, aumenta o n�mero de inimigos por onda
            if (currentSpawnCount < maxSpawnCount)
            {
                currentSpawnCount++;
                Debug.Log($"DIFICULDADE AUMENTOU! Agora nascem {currentSpawnCount} inimigos por onda.");
            }
        }
    }

    // Fun��o auxiliar para criar um �nico inimigo
    void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        // Escolhe um inimigo aleat�rio do array
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Gera uma posi��o aleat�ria
        Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);

        // Adiciona um pequeno desvio para que eles n�o nas�am exatamente no mesmo ponto
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        // Cria o inimigo escolhido
        Instantiate(enemyToSpawn, spawnPosition + offset, Quaternion.identity);
    }
}