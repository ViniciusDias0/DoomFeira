using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Esta fun��o ser� chamada pelo script do jogador quando for atingido
    public void Die()
    {
        // A a��o mais simples: destruir o objeto do inimigo
        Destroy(gameObject);
    }
}