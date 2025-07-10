using UnityEngine;
using UnityEngine.SceneManagement;

// A linha abaixo pode ser removida se você não a estiver usando mais.
// using UnityEngine.Experimental.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento e Tiro")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    // ... Suas outras variáveis (status, HUD, etc.) permanecem as mesmas ...
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

    // --- ADIÇÃO CRUCIAL ---
    private Rigidbody rb;
    // --- FIM DA ADIÇÃO ---


    void Start()
    {
        // --- ADIÇÃO CRUCIAL ---
        // Pega o componente Rigidbody no mesmo objeto
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerController precisa de um componente Rigidbody para funcionar!", this.gameObject);
            this.enabled = false;
            return;
        }
        // --- FIM DA ADIÇÃO ---

        playerCamera = GetComponentInChildren<Camera>();

        // A lógica do cursor é controlada pelo InputManager, então podemos remover isso daqui.
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        currentHealth = maxHealth;
        currentArmor = 0f;

        if (hudManager != null)
        {
            hudManager.UpdateStatus((int)currentHealth, (int)currentArmor);
        }

        if (playerCamera != null)
        {
            cameraDefaultPosition = playerCamera.transform.localPosition;
        }
    }

    void Update()
    {
        if (InputManager.Instance == null) return;

        // A lógica de tiro e efeitos visuais permanece em Update()
        HandleShooting();
        HandleDebugInputs();
        HandleHeadBob();
    }

    // --- NOVA FUNÇÃO E LÓGICA DE MOVIMENTO ---
    // FixedUpdate é chamada em um intervalo de tempo fixo, ideal para física.
    void FixedUpdate()
    {
        if (InputManager.Instance == null) return;

        // Pega os inputs diretamente do InputManager.
        float moveVertical = InputManager.Instance.VerticalAxis;
        float rotationHorizontal = InputManager.Instance.HorizontalAxis; // <<-- A rotação SEMPRE virá daqui agora.

        // --- LÓGICA DE MOVIMENTO E ROTAÇÃO CORRIGIDA PARA O ESTILO DOOM ---

        // ROTAÇÃO:
        // Usa o input horizontal (Setas Esquerda/Direita ou joystick) para girar o jogador.
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotationHorizontal * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // MOVIMENTO PARA FRENTE/TRÁS:
        // Usa o input vertical (Setas Cima/Baixo ou joystick) para mover para frente e para trás.
        Vector3 targetVelocity = transform.forward * moveVertical * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y; // Mantém a velocidade vertical atual (gravidade).
        rb.linearVelocity = targetVelocity;
    }
    // --- FIM DA NOVA LÓGICA DE MOVIMENTO ---

    // A sua função HandleMovement() antiga pode ser DELETADA, pois foi substituída por FixedUpdate.

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