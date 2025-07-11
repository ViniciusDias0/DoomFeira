using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Rotação")]
    public float moveSpeed = 5f;

    // --- MUDANÇA AQUI: Apenas uma variável para a sensibilidade ---
    [Tooltip("Controla a sensibilidade da rotação da câmera (mouse e toque).")]
    public float lookSensitivity = 100f;

    // ... (O resto das suas variáveis permanece o mesmo) ...
    #region Variáveis Intactas
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

    [Header("Equipamento")]
    public WeaponStats currentWeapon;
    #endregion

    private Rigidbody rb;
    private float cameraVerticalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerController precisa de um componente Rigidbody!", this.gameObject);
            this.enabled = false;
            return;
        }

        playerCamera = GetComponentInChildren<Camera>();

        currentHealth = maxHealth;
        currentArmor = 0f;

        if (hudManager != null) { hudManager.UpdateStatus((int)currentHealth, (int)currentArmor); }
        if (playerCamera != null) { cameraDefaultPosition = playerCamera.transform.localPosition; }
    }

    void Update()
    {
        if (InputManager.Instance == null) return;

        HandleRotation();
        HandleShooting();
        HandleDebugInputs();
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        if (InputManager.Instance == null) return;
        HandleMovement();
    }

    private void HandleRotation()
    {
        // Usa a variável única 'lookSensitivity' para a rotação.
        float lookX = InputManager.Instance.LookX * lookSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * lookX);

        cameraVerticalRotation -= InputManager.Instance.LookY * lookSensitivity * Time.deltaTime;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);
    }

    private void HandleMovement()
    {
        float moveVertical = InputManager.Instance.VerticalAxis;
        float moveHorizontal = InputManager.Instance.HorizontalAxis;

        Vector3 moveDirection = (transform.forward * moveVertical) + (transform.right * moveHorizontal);

        Vector3 targetVelocity = moveDirection.normalized * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }

    // --- FUNÇÃO PÚBLICA CORRIGIDA E NO LUGAR CERTO ---
    // Esta função permite que o PauseMenuController altere nossa variável lookSensitivity.
    public void SetLookSensitivity(float newSensitivity)
    {
        lookSensitivity = newSensitivity;
    }

    // O resto das suas funções (HandleShooting, TakeDamage, etc.) permanecem 100% intactas.
    #region Funções de Jogo (Intactas)
    private void HandleShooting()
    {
        if (InputManager.Instance.IsShooting)
        {
            Shoot();
        }
    }

    private void HandleDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.T)) { TakeDamage(10f); }
        if (Input.GetKeyDown(KeyCode.Y)) { AddArmor(25f); }
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
        if (hudManager != null) { hudManager.UpdateStatus((int)currentHealth, (int)currentArmor); }
        if (currentHealth <= 0) { Die(); }
    }

    public bool Heal(float amount)
    {
        if (currentHealth >= maxHealth) return false;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log($"Jogador curou {amount} de vida. Vida: {currentHealth}");
        UpdateHud();
        if (hudManager != null) { hudManager.UpdateStatus((int)currentHealth, (int)currentArmor); }
        return true;
    }

    public bool AddArmor(float amount)
    {
        if (currentArmor >= maxArmor) return false;
        currentArmor += amount;
        currentArmor = Mathf.Clamp(currentArmor, 0f, maxArmor);
        Debug.Log($"Jogador pegou {amount} de armadura. Armadura: {currentArmor}");
        UpdateHud();
        if (hudManager != null) { hudManager.UpdateStatus((int)currentHealth, (int)currentArmor); }
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

    private void HandleHeadBob()
    {
        if (!enableHeadBob) return;
        float horizontalInput = InputManager.Instance.HorizontalAxis;
        float verticalInput = InputManager.Instance.VerticalAxis;
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
    #endregion
}