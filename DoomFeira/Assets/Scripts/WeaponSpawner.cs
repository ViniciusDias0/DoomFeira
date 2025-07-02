using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Configura��o de Spawn de Armas")]
    // Agora s� precisamos saber quais perfis de arma est�o dispon�veis
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
                Debug.LogWarning("WeaponSpawner n�o tem perfis de arma para spawnar.");
                continue;
            }

            // 1. Escolhe um PERFIL de arma aleat�rio
            int randomIndex = Random.Range(0, availableWeaponProfiles.Length);
            WeaponProfile randomProfile = availableWeaponProfiles[randomIndex];

            // Garante que o perfil escolhido tem um prefab de pickup associado
            if (randomProfile.pickupPrefab == null)
            {
                Debug.LogWarning($"O perfil '{randomProfile.name}' n�o tem um 'Pickup Prefab' definido.");
                continue;
            }

            // 2. Gera uma posi��o aleat�ria
            Vector2 randomPointInCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);

            // 3. Cria uma inst�ncia do item de ch�o a partir do PERFIL
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