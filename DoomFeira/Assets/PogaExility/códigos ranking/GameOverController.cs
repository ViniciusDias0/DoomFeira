// GameOverController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Para TextMeshPro
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameOverController : MonoBehaviour
{
    [Header("Referências da UI")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI rankingText;
    public GameObject nameEntryPanel;
    public TextMeshProUGUI nameInputText;

    [Header("Configuração")]
    public string mainMenuSceneName = "MainMenuScene"; // Nome da cena do menu para o botão de voltar

    // Variáveis internas
    private int playerFinalScore;
    private char[] currentName = { 'A', 'A', 'A' };
    private int currentLetterIndex = 0;
    private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private int[] charIndices = { 0, 0, 0 };

    async void Start()
    {
        // Esconde o painel de entrada de nome no início.
        nameEntryPanel.SetActive(false);

        // Pega a pontuação da nossa "ponte".
        playerFinalScore = ScoreData.PlayerScore;
        finalScoreText.text = $"SUA PONTUAÇÃO: {playerFinalScore}";
        rankingText.text = "CARREGANDO...";

        // Carrega o ranking e verifica se o jogador fez um novo recorde.
        await LoadRankingAndCheckForHighScore();
    }

    void Update()
    {
        // Só processa o input de nome se o painel estiver visível.
        if (nameEntryPanel.activeSelf)
        {
            HandleNameInput();
        }
    }

    async Task LoadRankingAndCheckForHighScore()
    {
        if (FirebaseManager.Instance == null)
        {
            rankingText.text = "ERRO DE CONEXÃO";
            return;
        }

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();
        DisplayRanking(topScores);

        // Se viemos de um jogo E a pontuação é alta o suficiente, mostra o painel de nome.
        bool isNewHighScore = ScoreData.HasNewScore && (topScores.Count < 10 || playerFinalScore > topScores.Last().score);
        if (isNewHighScore)
        {
            nameEntryPanel.SetActive(true);
            UpdateNameInputDisplay();
        }

        // Reseta a flag para não mostrar de novo.
        ScoreData.HasNewScore = false;
    }

    void DisplayRanking(List<ScoreEntry> scores)
    {
        StringBuilder sb = new StringBuilder("--- HALL OF FAME ---\n\n");
        for (int i = 0; i < scores.Count; i++)
        {
            sb.AppendLine($"{(i + 1)}. {scores[i].name}   {scores[i].score}");
        }
        rankingText.text = sb.ToString();
    }

    // Controle para digitar o nome com as setas e espaço.
    private void HandleNameInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) currentLetterIndex = (currentLetterIndex + 1) % 3;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) currentLetterIndex = (currentLetterIndex - 1 + 3) % 3;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) charIndices[currentLetterIndex] = (charIndices[currentLetterIndex] + 1) % alphabet.Length;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) charIndices[currentLetterIndex] = (charIndices[currentLetterIndex] - 1 + alphabet.Length) % alphabet.Length;
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SubmitScore();
            return; // Sai da função
        }
        else return; // Sai se nenhuma tecla relevante foi pressionada

        currentName[currentLetterIndex] = alphabet[charIndices[currentLetterIndex]];
        UpdateNameInputDisplay();
    }

    private void UpdateNameInputDisplay()
    {
        StringBuilder displayName = new StringBuilder();
        for (int i = 0; i < 3; i++)
        {
            // Destaca a letra selecionada com uma cor diferente
            if (i == currentLetterIndex)
            {
                displayName.Append($"<color=yellow>{currentName[i]}</color>");
            }
            else
            {
                displayName.Append(currentName[i]);
            }
        }
        nameInputText.text = displayName.ToString();
    }

    private async void SubmitScore()
    {
        // Desativa o painel para o jogador não enviar de novo.
        nameEntryPanel.SetActive(false);
        string finalName = new string(currentName);

        rankingText.text += "\n\nSALVANDO RECORDE...";

        // Usa o FirebaseManager para salvar a pontuação.
        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);

        // Recarrega o ranking para mostrar a nova entrada imediatamente.
        await LoadRankingAndCheckForHighScore();
    }

    // Função pública para um botão de UI chamar
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}