using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 50f;
    public float lifetime = 3f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Causa dano no inimigo fraco
        Enemy meleeEnemy = other.GetComponent<Enemy>();
        if (meleeEnemy != null)
        {
            meleeEnemy.TakeDamage(damage); // Assumindo que ele tem vida agora
            Destroy(gameObject);
            return;
        }

        // Causa dano no inimigo forte
        RangedEnemy rangedEnemy = other.GetComponent<RangedEnemy>();
        if (rangedEnemy != null)
        {
            rangedEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Destr�i em paredes, etc.
        if (!other.CompareTag("Player") && other.GetComponent<Powerup>() == null)
        {
            Destroy(gameObject);
        }
    }
}