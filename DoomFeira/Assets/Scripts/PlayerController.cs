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
            hudManager.UpdateHUD((int)currentHealth, (int)currentArmor);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleDebugInputs();
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
        if (Input.GetKeyDown(KeyCode.Space))
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
            hudManager.UpdateHUD((int)currentHealth, (int)currentArmor);
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
            hudManager.UpdateHUD((int)currentHealth, (int)currentArmor);
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
            hudManager.UpdateHUD((int)currentHealth, (int)currentArmor);
        }

        return true;
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootDistance, shootableMask))
        {
            Enemy meleeEnemy = hit.transform.GetComponent<Enemy>();
            if (meleeEnemy != null)
            {
                meleeEnemy.Die(); // Mata o inimigo fraco com um tiro
                return; // Sai da função para não checar o outro tipo
            }

            RangedEnemy rangedEnemy = hit.transform.GetComponent<RangedEnemy>();
            if (rangedEnemy != null)
            {
                // Causa dano a ele. Vamos supor que seu tiro dá 50 de dano.
                rangedEnemy.TakeDamage(50f);
            }

            Debug.Log("Acertei: " + hit.transform.name);
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }

    }

    private void Die()
    {
        Debug.Log("GAME OVER! Reiniciando...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Função central para atualizar o HUD, para não repetir código
    private void UpdateHud()
    {
        if (hudManager != null)
        {
            hudManager.UpdateHUD((int)currentHealth, (int)currentArmor);
        }
    }
}