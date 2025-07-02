// MainMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Text;

public class MainMenuController : MonoBehaviour
{
    [Header("UI do Ranking")]
    public TextMeshProUGUI rankingText;

    [Header("Configuração de Cenas")]
    public string gameSceneName = "SampleScene";

    async void Start()
    {
        if (FirebaseManager.Instance == null)
        {
            if (rankingText != null) rankingText.text = "ERRO: FIREBASE NÃO INICIADO";
            return;
        }

        if (rankingText != null) rankingText.text = "CARREGANDO RANKING...";

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();

        StringBuilder sb = new StringBuilder("--- HALL OF FAME ---\n\n");
        int count = Mathf.Min(topScores.Count, 10);

        if (count == 0)
        {
            sb.Append("NENHUM RECORDE AINDA!");
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                sb.AppendLine($"{(i + 1)}. {topScores[i].name}   {topScores[i].score}");
            }
        }

        if (rankingText != null)
        {
            rankingText.text = sb.ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Carregando cena: {gameSceneName}");
            SceneManager.LoadScene(gameSceneName);
        }
    }
}