// GameOverController.cs (Vers�o com InputField)
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
    [Header("Refer�ncias da UI")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI rankingText;
    public GameObject nameEntryPanel;

    // --- MUDAN�AS AQUI ---
    // Trocamos o antigo Text por um InputField e um Button.
    public TMP_InputField nameInputField;
    public Button confirmButton;

    [Header("Configura��o")]
    public string mainMenuSceneName = "MainMenuScene";

    private int playerFinalScore;

    // (As vari�veis do sistema de setas foram removidas)

    async void Start()
    {
        nameEntryPanel.SetActive(false);
        playerFinalScore = ScoreData.PlayerScore;
        finalScoreText.text = $"SUA PONTUA��O: {playerFinalScore}";
        rankingText.text = "CARREGANDO...";

        // Configura os listeners dos nossos novos componentes de UI.
        if (confirmButton != null)
        {
            // Conecta a fun��o ConfirmName ao clique do bot�o.
            confirmButton.onClick.AddListener(ConfirmName);
        }
        if (nameInputField != null)
        {
            // Conecta uma fun��o para validar o nome toda vez que o texto mudar.
            nameInputField.onValueChanged.AddListener(ValidateName);
        }

        await LoadRankingAndCheckForHighScore();
    }

    void Update()
    {
        // Se o painel de nome N�O estiver ativo, o jogador pode voltar ao menu.
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
            rankingText.text = "ERRO DE CONEX�O";
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

            // Foca automaticamente no campo de input para o jogador come�ar a digitar.
            nameInputField.Select();
            nameInputField.ActivateInputField();

            // Desativa o bot�o de confirmar no in�cio.
            ValidateName(nameInputField.text);
        }
        else
        {
            sb.Append("\n\nPRESSIONE ESPA�O PARA VOLTAR AO MENU");
        }

        rankingText.text = sb.ToString();
        ScoreData.HasNewScore = false;
    }

    // --- NOVA FUN��O PARA VALIDAR O INPUT ---
    // Esta fun��o � chamada toda vez que o jogador digita algo.
    private void ValidateName(string currentText)
    {
        // O bot�o de confirmar s� fica clic�vel se houver algum texto no campo.
        if (confirmButton != null)
        {
            confirmButton.interactable = !string.IsNullOrWhiteSpace(currentText);
        }
    }

    // A fun��o de submeter agora pega o texto do InputField.
    private async void SubmitScore()
    {
        // Pega o texto, remove espa�os em branco no in�cio e no fim.
        string finalName = nameInputField.text.Trim();

        // Seguran�a extra: n�o envia se o nome estiver vazio.
        if (string.IsNullOrWhiteSpace(finalName))
        {
            return;
        }

        nameEntryPanel.SetActive(false);

        // Mostra o ranking com uma mensagem de "Salvando...".
        rankingText.text += "\n\nSALVANDO RECORDE...";

        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);

        // Recarrega o ranking para mostrar a nova pontua��o.
        await LoadRankingAndCheckForHighScore();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Esta fun��o agora � chamada apenas pelo bot�o de confirmar.
    public void ConfirmName()
    {
        SubmitScore();
    }

    // (As fun��es de setas foram removidas pois n�o s�o mais necess�rias)
}