// PlayerController.cs (Refatorado para usar Rigidbody)
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Rotação")]
    public float moveSpeed = 5f;
    [Tooltip("Sensibilidade da rotação para mouse e toque.")]
    public float lookSensitivity = 100f;

    // ... (Suas outras variáveis)
    #region Variáveis Intactas
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
    public bool enableHeadBob = true;
    public float bobFrequency = 2.0f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;
    private float walkingTime = 0.0f;
    private Vector3 cameraDefaultPosition;
    [Header("Equipamento")]
    public WeaponStats currentWeapon;
    #endregion

    // --- NOVAS VARIÁVEIS PARA O RIGIDBODY ---
    private Rigidbody rb;
    private float cameraVerticalRotation = 0f;
    private Vector3 playerVelocity = Vector3.zero;
    private float yRotation = 0f;

    void Start()
    {
        // --- ADIÇÃO NO START ---
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

        // No Update, nós apenas lemos o input e guardamos nas nossas variáveis.
        // A física será aplicada no FixedUpdate.
        ReadAndStoreInput();

        HandleShooting();
        HandleDebugInputs();
        HandleHeadBob();
    }

    // --- NOVA FUNÇÃO PARA ORGANIZAR O INPUT ---
    private void ReadAndStoreInput()
    {
        // 1. Input de Movimento (para a física)
        float moveVertical = InputManager.Instance.VerticalAxis;
        float moveHorizontal = InputManager.Instance.HorizontalAxis;
        Vector3 moveDirection = (transform.forward * moveVertical) + (transform.right * moveHorizontal);
        playerVelocity = moveDirection.normalized * moveSpeed;

        // 2. Input de Rotação (para a física)
        yRotation = InputManager.Instance.LookX * lookSensitivity;

        // 3. Rotação Vertical da Câmera (pode ser feita aqui mesmo)
        cameraVerticalRotation -= InputManager.Instance.LookY * lookSensitivity * Time.deltaTime;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);
    }

    // O movimento e a rotação agora acontecem no FixedUpdate para serem consistentes com a física.
    void FixedUpdate()
    {
        // Aplica a rotação horizontal ao Rigidbody
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * yRotation * Time.fixedDeltaTime));

        // Aplica o movimento ao Rigidbody
        rb.linearVelocity = new Vector3(playerVelocity.x, rb.linearVelocity.y, playerVelocity.z);
    }

    // As funções HandleMovement e HandleRotation foram substituídas pela lógica acima.

    public void SetLookSensitivity(float newSensitivity)
    {
        lookSensitivity = newSensitivity;
    }

    // O resto do código permanece 100% intacto.
    #region Funções Intactas
    private void HandleShooting()
    {
        if (InputManager.Instance.IsShooting) { Shoot(); }
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
        UpdateHud();
        if (currentHealth <= 0) { Die(); }
    }
    public bool Heal(float amount)
    {
        if (currentHealth >= maxHealth) return false;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHud();
        return true;
    }
    public bool AddArmor(float amount)
    {
        if (currentArmor >= maxArmor) return false;
        currentArmor += amount;
        currentArmor = Mathf.Clamp(currentArmor, 0f, maxArmor);
        UpdateHud();
        return true;
    }
    private void Shoot()
    {
        if (currentWeapon != null) { currentWeapon.TryToShoot(); }
    }
    private void Die()
    {
        FindObjectOfType<GameOverTrigger>().TriggerGameOver();
    }
    private void UpdateHud()
    {
        if (hudManager != null) { hudManager.UpdateStatus((int)currentHealth, (int)currentArmor); }
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
            playerCamera.transform.localPosition = new Vector3(cameraDefaultPosition.x + horizontalOffset, cameraDefaultPosition.y + verticalOffset, cameraDefaultPosition.z);
        }
        else
        {
            walkingTime = 0;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraDefaultPosition, Time.deltaTime * 5f);
        }
    }
    #endregion
}