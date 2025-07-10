// GameOverController.cs (Sem os textos de cabeçalho/rodapé)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameOverController : MonoBehaviour
{
    // ... (Suas variáveis e a função Start continuam exatamente iguais)
    #region Variáveis e Start (Intactos)
    [Header("Referências da UI")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI rankingText;
    public TMP_InputField nameInputField;
    public Button confirmButton;
    public Button backToMenuButton;

    [Header("Configuração")]
    public string mainMenuSceneName = "MainMenuScene";

    private int playerFinalScore;
    private bool isNewHighScore = false;

    async void Start()
    {
        playerFinalScore = ScoreData.PlayerScore;
        finalScoreText.text = $"SUA PONTUAÇÃO: {playerFinalScore}";
        rankingText.text = "CARREGANDO...";
        SetupButtons();
        if (InputManager.Instance != null && InputManager.Instance.currentScheme == InputManager.ControlScheme.PC)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        await LoadRankingAndHandleUI();
    }
    #endregion

    void Update()
    {
        // ... (A função Update continua exatamente igual)
        #region Update (Intacto)
        if (InputManager.Instance == null) return;
        if (isNewHighScore && !nameInputField.interactable &&
            InputManager.Instance.currentScheme == InputManager.ControlScheme.PC && Input.GetKeyDown(KeyCode.Return))
        {
            EnableTypingMode();
        }
        else if (!isNewHighScore && Input.GetKeyDown(KeyCode.Space))
        {
            GoToMainMenu();
        }
        #endregion
    }

    private void SetupButtons()
    {
        // ... (A função SetupButtons continua exatamente igual)
        #region SetupButtons (Intacto)
        if (confirmButton != null) { confirmButton.onClick.AddListener(ConfirmName); }
        if (backToMenuButton != null) { backToMenuButton.onClick.AddListener(GoToMainMenu); }
        if (nameInputField != null)
        {
            nameInputField.onSubmit.AddListener((text) => { ConfirmName(); });
            nameInputField.onValueChanged.AddListener(ValidateName);
        }
        #endregion
    }

    // --- A ÚNICA ALTERAÇÃO FOI FEITA AQUI ---
    async Task LoadRankingAndHandleUI()
    {
        if (FirebaseManager.Instance == null)
        {
            rankingText.text = "ERRO DE CONEXÃO";
            return;
        }

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();
        isNewHighScore = ScoreData.HasNewScore && (topScores.Count < 10 || playerFinalScore > topScores.Last().score);

        // MUDANÇA 1: Começa com o StringBuilder vazio, sem o "HALL OF FAME".
        StringBuilder sb = new StringBuilder();

        // Monta a lista de pontuações
        for (int i = 0; i < topScores.Count; i++)
        {
            sb.AppendLine($"{(i + 1)}. {topScores[i].name}   {topScores[i].score}");
        }
        rankingText.text = sb.ToString();

        // A lógica para habilitar a entrada de nome continua a mesma
        if (isNewHighScore)
        {
            if (InputManager.Instance.currentScheme == InputManager.ControlScheme.Mobile)
            {
                EnableTypingMode();
            }
            else
            {
                nameInputField.interactable = false;
                ValidateName(nameInputField.text);
            }
        }
        else // Se não for um novo recorde
        {
            nameInputField.interactable = false;
            confirmButton.interactable = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            // MUDANÇA 2: A linha que adicionava "PRESSIONE ESPAÇO..." foi removida.
        }

        ScoreData.HasNewScore = false;
    }
    // --- FIM DA ALTERAÇÃO ---

    // ... (O resto das funções (EnableTypingMode, ValidateName, etc.) continuam exatamente iguais)
    #region Funções Restantes (Intactas)
    private void EnableTypingMode()
    {
        nameInputField.interactable = true;
        nameInputField.Select();
        nameInputField.ActivateInputField();
        ValidateName(nameInputField.text);
    }

    private void ValidateName(string currentText)
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = nameInputField.interactable && !string.IsNullOrWhiteSpace(currentText);
        }
    }

    private async void SubmitScore()
    {
        string finalName = nameInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(finalName)) return;
        isNewHighScore = false;
        nameInputField.interactable = false;
        confirmButton.interactable = false;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);
        await LoadRankingAndHandleUI();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ConfirmName()
    {
        SubmitScore();
    }
    #endregion
}