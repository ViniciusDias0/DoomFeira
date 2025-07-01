// MainMenuController.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI rankingText;

    async void Start()
    {
        if (FirebaseManager.Instance == null)
        {
            if (rankingText != null) rankingText.text = "ERRO: FIREBASE NÃO INICIADO";
            return;
        }

        // Espera a inicialização do Firebase terminar.
        await FirebaseManager.Instance.InitializationTask;

        if (rankingText != null) rankingText.text = "CARREGANDO RANKING...";

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();

        StringBuilder sb = new StringBuilder("--- HALL OF FAME ---\n\n");

        int count = Mathf.Min(topScores.Count, 10);
        for (int i = 0; i < count; i++)
        {
            sb.AppendLine($"{(i + 1)}. {topScores[i].name}   {topScores[i].score}");
        }

        if (count == 0)
        {
            sb.Append("\nNENHUM RECORDE AINDA!");
        }

        // --- A CORREÇÃO ESTÁ AQUI ---
        // Usando o nome correto da variável: 'rankingText' com 'T' maiúsculo.
        if (rankingText != null)
        {
            rankingText.text = sb.ToString();
        }
    }
}