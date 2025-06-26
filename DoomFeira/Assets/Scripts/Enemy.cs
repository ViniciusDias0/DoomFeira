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

    // Esta � a fun��o que faz o inimigo "sumir"
    public void Die()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        // 2. Se encontrou, chama a fun��o para adicionar pontos
        if (gameManager != null)
        {
            gameManager.AddScore(pointsValue);
        }
        else
        {
            Debug.LogWarning("GameManager n�o encontrado na cena!");
        }

        // Destr�i o GameObject ao qual este script est� anexado.
        Destroy(gameObject);
    }

    // Adicione esta fun��o inteira ao script Enemy.cs
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

            // Depois de atacar, o inimigo se destr�i
            // (Isso evita que um �nico inimigo cause dano cont�nuo)
            Destroy(gameObject);
        }
    }
}