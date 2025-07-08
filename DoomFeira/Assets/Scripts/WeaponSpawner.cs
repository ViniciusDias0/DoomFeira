using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn de Armas")]
    public WeaponProfile[] availableWeaponProfiles;

    // --- MUDANÇA: Substituímos o raio por um Vector3 para definir o tamanho da caixa ---
    [Header("Área de Spawn (Paralelepípedo)")]
    public Vector3 spawnAreaSize = new Vector3(40f, 1f, 40f); // Largura(X), Altura(Y), Comprimento(Z)

    [Header("Controle de Spawn")]
    public float spawnInterval = 15f;


    // --- ADIÇÃO: A função que desenha o Gizmo no Editor ---
    // Esta função especial da Unity é chamada automaticamente na janela Scene
    void OnDrawGizmosSelected()
    {
        // Define a cor que o Gizmo terá. Usaremos um verde semitransparente.
        Gizmos.color = new Color(0, 1, 0, 0.5f); // R, G, B, Alpha

        // Desenha um cubo (paralelepípedo) na posição deste objeto, com o tamanho que definimos.
        // Este Gizmo só será visível quando você selecionar o objeto Spawner na Hierarchy.
        Gizmos.DrawCube(transform.position, spawnAreaSize);
    }


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

            int randomIndex = Random.Range(0, availableWeaponProfiles.Length);
            WeaponProfile randomProfile = availableWeaponProfiles[randomIndex];

            if (randomProfile.pickupPrefab == null)
            {
                Debug.LogWarning($"O perfil '{randomProfile.name}' não tem um 'Pickup Prefab' definido.");
                continue;
            }

            // --- MUDANÇA: Lógica de geração de posição ---
            // Agora geramos um ponto dentro do nosso paralelepípedo

            // Calcula a metade das dimensões para facilitar o cálculo do intervalo
            float halfWidth = spawnAreaSize.x / 2;
            float halfDepth = spawnAreaSize.z / 2;

            // Gera um deslocamento aleatório a partir do centro do spawner
            Vector3 randomOffset = new Vector3(
                Random.Range(-halfWidth, halfWidth),
                0, // Mantém o spawn na mesma altura Y do spawner
                Random.Range(-halfDepth, halfDepth)
            );

            // A posição final é a posição do spawner mais o deslocamento aleatório
            Vector3 spawnPosition = transform.position + randomOffset;
            // --- FIM DA MUDANÇA ---


            GameObject pickupInstance = Instantiate(randomProfile.pickupPrefab, spawnPosition, Quaternion.identity);

            WeaponPickup pickupScript = pickupInstance.GetComponent<WeaponPickup>();
            if (pickupScript != null)
            {
                pickupScript.weaponProfile = randomProfile;
            }
        }
    }
}