using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Esta função será chamada pelo script do jogador quando for atingido
    public void Die()
    {
        // A ação mais simples: destruir o objeto do inimigo
        Destroy(gameObject);
    }
}