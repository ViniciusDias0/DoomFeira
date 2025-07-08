using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Tiro")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    // ... suas outras vari�veis de tiro, etc.

    // --- ADI��ES PARA MOBILE ---
    [Header("Controles Mobile")]
    public Joystick movementJoystick; // Arraste o joystick de movimento aqui
    private bool shooting = false; // Flag para saber se o bot�o de tiro est� pressionado
    // --- FIM DAS ADI��ES ---

    // ... suas outras vari�veis (status, HUD, head bob, etc.) ...
    [Header("Status do Jogador (usando float)")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxArmor = 100f;
    public float currentArmor;

    [Header("Componentes e HUD")]
    public HUDManager hudManager;
    private Camera playerCamera;

    [Header("Efeitos da C�mera (Head Bob)")]
    public bool enableHeadBob = true;
    public float bobFrequency = 2.0f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;

    private float walkingTime = 0.0f;
    private Vector3 cameraDefaultPosition;

    [Header("Equipamento")]
    public WeaponStats currentWeapon;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        // No mobile, n�o precisamos travar o cursor
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        if (movementJoystick == null)
        {
            Debug.LogError("Joystick de movimento n�o foi definido no PlayerController! Controles mobile n�o funcionar�o.");
        }

        // ... resto do seu c�digo de Start ...
        currentHealth = maxHealth;
        currentArmor = 0f;
        if (hudManager != null) hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        if (playerCamera != null) cameraDefaultPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        // A l�gica de tiro agora � baseada na nossa flag 'shooting'
        if (shooting)
        {
            Shoot();
        }

        HandleDebugInputs(); // Pode manter para testar no PC
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        // --- L�GICA DE MOVIMENTO E ROTA��O MODIFICADA ---
        if (movementJoystick == null) return; // N�o faz nada se o joystick n�o estiver configurado

        // Pega os inputs diretamente do joystick
        // O joystick de movimento controlar� tanto o andar quanto o girar
        float moveVertical = movementJoystick.Vertical; // Para frente e para tr�s
        float rotationHorizontal = movementJoystick.Horizontal; // Para girar para os lados

        // ROTA��O
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotationHorizontal * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // MOVIMENTO
        Vector3 moveDirection = transform.forward * moveVertical * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    // --- NOVAS FUN��ES P�BLICAS PARA OS BOT�ES ---
    // Esta fun��o ser� chamada quando o bot�o de tiro for PRESSIONADO
    public void OnShootButtonDown()
    {
        shooting = true;
    }

    // Esta fun��o ser� chamada quando o bot�o de tiro for SOLTO
    public void OnShootButtonUp()
    {
        shooting = false;
    }
    // --- FIM DAS NOVAS FUN��ES ---

    // A fun��o HandleHeadBob precisa ler os inputs do joystick agora
    private void HandleHeadBob()
    {
        if (!enableHeadBob || movementJoystick == null) return;

        // Pega os inputs do joystick
        float horizontalInput = movementJoystick.Horizontal;
        float verticalInput = movementJoystick.Vertical;

        // O resto da l�gica do Head Bob permanece a mesma
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

    // O resto dos seus scripts (TakeDamage, Heal, Die, etc.) n�o precisam de altera��o
    // ...
    // (Cole o resto das suas fun��es aqui)
    // ...
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
        // A l�gica antiga de reiniciar a cena foi substitu�da pela nova l�gica
        // que chama o sistema de Game Over. A l�gica do Debug.Log pode ser mantida.
        Debug.Log("O jogador morreu! Acionando o sistema de Game Over...");

        FindObjectOfType<GameOverTrigger>().TriggerGameOver();
    }

    private void UpdateHud()
    {
        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }
    }

    private void HandleDebugInputs() { }
    #endregion
}