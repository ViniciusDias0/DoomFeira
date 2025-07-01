// GameOverTrigger.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        // Encontra o GameManager na cena para poder se comunicar com ele.
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameOverTrigger: GameManager n�o encontrado na cena!");
        }
    }

    // Outro script (como o de vida do jogador) deve chamar esta fun��o quando o jogo acabar.
    public void TriggerGameOver()
    {
        if (gameManager == null) return;

        // 1. Pega a pontua��o final usando a fun��o que j� existe no seu GameManager.
        int finalScore = gameManager.GetFinalScore();

        // 2. Guarda a pontua��o na nossa "ponte" de dados.
        ScoreData.PlayerScore = finalScore;
        ScoreData.HasNewScore = true;

        // 3. Carrega a cena de Game Over.
        // Certifique-se de que o nome "GameOverScene" est� correto!
        SceneManager.LoadScene("GameOverScene");
    }

    // Use a tecla 'G' para simular o fim do jogo e testar rapidamente.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TriggerGameOver();
        }
    }
}