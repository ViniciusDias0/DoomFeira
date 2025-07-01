using UnityEngine;
using TMPro; // NECESSÁRIO para usar TextMeshPro

public class GameManager : MonoBehaviour
{
    // Variável pública para arrastarmos o nosso texto da UI
    public TextMeshProUGUI scoreText;

    // Variável privada para guardar a pontuação
    private int currentScore;

    void Start()
    {
        // Inicia o jogo com 0 pontos
        currentScore = 0;

        // Atualiza o texto na tela pela primeira vez
        UpdateScoreUI();
    }

    // Função pública que outros scripts (como o do inimigo) vão chamar
    public void AddScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;
        Debug.Log($"Pontuação: {currentScore}"); // Útil para testar no console

        // Atualiza o texto na tela com a nova pontuação
        UpdateScoreUI();
    }

    public int GetFinalScore()
    {
        return currentScore;
    }

    // Função privada que atualiza o elemento de texto
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Pontos: {currentScore}";
        }
    }
}