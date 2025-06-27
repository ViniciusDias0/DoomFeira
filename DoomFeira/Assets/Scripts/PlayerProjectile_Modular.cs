using UnityEngine;

public class PlayerProjectile_Modular : MonoBehaviour
{
    // Vari�veis p�blicas que ser�o definidas pela ARMA no momento do disparo
    public float damage;
    public float speed;
    public float lifetime = 3f;

    void Start()
    {
        // A velocidade � aplicada no momento em que � criado
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Se colidir com um inimigo, causa o dano que foi definido pela arma
        Enemy meleeEnemy = other.GetComponent<Enemy>();
        if (meleeEnemy != null)
        {
            meleeEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        RangedEnemy rangedEnemy = other.GetComponent<RangedEnemy>();
        if (rangedEnemy != null)
        {
            rangedEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // IMPORTANTE: Ignora a colis�o com o jogador
        if (other.CompareTag("Player"))
        {
            return; // N�o faz nada, simplesmente continua
        }

        // Destr�i em qualquer outra colis�o (paredes, ch�o, etc.)
        if (other.GetComponent<Powerup>() == null)
        {
            Destroy(gameObject);
        }
    }
}