using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    // Enum para definir o tipo do item no Inspector
    public enum PowerupType { Health, Armor }
    
    [Header("Configuração do Item")]
    public PowerupType type;
    public float value = 25f;

    [Header("Efeitos Visuais")]
    public float rotationSpeed = 50f;

    [Header("Tempo de Vida")]
    public float lifetime = 10f; // Tempo em segundos antes de desaparecer

    // Referência para o HUD Manager
    private HUDManager hudManager;

    void Start()
    {
        // Encontra o HUD Manager na cena assim que o item é criado.
        // Isso assume que você tem apenas um objeto com o script HUDManager.
        hudManager = FindObjectOfType<HUDManager>();
        
        // Inicia a contagem regressiva para se autodestruir
        StartCoroutine(LifetimeRoutine());
    }

    void Update()
    {
        // Faz o item girar
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    // Rotina que destrói o item após 'lifetime' segundos
    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        
        // Se o item ainda existir depois do tempo, destrói ele
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>(); // Troque "PlayerController" pelo nome exato do seu script de jogador
            if (player == null) return; // Sai se não encontrar o script do jogador

            bool itemWasUsed = false;

            // Verifica o tipo do item e tenta usá-lo
            if (type == PowerupType.Health)
            {
                itemWasUsed = player.Heal(value); // A função agora retorna true ou false
            }
            else if (type == PowerupType.Armor)
            {
                itemWasUsed = player.AddArmor(value);
            }

            // Se o item foi usado com sucesso...
            if (itemWasUsed)
            {
                // ATUALIZA O HUD
                // A atualização agora está dentro das funções Heal() e AddArmor() do jogador,
                // mas se não estivesse, você a chamaria aqui:
                // hudManager.UpdateHUD(player.GetCurrentHealth(), player.GetCurrentArmor());
                
                // ...destrói o objeto do item.
                Destroy(gameObject);
            }
            // Se o item não foi usado (vida/armadura cheia), nada acontece. O item continua lá
            // até seu tempo de vida acabar.
        }
    }
}