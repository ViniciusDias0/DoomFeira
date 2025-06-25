using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float shootDistance = 100f;
    private Camera playerCamera;
    public LayerMask shootableMask; // Adicione esta linha

    // --- Vari�veis de Stats e HUD ---
    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int maxArmor = 100;
    public int currentArmor;

    [Header("HUD")]
    public HUDManager hudManager; // Refer�ncia para o nosso script da HUD

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxHealth;
        currentArmor = 0; // Come�a sem armadura

        // Atualiza a HUD pela primeira vez
        hudManager.UpdateHUD(currentHealth, currentArmor);
    }

    public void TakeDamage(int damage)
    {
        // O dano primeiro atinge a armadura
        int damageToArmor = Mathf.Min(currentArmor, damage);
        currentArmor -= damageToArmor;

        // O dano restante atinge a vida
        int remainingDamage = damage - damageToArmor;
        currentHealth -= remainingDamage;

        // Garante que a vida n�o fique negativa
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log($"Vida: {currentHealth}, Armadura: {currentArmor}");

        // ATUALIZA A HUD!
        hudManager.UpdateHUD(currentHealth, currentArmor);

        // L�gica de morte (opcional por enquanto)
        if (currentHealth <= 0)
        {
            Debug.Log("JOGADOR MORREU!");
            // Aqui voc� pode reiniciar a fase ou mostrar uma tela de Game Over
        }
    }


    void Update()
    {
        float moveVertical = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime);

        float rotationHorizontal = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * rotationHorizontal * rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // ... seu c�digo antigo do Update() ...

        // --- TESTE DE DANO ---
        // Aperte 'T' para tomar 10 de dano
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
        // Aperte 'Y' para pegar 25 de armadura
        if (Input.GetKeyDown(KeyCode.Y))
        {
            currentArmor = Mathf.Min(currentArmor + 25, maxArmor);
            hudManager.UpdateHUD(currentHealth, currentArmor);


        }

        void Shoot()
        {
            // Adicionamos um Debug para saber se a fun��o � chamada
            Debug.Log("Atirando!");

            RaycastHit hit;

            // Dispara o raio com a m�scara
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootDistance, shootableMask))
            {
                // Se acertar algo, dizemos O QUE e em QUAL CAMADA est�
                Debug.Log("Acertei: " + hit.transform.name + " | Na camada: " + LayerMask.LayerToName(hit.transform.gameObject.layer));

                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log("� um inimigo! Destruindo...");
                    enemy.Die();
                }
                else
                {
                    Debug.Log("N�o � um inimigo.");
                }
            }
            else
            {
                // Se o raio n�o acertar NADA (nem inimigo, nem parede, nada)
                Debug.Log("N�o acertei nada na dist�ncia do tiro.");
            }
        }
    }
}

