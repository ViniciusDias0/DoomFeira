using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform playerTarget;
    private NavMeshAgent agent;

    [Header("Attack")]
    public int attackDamage = 15; // Dano que o inimigo causa
    public int pointsValue = 300;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
    }

    void Update()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    // Esta é a função que faz o inimigo "sumir"
    public void Die()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        // 2. Se encontrou, chama a função para adicionar pontos
        if (gameManager != null)
        {
            gameManager.AddScore(pointsValue);
        }
        else
        {
            Debug.LogWarning("GameManager não encontrado na cena!");
        }

        // Destrói o GameObject ao qual este script está anexado.
        Destroy(gameObject);
    }

    // Adicione esta função inteira ao script Enemy.cs
    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto que colidimos tem a tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tenta pegar o componente PlayerController do objeto que colidimos
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // Se encontrou o script do jogador, causa dano a ele
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }

            // Depois de atacar, o inimigo se destrói
            // (Isso evita que um único inimigo cause dano contínuo)
            Destroy(gameObject);
        }
    }
}