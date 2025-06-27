using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Configura��o de Spawn de Armas")]
    public GameObject[] weaponPickupPrefabs; // Array APENAS para prefabs de armas colet�veis
    public float spawnRadius = 22f; // Um raio um pouco diferente para n�o nascer no mesmo lugar
    public float spawnInterval = 15f; // Armas podem ser mais raras

    void Start()
    {
        StartCoroutine(SpawnWeaponsRoutine());
    }

    IEnumerator SpawnWeaponsRoutine()
    {
        // Espera alguns segundos no in�cio para n�o sobrecarregar o jogador
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Espera o intervalo para o pr�ximo spawn
            yield return new WaitForSeconds(spawnInterval);

            // Escolhe um prefab de arma aleat�rio
            int randomIndex = Random.Range(0, weaponPickupPrefabs.Length);
            GameObject weaponToSpawn = weaponPickupPrefabs[randomIndex];

            // Gera uma posi��o aleat�ria
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);

            // Cria o item da arma na cena
            Instantiate(weaponToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}