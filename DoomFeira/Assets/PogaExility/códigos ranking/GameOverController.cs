// GameOverController.cs (Versão com InputField)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Essencial para usar o componente Button
using TMPro; // Essencial para InputField e Text
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

    // --- MUDANÇAS AQUI ---
    // Trocamos o antigo Text por um InputField e um Button.
    public TMP_InputField nameInputField;
    public Button confirmButton;

    [Header("Configuração")]
    public string mainMenuSceneName = "MainMenuScene";

    private int playerFinalScore;

    // (As variáveis do sistema de setas foram removidas)

    async void Start()
    {
        nameEntryPanel.SetActive(false);
        playerFinalScore = ScoreData.PlayerScore;
        finalScoreText.text = $"SUA PONTUAÇÃO: {playerFinalScore}";
        rankingText.text = "CARREGANDO...";

        // Configura os listeners dos nossos novos componentes de UI.
        if (confirmButton != null)
        {
            // Conecta a função ConfirmName ao clique do botão.
            confirmButton.onClick.AddListener(ConfirmName);
        }
        if (nameInputField != null)
        {
            // Conecta uma função para validar o nome toda vez que o texto mudar.
            nameInputField.onValueChanged.AddListener(ValidateName);
        }

        await LoadRankingAndCheckForHighScore();
    }

    void Update()
    {
        // Se o painel de nome NÃO estiver ativo, o jogador pode voltar ao menu.
        if (!nameEntryPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
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

            // Foca automaticamente no campo de input para o jogador começar a digitar.
            nameInputField.Select();
            nameInputField.ActivateInputField();

            // Desativa o botão de confirmar no início.
            ValidateName(nameInputField.text);
        }
        else
        {
            sb.Append("\n\nPRESSIONE ESPAÇO PARA VOLTAR AO MENU");
        }

        rankingText.text = sb.ToString();
        ScoreData.HasNewScore = false;
    }

    // --- NOVA FUNÇÃO PARA VALIDAR O INPUT ---
    // Esta função é chamada toda vez que o jogador digita algo.
    private void ValidateName(string currentText)
    {
        // O botão de confirmar só fica clicável se houver algum texto no campo.
        if (confirmButton != null)
        {
            confirmButton.interactable = !string.IsNullOrWhiteSpace(currentText);
        }
    }

    // A função de submeter agora pega o texto do InputField.
    private async void SubmitScore()
    {
        // Pega o texto, remove espaços em branco no início e no fim.
        string finalName = nameInputField.text.Trim();

        // Segurança extra: não envia se o nome estiver vazio.
        if (string.IsNullOrWhiteSpace(finalName))
        {
            return;
        }

        nameEntryPanel.SetActive(false);

        // Mostra o ranking com uma mensagem de "Salvando...".
        rankingText.text += "\n\nSALVANDO RECORDE...";

        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);

        // Recarrega o ranking para mostrar a nova pontuação.
        await LoadRankingAndCheckForHighScore();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Esta função agora é chamada apenas pelo botão de confirmar.
    public void ConfirmName()
    {
        SubmitScore();
    }

    // (As funções de setas foram removidas pois não são mais necessárias)
}