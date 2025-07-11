// PlayerController.cs (Corrigido para FPS com Toque)
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Rotação")]
    public float moveSpeed = 5f;
    [Tooltip("Sensibilidade da rotação para mouse e toque.")]
    public float lookSensitivity = 100f; // Esta é a variável que o menu de pause vai controlar

    // Removida a 'rotationSpeed' para evitar confusão.

    // ... (O resto das suas variáveis continua igual) ...
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

    private float cameraVerticalRotation = 0f;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        currentHealth = maxHealth;
        currentArmor = 0f;
        if (hudManager != null) { hudManager.UpdateStatus((int)currentHealth, (int)currentArmor); }
        if (playerCamera != null) { cameraDefaultPosition = playerCamera.transform.localPosition; }
    }

    void Update()
    {
        if (InputManager.Instance == null) return;

        // O Update agora cuida da ROTAÇÃO e do TIRO.
        HandleRotation();
        HandleShooting();
        HandleDebugInputs();
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        // O FixedUpdate cuida do MOVIMENTO (física).
        HandleMovement();
    }

    // --- LÓGICA DE ROTAÇÃO CORRIGIDA ---
    private void HandleRotation()
    {
        // 1. Rotação Horizontal (corpo do jogador)
        // O jogador vira para os lados com o input de olhar (mouse ou toque).
        float lookX = InputManager.Instance.LookX * lookSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * lookX);

        // 2. Rotação Vertical (apenas a câmera)
        // A câmera olha para cima e para baixo.
        cameraVerticalRotation -= InputManager.Instance.LookY * lookSensitivity * Time.deltaTime;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);
    }

    // --- LÓGICA DE MOVIMENTO CORRIGIDA ---
    private void HandleMovement()
    {
        if (InputManager.Instance == null) return;

        // O jogador se move para frente/trás/lados com o input de movimento (teclado ou joystick).
        float moveVertical = InputManager.Instance.VerticalAxis;
        float moveHorizontal = InputManager.Instance.HorizontalAxis;

        Vector3 moveDirection = (transform.forward * moveVertical) + (transform.right * moveHorizontal);
        transform.position += moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
    }

    // --- FUNÇÃO PÚBLICA PARA O MENU DE PAUSE ---
    // Esta função permite que o PauseMenuController altere a sensibilidade.
    public void SetLookSensitivity(float newSensitivity)
    {
        lookSensitivity = newSensitivity;
    }

    // ... (O resto das suas funções (HandleShooting, TakeDamage, etc.) permanecem intactas) ...
    #region Funções Intactas
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