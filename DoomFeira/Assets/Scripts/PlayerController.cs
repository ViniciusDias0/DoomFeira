using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Vari�veis de Movimento
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    // Vari�veis de Tiro
    public float shootDistance = 100f; // Dist�ncia m�xima do tiro
    private Camera playerCamera;

    void Start()
    {
        // Pega a refer�ncia da c�mera que est� filha do jogador
        playerCamera = GetComponentInChildren<Camera>();

        // Trava e esconde o cursor do mouse (t�pico de FPS)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- MOVIMENTO ---
        // Mover para frente e para tr�s (Setas Cima/Baixo)
        float moveVertical = Input.GetAxis("Vertical"); // Usa as configura��es padr�o de W/S e Cima/Baixo
        transform.Translate(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime);

        // Rotacionar para os lados (Setas Esquerda/Direita)
        float rotationHorizontal = Input.GetAxis("Horizontal"); // Usa as configura��es padr�o de A/D e Esquerda/Direita
        transform.Rotate(Vector3.up * rotationHorizontal * rotationSpeed * Time.deltaTime);

        // --- TIRO ---
        // Atirar com a tecla Espa�o
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Cria um raio a partir do centro da c�mera, para frente
        RaycastHit hit;

        // Physics.Raycast dispara um "raio laser" invis�vel.
        // Se ele atingir algo com um Collider, a fun��o retorna true e as informa��es do que foi atingido s�o guardadas em 'hit'.
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootDistance))
        {
            Debug.Log("Acertei: " + hit.transform.name); // Mostra no console o que foi atingido

            // Verifica se o que atingimos tem o script do Inimigo
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Se for um inimigo, chama a fun��o para ele morrer
                enemy.Die();
            }
        }
    }
}