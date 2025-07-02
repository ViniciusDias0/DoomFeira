// GameOverController.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para carregar cenas
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

    // Variáveis internas
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

    // --- INÍCIO DA ALTERAÇÃO 1 ---
    // A função Update agora tem duas responsabilidades distintas
    void Update()
    {
        // 1. Se o painel de nome está ativo, estamos em "modo de digitação".
        if (nameEntryPanel.activeSelf)
        {
            HandleNameInput();
        }
        // 2. Senão, estamos em "modo de visualização do ranking".
        else
        {
            // Neste modo, a tecla Espaço leva de volta ao menu.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GoToMainMenu();
            }
        }
    }
    // --- FIM DA ALTERAÇÃO 1 ---

    async Task LoadRankingAndCheckForHighScore()
    {
        if (FirebaseManager.Instance == null)
        {
            rankingText.text = "ERRO DE CONEXÃO";
            return;
        }

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();

        // Vamos construir o texto do ranking aqui
        StringBuilder sb = new StringBuilder("--- HALL OF FAME ---\n\n");
        for (int i = 0; i < topScores.Count; i++)
        {
            sb.AppendLine($"{(i + 1)}. {topScores[i].name}   {topScores[i].score}");
        }

        // Verificamos se é um novo recorde
        bool isNewHighScore = ScoreData.HasNewScore && (topScores.Count < 10 || playerFinalScore > topScores.Last().score);

        if (isNewHighScore)
        {
            // Se for, ativamos o painel de nome e não adicionamos a instrução de voltar.
            nameEntryPanel.SetActive(true);
            UpdateNameInputDisplay();
        }
        // --- INÍCIO DA ALTERAÇÃO 2 ---
        else
        {
            // Se não for um novo recorde (ou se já salvamos), adicionamos a instrução.
            sb.Append("\n\nPRESSIONE ESPAÇO PARA VOLTAR AO MENU");
        }
        // --- FIM DA ALTERAÇÃO 2 ---

        // Atualiza o texto na tela com o resultado final
        rankingText.text = sb.ToString();
        ScoreData.HasNewScore = false;
    }

    private void HandleNameInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) currentLetterIndex = (currentLetterIndex + 1) % 3;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) currentLetterIndex = (currentLetterIndex - 1 + 3) % 3;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) charIndices[currentLetterIndex] = (charIndices[currentLetterIndex] + 1) % alphabet.Length;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) charIndices[currentLetterIndex] = (charIndices[currentLetterIndex] - 1 + alphabet.Length) % alphabet.Length;
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SubmitScore();
            return;
        }
        else return;

        currentName[currentLetterIndex] = alphabet[charIndices[currentLetterIndex]];
        UpdateNameInputDisplay();
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

        // Envia o novo recorde para o Firebase.
        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);

        // Recarrega o ranking. A função agora irá automaticamente adicionar a
        // instrução "Pressione espaço para voltar".
        await LoadRankingAndCheckForHighScore();
    }

    // Esta função carrega a cena do menu principal.
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}