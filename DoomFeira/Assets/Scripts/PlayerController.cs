using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variáveis de Movimento
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    // Variáveis de Tiro
    public float shootDistance = 100f; // Distância máxima do tiro
    private Camera playerCamera;

    void Start()
    {
        // Pega a referência da câmera que está filha do jogador
        playerCamera = GetComponentInChildren<Camera>();

        // Trava e esconde o cursor do mouse (típico de FPS)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- MOVIMENTO ---
        // Mover para frente e para trás (Setas Cima/Baixo)
        float moveVertical = Input.GetAxis("Vertical"); // Usa as configurações padrão de W/S e Cima/Baixo
        transform.Translate(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime);

        // Rotacionar para os lados (Setas Esquerda/Direita)
        float rotationHorizontal = Input.GetAxis("Horizontal"); // Usa as configurações padrão de A/D e Esquerda/Direita
        transform.Rotate(Vector3.up * rotationHorizontal * rotationSpeed * Time.deltaTime);

        // --- TIRO ---
        // Atirar com a tecla Espaço
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Cria um raio a partir do centro da câmera, para frente
        RaycastHit hit;

        // Physics.Raycast dispara um "raio laser" invisível.
        // Se ele atingir algo com um Collider, a função retorna true e as informações do que foi atingido são guardadas em 'hit'.
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootDistance))
        {
            Debug.Log("Acertei: " + hit.transform.name); // Mostra no console o que foi atingido

            // Verifica se o que atingimos tem o script do Inimigo
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Se for um inimigo, chama a função para ele morrer
                enemy.Die();
            }
        }
    }
}