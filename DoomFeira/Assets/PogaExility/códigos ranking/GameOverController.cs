// GameOverController.cs (Versão Final Sem Complicação)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class GameOverController : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI rankingText;
    public TMP_InputField nameInputField;
    public Button confirmButton;
    public Button backToMenuButton;

    [Header("Configuração")]
    public string mainMenuSceneName = "MainMenuScene";

    private int playerFinalScore;

    async void Start()
    {
        playerFinalScore = ScoreData.PlayerScore;
        finalScoreText.text = $"SUA PONTUAÇÃO: {playerFinalScore}";
        rankingText.text = "CARREGANDO...";

        // O CAMPO ESTÁ SEMPRE ATIVO.
        nameInputField.interactable = true;
        ValidateName(nameInputField.text);

        // Foca no campo se for PC.
        if (InputManager.Instance.currentScheme == InputManager.ControlScheme.PC)
        {
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }

        SetupButtons();
        await LoadRanking();
    }

    private void SetupButtons()
    {
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmName);
        if (backToMenuButton != null) backToMenuButton.onClick.AddListener(GoToMainMenu);
        if (nameInputField != null)
        {
            nameInputField.onSubmit.AddListener((text) => ConfirmName());
            nameInputField.onValueChanged.AddListener(ValidateName);
        }
    }

    async Task LoadRanking()
    {
        if (FirebaseManager.Instance == null) { rankingText.text = "ERRO"; return; }
        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < topScores.Count; i++)
        {
            sb.AppendLine($"{(i + 1)}. {topScores[i].name}   {topScores[i].score}");
        }
        rankingText.text = sb.ToString();
    }

    private void ValidateName(string currentText)
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = !string.IsNullOrWhiteSpace(currentText);
        }
    }

    public async void ConfirmName()
    {
        string finalName = nameInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(finalName)) return;

        nameInputField.text = "";
        nameInputField.interactable = false;
        confirmButton.interactable = false;

        await FirebaseManager.Instance.AddScore(finalName, playerFinalScore);
        await LoadRanking();

        nameInputField.interactable = true;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    // Adicione esta função inteira ao seu script GameOverController.cs
    void Update()
    {
        // Verifica a cada frame se a tecla Espaço foi pressionada.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Se foi, chama a função que já existe para voltar ao menu.
            GoToMainMenu();
        }
    }
}