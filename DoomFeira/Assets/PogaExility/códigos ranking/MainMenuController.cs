// MainMenuController.cs (Sua versão, com a única alteração solicitada)
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Text;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    [Header("UI do Ranking")]
    public TextMeshProUGUI rankingText;

    [Header("Configuração de Cenas")]
    public string gameSceneName = "SampleScene";

    private void OnEnable()
    {
        LoadRanking();
    }

    private async void LoadRanking()
    {
        // Esta parte do código que garante a inicialização está ótima e não precisa de mudanças.
        while (FirebaseManager.Instance == null)
        {
            await Task.Yield();
        }
        while (FirebaseManager.Instance.InitializationTask == null)
        {
            await Task.Yield();
        }
        await FirebaseManager.Instance.InitializationTask;

        if (rankingText != null) rankingText.text = "CARREGANDO RANKING...";

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();

        Debug.Log($"MainMenuController: Foram encontrados {topScores.Count} recordes no Firebase.");

        // --- A ÚNICA MUDANÇA ESTÁ AQUI ---
        // Em vez de começar com o cabeçalho, o StringBuilder agora começa vazio.
        StringBuilder sb = new StringBuilder("");

        int count = Mathf.Min(topScores.Count, 10);

        if (count == 0)
        {
            sb.Append("NENHUM RECORDE AINDA!");
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                // A quebra de linha \n no final garante o espaçamento entre as linhas.
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