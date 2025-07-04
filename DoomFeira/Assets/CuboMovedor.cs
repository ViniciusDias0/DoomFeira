using UnityEngine;

public class CuboMovedor : MonoBehaviour
{
    [Header("Pontos de Movimento")]
    public Transform startPoint;  // O ponto inicial do movimento
    public Transform endPoint;    // O ponto final do movimento

    [Header("Configurações de Movimento")]
    public float speed = 2.0f; // Velocidade da plataforma
    public float waitTime = 1.0f; // Tempo que a plataforma espera em cada ponta

    private Vector3 targetPosition;
    private bool isWaiting = false;

    void Start()
    {
        // Define a posição inicial e o primeiro alvo
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Pontos de início ou fim não definidos para a plataforma!", this.gameObject);
            this.enabled = false; // Desativa o script para evitar erros
            return;
        }

        transform.position = startPoint.position;
        targetPosition = endPoint.position;
    }

    void Update()
    {
        // Se a plataforma não estiver esperando, mova-a
        if (!isWaiting)
        {
            // Move a plataforma em direção ao alvo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Verifica se a plataforma chegou ao alvo
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                // Chegou! Inicia a espera
                StartCoroutine(WaitAndSwitchTarget());
            }
        }
    }

    // Coroutine para esperar em uma ponta e depois trocar o alvo
    private System.Collections.IEnumerator WaitAndSwitchTarget()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        // Troca o alvo: se estava indo para o fim, agora vai para o início, e vice-versa
        if (targetPosition == endPoint.position)
        {
            targetPosition = startPoint.position;
        }
        else
        {
            targetPosition = endPoint.position;
        }

        isWaiting = false;
    }
}
