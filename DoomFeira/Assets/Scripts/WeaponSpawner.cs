using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn de Armas")]
    public GameObject[] weaponPickupPrefabs; // Array APENAS para prefabs de armas coletáveis
    public float spawnRadius = 22f; // Um raio um pouco diferente para não nascer no mesmo lugar
    public float spawnInterval = 15f; // Armas podem ser mais raras

    void Start()
    {
        StartCoroutine(SpawnWeaponsRoutine());
    }

    IEnumerator SpawnWeaponsRoutine()
    {
        // Espera alguns segundos no início para não sobrecarregar o jogador
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Espera o intervalo para o próximo spawn
            yield return new WaitForSeconds(spawnInterval);

            // Escolhe um prefab de arma aleatório
            int randomIndex = Random.Range(0, weaponPickupPrefabs.Length);
            GameObject weaponToSpawn = weaponPickupPrefabs[randomIndex];

            // Gera uma posição aleatória
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);

            // Cria o item da arma na cena
            Instantiate(weaponToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}