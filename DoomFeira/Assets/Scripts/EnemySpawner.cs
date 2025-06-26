using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    // Agora é um array para guardar todos os tipos de inimigos
    public GameObject[] enemyPrefabs;
    public float spawnRadius = 20f;
    public float spawnInterval = 2f;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 1. Escolhe um inimigo aleatório do array
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[randomIndex];

            // 2. Gera uma posição aleatória
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);

            // 3. Cria o inimigo escolhido
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}