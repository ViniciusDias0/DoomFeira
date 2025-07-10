// MainMenuController.cs (Sua vers�o, com a �nica altera��o solicitada)
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

    [Header("Configura��o de Cenas")]
    public string gameSceneName = "SampleScene";

    private void OnEnable()
    {
        LoadRanking();
    }

    private async void LoadRanking()
    {
        // Esta parte do c�digo que garante a inicializa��o est� �tima e n�o precisa de mudan�as.
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

        // --- A �NICA MUDAN�A EST� AQUI ---
        // Em vez de come�ar com o cabe�alho, o StringBuilder agora come�a vazio.
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
                // A quebra de linha \n no final garante o espa�amento entre as linhas.
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