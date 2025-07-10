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
    public bool enableHeadBob = true;
    public float bobFrequency = 2.0f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;

    private float walkingTime = 0.0f;
    private Vector3 cameraDefaultPosition;

    [Header("Equipamento")]
    public WeaponStats currentWeapon;


    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxHealth;
        currentArmor = 0f;

        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }

        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            cameraDefaultPosition = playerCamera.transform.localPosition;
        }

    }

    // --- A CORREÇÃO ESTÁ AQUI ---
    void Update()
    {
        // Se o InputManager não existir por algum motivo, não faz nada.
        if (InputManager.Instance == null) return;

        // A lógica de movimento e tiro agora é chamada em TODOS os frames.
        // O InputManager é quem decide de onde vem o input (teclado ou joystick).
        HandleMovement();
        HandleShooting();

        // Os inputs de debug e headbob continuam funcionando normalmente.
        HandleDebugInputs();
        HandleHeadBob();
    }
    // --- FIM DA CORREÇÃO ---

    private void HandleMovement()
    {
        float moveVertical = InputManager.Instance.VerticalAxis;
        transform.Translate(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime);

        float rotationHorizontal = InputManager.Instance.HorizontalAxis;
        transform.Rotate(Vector3.up * rotationHorizontal * rotationSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (InputManager.Instance.IsShooting)
        {
            Shoot();
        }
    }

    private void HandleDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddArmor(25f);
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
}