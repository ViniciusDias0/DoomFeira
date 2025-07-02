using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Tiro")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float shootDistance = 100f;
    public LayerMask shootableMask;

    [Header("Status do Jogador (usando float)")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxArmor = 100f;
    public float currentArmor;

    [Header("Componentes e HUD")]
    public HUDManager hudManager;
    private Camera playerCamera;

    [Header("Efeitos da Câmera (Head Bob)")]
    public bool enableHeadBob = true; // Para poder ligar/desligar o efeito facilmente
    public float bobFrequency = 2.0f; // Quão rápido a cabeça balança (2 balanços por segundo)
    public float bobHorizontalAmplitude = 0.1f; // O quanto a cabeça move para os lados
    public float bobVerticalAmplitude = 0.1f; // O quanto a cabeça move para cima e para baixo

    private float walkingTime = 0.0f;
    private Vector3 cameraDefaultPosition; // Para sabermos a posição original da câmera

    [Header("Equipamento")]
    public WeaponStats currentWeapon;


    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxHealth;
        currentArmor = 0f; // Começa sem armadura

        // Atualiza a HUD pela primeira vez
        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }

        playerCamera = GetComponentInChildren<Camera>(); // Você já deve ter esta linha

        // GUARDA A POSIÇÃO INICIAL DA CÂMERA
        if (playerCamera != null)
        {
            cameraDefaultPosition = playerCamera.transform.localPosition;
        }

    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleDebugInputs();

        HandleHeadBob();
    }

    private void HandleMovement()
    {
        float moveVertical = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime);

        float rotationHorizontal = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * rotationHorizontal * rotationSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void HandleDebugInputs()
    {
        // Aperte 'T' para tomar 10 de dano
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f); // Usamos 10f para ser consistente com o tipo float
        }

        // Aperte 'Y' para pegar 25 de armadura
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddArmor(25f); // Usamos 25f
        }
    }

    public void TakeDamage(float damage)
    {
        float damageToArmor = Mathf.Min(currentArmor, damage);
        currentArmor -= damageToArmor;

        float remainingDamage = damage - damageToArmor;
        currentHealth -= remainingDamage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Dano recebido! Vida: {currentHealth}, Armadura: {currentArmor}");
        UpdateHud();

        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool Heal(float amount)
    {
        if (currentHealth >= maxHealth)
        {
            return false;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log($"Jogador curou {amount} de vida. Vida: {currentHealth}");
        UpdateHud();

        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }

        return true;
    }

    public bool AddArmor(float amount)
    {
        if (currentArmor >= maxArmor)
        {
            return false;
        }

        currentArmor += amount;
        currentArmor = Mathf.Clamp(currentArmor, 0f, maxArmor);
        Debug.Log($"Jogador pegou {amount} de armadura. Armadura: {currentArmor}");
        UpdateHud();

        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }

        return true;
    }

    private void Shoot()
    {
        if (currentWeapon != null)
        {
            currentWeapon.TryToShoot();
        }
    }

    // --- A ÚNICA ALTERAÇÃO FOI FEITA AQUI ---
    private void Die()
    {
        // A lógica antiga de reiniciar a cena foi substituída pela nova lógica
        // que chama o sistema de Game Over. A lógica do Debug.Log pode ser mantida.
        Debug.Log("O jogador morreu! Acionando o sistema de Game Over...");

        // Esta linha encontra o GameOverTrigger na cena e chama a função para carregar a tela de Game Over.
        FindObjectOfType<GameOverTrigger>().TriggerGameOver();
    }
    // --- FIM DA ALTERAÇÃO ---

    // Função central para atualizar o HUD, para não repetir código
    private void UpdateHud()
    {
        if (hudManager != null)
        {
            // CORREÇÃO: Usando o novo nome da função
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }
    }

    // Adicione esta nova função ao PlayerController.cs
    private void HandleHeadBob()
    {
        // Se o efeito estiver desligado, não faz nada
        if (!enableHeadBob) return;

        // Pega a entrada de movimento horizontal e vertical
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            // JOGADOR ESTÁ SE MOVENDO

            // Incrementa o tempo de caminhada, que será usado na função de seno
            walkingTime += Time.deltaTime;

            // Calcula os deslocamentos usando a função Seno para um movimento suave e oscilante
            float horizontalOffset = Mathf.Cos(walkingTime * bobFrequency) * bobHorizontalAmplitude;
            float verticalOffset = Mathf.Sin(walkingTime * bobFrequency * 2) * bobVerticalAmplitude; // Multiplicamos por 2 para que o movimento vertical seja mais rápido (passo cima-baixo)

            // Aplica o deslocamento à posição padrão da câmera
            playerCamera.transform.localPosition = new Vector3(
                cameraDefaultPosition.x + horizontalOffset,
                cameraDefaultPosition.y + verticalOffset,
                cameraDefaultPosition.z
            );
        }
        else
        {
            // JOGADOR ESTÁ PARADO

            // Reseta o tempo de caminhada
            walkingTime = 0;

            // Suaviza o retorno da câmera para a posição padrão usando Lerp
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                cameraDefaultPosition,
                Time.deltaTime * 5f
            );
        }
    }
}