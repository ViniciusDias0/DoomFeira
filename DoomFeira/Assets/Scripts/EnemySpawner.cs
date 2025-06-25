using UnityEngine;
using System.Collections; // Necessário para usar Coroutines

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // O "molde" do inimigo que vamos criar
    public float spawnRadius = 20f; // O raio do círculo onde os inimigos podem nascer
    public float spawnInterval = 2f; // Intervalo de tempo entre cada nascimento

    void Start()
    {
        // Inicia a rotina de spawn
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // Loop infinito para continuar criando inimigos
        while (true)
        {
            // Espera pelo tempo definido em spawnInterval
            yield return new WaitForSeconds(spawnInterval);

            // Gera uma posição aleatória dentro de um círculo
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y); // Usamos Y para o eixo Z

            // Cria o inimigo na posição calculada
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}