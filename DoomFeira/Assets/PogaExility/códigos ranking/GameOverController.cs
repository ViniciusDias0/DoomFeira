using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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
    public string mainMenuSceneName = "MainMenuScene";

    private int playerFinalScore;
    private char[] currentName = { 'A', 'A', 'A' };
    private int currentLetterIndex = 0;
    private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private int[] charIndices = { 0, 0, 0 };

    async void Start()
    {
        nameEntryPanel.SetActive(false);
        playerFinalScore = ScoreData.PlayerScore;
        finalScoreText.text = $"SUA PONTUAÇÃO: {playerFinalScore}";
        rankingText.text = "CARREGANDO...";
        await LoadRankingAndCheckForHighScore();
    }

    void Update()
    {
        if (nameEntryPanel.activeSelf)
        {
            HandleNameInput();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GoToMainMenu();
            }
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
        StringBuilder sb = new StringBuilder("--- HALL OF FAME ---\n\n");
        for (int i = 0; i < topScores.Count; i++)
        {
            sb.AppendLine($"{(i + 1)}. {topScores[i].name}   {topScores[i].score}");
        }
        bool isNewHighScore = ScoreData.HasNewScore && (topScores.Count < 10 || playerFinalScore > topScores.Last().score);
        if (isNewHighScore)
        {
            nameEntryPanel.SetActive(true);
            UpdateNameInputDisplay();
        }
        else
        {
            sb.Append("\n\nPRESSIONE ESPAÇO PARA VOLTAR AO MENU");
        }
        rankingText.text = sb.ToString();
        ScoreData.HasNewScore = false;
    }

    private void HandleNameInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveToNextLetter();
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveToPreviousLetter();
        else if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeCharacterUp();
        else if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeCharacterDown();
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) SubmitScore();
    }

    private void UpdateNameInputDisplay()
    {
        StringBuilder displayName = new StringBuilder();
        for (int i = 0; i < 3; i++)
        {
            displayName.Append(i == currentLetterIndex ? $"<color=yellow>{currentName[i]}</color>" : currentName[i].ToString());
        }
        nameInputText.text = displayName.ToString();
    }

    private async void SubmitScore()
    {
        nameEntryPanel.SetActive(false);
        string finalName = new string(currentName);
        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);
        await LoadRankingAndCheckForHighScore();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // --- INÍCIO DAS NOVAS FUNÇÕES PARA OS BOTÕES ---

    // Chame esta função no botão da seta para a DIREITA
    public void MoveToNextLetter()
    {
        currentLetterIndex = (currentLetterIndex + 1) % 3;
        UpdateNameInputDisplay();
    }

    // Chame esta função no botão da seta para a ESQUERDA
    public void MoveToPreviousLetter()
    {
        currentLetterIndex = (currentLetterIndex - 1 + 3) % 3;
        UpdateNameInputDisplay();
    }

    // Chame esta função no botão da seta para CIMA
    public void ChangeCharacterUp()
    {
        charIndices[currentLetterIndex] = (charIndices[currentLetterIndex] + 1) % alphabet.Length;
        currentName[currentLetterIndex] = alphabet[charIndices[currentLetterIndex]];
        UpdateNameInputDisplay();
    }

    // Chame esta função no botão da seta para BAIXO
    public void ChangeCharacterDown()
    {
        charIndices[currentLetterIndex] = (charIndices[currentLetterIndex] - 1 + alphabet.Length) % alphabet.Length;
        currentName[currentLetterIndex] = alphabet[charIndices[currentLetterIndex]];
        UpdateNameInputDisplay();
    }

    // Chame esta função no botão de CONFIRMAR ou OK
    public void ConfirmName()
    {
        SubmitScore();
    }

    // --- FIM DAS NOVAS FUNÇÕES PARA OS BOTÕES ---
}