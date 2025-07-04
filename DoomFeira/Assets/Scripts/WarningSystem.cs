using UnityEngine;
using TMPro; // Necessário para usar o TextMeshPro

public class WarningSystem : MonoBehaviour
{
    [Header("Configuração do Alerta")]
    public TextMeshProUGUI warningText; // O texto que vamos ativar/desativar
    public float warningRadius = 10f; // A distância máxima para o alerta ser acionado
    public float behindAngleThreshold = -0.1f; // Define o quão "atrás" o inimigo precisa estar

    private Transform playerTransform;
    private Enemy[] allEnemies; // Array para guardar os inimigos
    private float checkTimer = 0f;
    private float checkInterval = 0.2f; // Procura por inimigos 5 vezes por segundo (otimização)

    void Start()
    {
        playerTransform = transform;
        if (warningText == null)
        {
            Debug.LogError("Texto de Alerta (WarningText) não definido!", this.gameObject);
            this.enabled = false;
            return;
        }
        // Garante que o texto comece desligado
        warningText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Otimização: Em vez de procurar inimigos a cada frame, fazemos isso em intervalos
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0;
            CheckForEnemiesBehind();
        }
    }

    void CheckForEnemiesBehind()
    {
        // Encontra todos os objetos com o script Enemy na cena
        // Nota: Para jogos com muitos inimigos, existem sistemas mais otimizados,
        // mas para o nosso caso, isso funciona perfeitamente.
        allEnemies = FindObjectsOfType<Enemy>(); // Você pode incluir RangedEnemy aqui se precisar

        bool isEnemyBehind = false;

        foreach (Enemy enemy in allEnemies)
        {
            // 1. Verifica a distância
            float distanceToEnemy = Vector3.Distance(playerTransform.position, enemy.transform.position);
            if (distanceToEnemy <= warningRadius)
            {
                // 2. Verifica se o inimigo está atrás
                Vector3 directionToEnemy = (enemy.transform.position - playerTransform.position).normalized;

                // O "Dot Product" nos diz se os vetores apontam na mesma direção.
                // Se for < 0, o inimigo está no hemisfério "de trás" do jogador.
                float dotProduct = Vector3.Dot(playerTransform.forward, directionToEnemy);

                if (dotProduct < behindAngleThreshold)
                {
                    isEnemyBehind = true;
                    break; // Encontrou um inimigo, não precisa checar os outros
                }
            }
        }

        // Ativa ou desativa o texto de aviso com base no resultado
        warningText.gameObject.SetActive(isEnemyBehind);
    }
}