using UnityEngine;
using System.Collections; // Adicionado para podermos usar Coroutines (para o tempo de vida)

public class WeaponPickup : MonoBehaviour
{
    // Esta parte do seu código permanece a mesma
    public WeaponProfile weaponProfile;

    // --- ADIÇÕES ---
    [Header("Efeitos e Tempo de Vida")]
    public float rotationSpeed = 60f; // Velocidade que o item gira no chão. Configurável no Inspector.
    public float lifetime = 15f;      // Tempo em segundos para o item desaparecer. Configurável no Inspector.

    // A função Start é executada uma vez quando o objeto é criado
    void Start()
    {
        // Garante que o item tenha uma aparência visual baseada no perfil da arma
        // Isso é útil se você tiver um spawner que atribui o perfil dinamicamente
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && weaponProfile != null && weaponProfile.weaponSprite != null)
        {
            sr.sprite = weaponProfile.weaponSprite;
        }

        // Inicia a "contagem regressiva" para o item desaparecer
        StartCoroutine(LifetimeRoutine());
    }

    // A função Update é executada a cada frame
    void Update()
    {
        // Faz o objeto girar no seu eixo Y (para cima), criando o efeito de rotação
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    // Esta é a rotina que cuida do tempo de vida
    private IEnumerator LifetimeRoutine()
    {
        // Espera pela quantidade de segundos definida na variável 'lifetime'
        yield return new WaitForSeconds(lifetime);

        // Após a espera, destrói o objeto do item do chão
        Destroy(gameObject);
    }
    // --- FIM DAS ADIÇÕES ---


    // Esta parte do seu código permanece a mesma
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponManager_Profiled weaponManager = other.GetComponent<WeaponManager_Profiled>();
            if (weaponManager != null)
            {
                // A sua função para pegar a arma, que já funciona
                weaponManager.PickupWeapon(weaponProfile);
                Destroy(gameObject);
            }
        }
    }
}