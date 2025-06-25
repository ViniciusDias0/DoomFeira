using UnityEngine;
using System.Collections; // Necess�rio para usar Coroutines

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // O "molde" do inimigo que vamos criar
    public float spawnRadius = 20f; // O raio do c�rculo onde os inimigos podem nascer
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

            // Gera uma posi��o aleat�ria dentro de um c�rculo
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y); // Usamos Y para o eixo Z

            // Cria o inimigo na posi��o calculada
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}