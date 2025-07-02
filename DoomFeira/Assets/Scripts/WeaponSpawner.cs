using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn de Armas")]
    // Agora só precisamos saber quais perfis de arma estão disponíveis
    public WeaponProfile[] availableWeaponProfiles;

    [Header("Controle de Spawn")]
    public float spawnRadius = 22f;
    public float spawnInterval = 15f;

    void Start()
    {
        StartCoroutine(SpawnWeaponsRoutine());
    }

    IEnumerator SpawnWeaponsRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (availableWeaponProfiles.Length == 0)
            {
                Debug.LogWarning("WeaponSpawner não tem perfis de arma para spawnar.");
                continue;
            }

            // 1. Escolhe um PERFIL de arma aleatório
            int randomIndex = Random.Range(0, availableWeaponProfiles.Length);
            WeaponProfile randomProfile = availableWeaponProfiles[randomIndex];

            // Garante que o perfil escolhido tem um prefab de pickup associado
            if (randomProfile.pickupPrefab == null)
            {
                Debug.LogWarning($"O perfil '{randomProfile.name}' não tem um 'Pickup Prefab' definido.");
                continue;
            }

            // 2. Gera uma posição aleatória
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);

            // 3. Cria uma instância do item de chão a partir do PERFIL
            GameObject pickupInstance = Instantiate(randomProfile.pickupPrefab, spawnPosition, Quaternion.identity);

            // 4. (Opcional, mas seguro) Garante que o item criado sabe a qual perfil ele pertence
            WeaponPickup pickupScript = pickupInstance.GetComponent<WeaponPickup>();
            if (pickupScript != null)
            {
                pickupScript.weaponProfile = randomProfile;
            }
        }
    }
}