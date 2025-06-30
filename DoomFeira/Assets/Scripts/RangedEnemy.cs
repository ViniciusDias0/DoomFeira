using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 150f; // Vida maior
    public int pointsValue = 600; // Vale mais pontos

    [Header("Movimento")]
    public float stoppingDistance = 10f; // Distância que ele para antes de atirar

    [Header("Combate")]
    public GameObject projectilePrefab; // O prefab do tiro que criamos
    public Transform firePoint; // O ponto de onde o tiro vai sair
    public float fireRate = 2f; // Atira a cada 2 segundos
    private float nextFireTime = 0f;

    private Transform playerTarget;
    private NavMeshAgent agent;
    private GameManager gameManager;

    public SpriteAnimator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();

        // Encontra o jogador
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;

            // Se o inimigo está se movendo, toca a animação "Walk"
            if (agent.velocity.magnitude > 0.1f)
            {
                animator.Play("Walk");
            }
            // Se estiver parado, toca a animação "Idle"
            else
            {
                animator.Play("Idle");
            }
        }

        // Define a distância de parada no NavMeshAgent
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (playerTarget == null) return;

        // A distância até o jogador
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // Movimenta-se em direção ao jogador
        agent.SetDestination(playerTarget.position);

        // Gira para olhar para o jogador SOMENTE se não estiver muito perto
        if (distanceToPlayer > 0.5f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(playerTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }


        // --- LÓGICA DE TIRO CORRIGIDA ---
        // Verifica se o jogador está dentro da distância de ataque
        if (distanceToPlayer <= agent.stoppingDistance)
        {
            // Se estiver na distância, verifica se já pode atirar novamente
            if (Time.time >= nextFireTime)
            {
                // Atira e ATUALIZA o tempo do próximo tiro
                Shoot();
                nextFireTime = Time.time + 1f / fireRate; // A atualização do tempo agora está AQUI
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Cria o projétil no ponto de disparo
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    // Função para receber dano (do tiro do jogador)
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    // No RangedEnemy.cs

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
        animator.Play("Death");

        // Desativa a lógica para que ele pare no lugar
        this.enabled = false; // Desativa este próprio script (o Update para)
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<Billboard>().enabled = false; // Para de encarar a câmera

        // ... sua lógica de score ...

        // Destrói o objeto depois de um tempo para dar tempo de ver a animação de morte
        Destroy(gameObject, 2f);
    }
}