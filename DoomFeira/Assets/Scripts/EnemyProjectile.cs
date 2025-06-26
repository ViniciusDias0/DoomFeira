using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 25f; // Dano maior que o inimigo de corpo a corpo
    public float lifetime = 5f; // Tempo para o proj�til se destruir se n�o acertar nada

    void Start()
    {
        // Pega o componente Rigidbody e d� a ele uma velocidade inicial para frente
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        // Destr�i o proj�til depois de 'lifetime' segundos
        Destroy(gameObject, lifetime);
    }

    // Fun��o chamada quando o proj�til entra em um trigger
    void OnTriggerEnter(Collider other)
    {
        // Se acertar o jogador...
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // ...causa dano
                player.TakeDamage(damage);
            }

            // Destr�i o proj�til no impacto com o jogador
            Destroy(gameObject);
        }
        // Ignora colis�es com outros inimigos ou itens
        else if (other.CompareTag("Enemy") || other.GetComponent<Powerup>() != null)
        {
            // N�o faz nada, apenas passa por eles
        }
        else
        {
            // Se acertar uma parede ou o ch�o, tamb�m se destr�i
            Destroy(gameObject);
        }
    }
}