using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject[] itemPrefabs;   // Array para colocar nossos prefabs de Cura e Armadura
    public float spawnRadius = 24f;      // Raio do círculo onde os itens podem nascer
    public float spawnInterval = 10f;    // Tempo (em segundos) entre cada spawn

    [Header("Controle de Itens")]
    public int maxItemsOnMap = 5; // Número máximo de itens no mapa ao mesmo tempo
    private int currentItemCount = 0; // Contador de itens atuais

    void Start()
    {
        // Inicia a rotina de spawn de itens
        StartCoroutine(SpawnItemsRoutine());
    }

    IEnumerator SpawnItemsRoutine()
    {
        // Loop infinito para sempre tentar criar itens
        while (true)
        {
            // Só cria um novo item se tivermos espaço no mapa
            if (currentItemCount < maxItemsOnMap)
            {
                SpawnRandomItem();
            }
            // Espera o intervalo de tempo antes de tentar novamente
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomItem()
    {
        // 1. Escolhe um item aleatório do nosso array de prefabs
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject itemToSpawn = itemPrefabs[randomIndex];
        
        // 2. Gera uma posição aleatória dentro do círculo da arena
        Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y); // Y=1 para ele aparecer um pouco acima do chão

        // 3. Cria (instancia) o item na cena
        Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);

        // 4. Incrementa nosso contador de itens
        currentItemCount++;
    }

    // Precisamos de uma forma de saber quando um item é destruído
    // Podemos fazer isso com um evento ou de forma mais simples:
    void Update()
    {
        // Encontra todos os objetos com o script Powerup na cena e atualiza o contador
        // Nota: Não é a forma mais otimizada, mas para um jogo simples, funciona perfeitamente.
        currentItemCount = FindObjectsByType<Powerup>(FindObjectsSortMode.None).Length;
    }
}