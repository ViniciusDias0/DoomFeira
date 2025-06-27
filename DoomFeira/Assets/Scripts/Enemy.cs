using UnityEngine;
using UnityEngine.AI; // Adicione se não tiver

public class Enemy : MonoBehaviour
{
    // --- VARIÁVEIS DO INIMIGO FRACO ---
    public float health = 100f;      // <<-- ADICIONE A VIDA
    public float damage = 20f;
    public int pointsValue = 10;

    private Transform playerTarget;
    private NavMeshAgent agent;
    private GameManager gameManager; // Adicione para otimizar a busca

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>(); // Encontra o game manager uma vez

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); // O dano dele ao encostar
            }
            Destroy(gameObject);
        }
    }

    // --- FUNÇÃO FALTANTE ---
    // Adicione esta função pública para que o projétil possa chamá-la
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    // A função Die() permanece a mesma
    public void Die()
    {
        if (gameManager != null)
        {
            gameManager.AddScore(pointsValue);
        }
        else // Fallback caso não encontre no Start
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null) gm.AddScore(pointsValue);
        }
        Destroy(gameObject);
    }
}