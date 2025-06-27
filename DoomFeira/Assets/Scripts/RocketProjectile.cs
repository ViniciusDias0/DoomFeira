using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [Header("Atributos do Foguete")]
    public float speed = 20f;
    public float lifetime = 5f;
    public float explosionRadius = 5f; // O raio da explos�o
    public float explosionDamage = 150f; // Dano alto para matar a maioria dos inimigos
    public GameObject explosionEffectPrefab; // Um efeito de part�cula para a explos�o (opcional)

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        Destroy(gameObject, lifetime);
    }

    // Quando o foguete colide com algo
    void OnTriggerEnter(Collider other)
    {
        // Ignora colis�es com o pr�prio jogador ou com itens
        if (other.CompareTag("Player") || other.GetComponent<Powerup>() != null)
        {
            return;
        }

        // Aciona a explos�o e se destr�i
        Explode();
        Destroy(gameObject);
    }

    void Explode()
    {
        // Opcional: Instancia o efeito de explos�o
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // --- A M�GICA DO DANO EM �REA ---
        // Cria uma esfera invis�vel e pega todos os colliders dentro dela
        Collider[] collidersInRadius = Physics.OverlapSphere(transform.position, explosionRadius);

        // Passa por cada objeto encontrado
        foreach (Collider hitCollider in collidersInRadius)
        {
            // Tenta pegar o script de inimigo do objeto
            Enemy meleeEnemy = hitCollider.GetComponent<Enemy>();
            if (meleeEnemy != null)
            {
                // Aplica o dano da explos�o
                meleeEnemy.TakeDamage(explosionDamage);
                continue; // Pula para o pr�ximo objeto
            }

            RangedEnemy rangedEnemy = hitCollider.GetComponent<RangedEnemy>();
            if (rangedEnemy != null)
            {
                rangedEnemy.TakeDamage(explosionDamage);
            }
        }
    }
}