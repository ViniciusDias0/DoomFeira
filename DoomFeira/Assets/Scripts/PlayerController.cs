using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Tiro")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    // ... Remova as referências do joystick daqui ...

    // ... Suas outras variáveis (status, HUD, etc.) ...
    [Header("Status do Jogador (usando float)")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxArmor = 100f;
    public float currentArmor;

    [Header("Componentes e HUD")]
    public HUDManager hudManager;
    private Camera playerCamera;

    [Header("Efeitos da Câmera (Head Bob)")]
    public bool enableHeadBob = true;
    public float bobFrequency = 2.0f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;

    private float walkingTime = 0.0f;
    private Vector3 cameraDefaultPosition;

    [Header("Controle de Queda")]
    [Tooltip("A altura Y abaixo da qual o jogador morre.")]
    public float fallThreshold = -50f;

    private bool isDead = false;

    [Header("Equipamento")]
    public WeaponStats currentWeapon;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        // ... O resto do seu Start pode permanecer o mesmo, mas a lógica do cursor agora é do InputManager ...
        currentHealth = maxHealth;
        currentArmor = 0f;
        if (hudManager != null) hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        if (playerCamera != null) cameraDefaultPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        // Agora só precisamos checar o InputManager
        if (InputManager.Instance.IsShooting)
        {
            Shoot();
        }
        HandleHeadBob();

        if (isDead) return;

        // --- ADIÇÃO AQUI ---
        // Verifica a cada frame se a posição Y do jogador está abaixo do limite.
        if (transform.position.y < fallThreshold)
        {
            isDead = true; // Marca como morto para não chamar de novo.
            Debug.Log("Jogador caiu no vácuo! Acionando Game Over...");
            Die(); // Chama a sua função de morte existente.
            return; // Para a execução do Update para este frame.
        }
    }

    void FixedUpdate()
    {
        // Pega os inputs diretamente do InputManager
        float moveVertical = InputManager.Instance.VerticalAxis;
        float rotationHorizontal = InputManager.Instance.HorizontalAxis;

        // O resto da sua lógica de movimento permanece a mesma
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotationHorizontal * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

        Vector3 moveDirection = transform.forward * moveVertical * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    // As funções OnShootButtonDown/Up não são mais necessárias aqui

    private void HandleHeadBob()
    {
        if (!enableHeadBob) return;

        // Pega os inputs do InputManager
        float horizontalInput = InputManager.Instance.HorizontalAxis;
        float verticalInput = InputManager.Instance.VerticalAxis;

        // O resto da lógica do Head Bob permanece a mesma
        // ...
        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            walkingTime += Time.deltaTime;
            float horizontalOffset = Mathf.Cos(walkingTime * bobFrequency) * bobHorizontalAmplitude;
            float verticalOffset = Mathf.Sin(walkingTime * bobFrequency * 2) * bobVerticalAmplitude;
            playerCamera.transform.localPosition = new Vector3(
                cameraDefaultPosition.x + horizontalOffset,
                cameraDefaultPosition.y + verticalOffset,
                cameraDefaultPosition.z
            );
        }
        else
        {
            walkingTime = 0;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                cameraDefaultPosition,
                Time.deltaTime * 5f
            );
        }
    }

    // ... O resto das suas funções (TakeDamage, Heal, Die, etc.) não precisam de alteração ...
    #region FuncoesDeStatus
    public void TakeDamage(float damage)
    {
        float damageToArmor = Mathf.Min(currentArmor, damage);
        currentArmor -= damageToArmor;

        float remainingDamage = damage - damageToArmor;
        currentHealth -= remainingDamage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHud();

        if (currentHealth <= 0)
        {
            Die();
        }

        if (isDead) return;
    }

    public bool Heal(float amount)
    {
        if (currentHealth >= maxHealth) return false;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateHud();
        return true;
    }

    public bool AddArmor(float amount)
    {
        if (currentArmor >= maxArmor) return false;
        currentArmor = Mathf.Clamp(currentArmor + amount, 0f, maxArmor);
        UpdateHud();
        return true;
    }

    private void Shoot()
    {
        if (currentWeapon != null)
        {
            currentWeapon.TryToShoot();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("O jogador morreu! Acionando o sistema de Game Over...");
        FindAnyObjectByType<GameOverTrigger>().TriggerGameOver();
    }

    private void UpdateHud()
    {
        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }
    }
    #endregion
}