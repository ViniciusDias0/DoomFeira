// MainMenuController.cs (Sem o t�tulo "Hall of Fame")
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown rankingDropdown;

    [Header("Configura��o de Cenas")]
    public string gameSceneName = "SampleScene";

    private bool isRankingLoaded = false;

    void Start()
    {
        if (rankingDropdown == null)
        {
            Debug.LogError("Ranking Dropdown n�o foi definido no Inspector!");
            return;
        }

        rankingDropdown.ClearOptions();
        rankingDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "VER RANKING" });
        rankingDropdown.RefreshShownValue();

        rankingDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public async void OnDropdownClicked()
    {
        if (isRankingLoaded) return;

        rankingDropdown.ClearOptions();
        rankingDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "Carregando..." });
        rankingDropdown.RefreshShownValue();

        await LoadRanking();
    }

    private async Task LoadRanking()
    {
        while (FirebaseManager.Instance == null || FirebaseManager.Instance.InitializationTask == null)
        {
            await Task.Yield();
        }
        await FirebaseManager.Instance.InitializationTask;

        List<ScoreEntry> topScores = await FirebaseManager.Instance.GetTopScores();

        rankingDropdown.ClearOptions();

        if (topScores.Count == 0)
        {
            rankingDropdown.options.Add(new TMP_Dropdown.OptionData("Nenhum recorde encontrado."));
        }
        else
        {
            // --- ALTERA��O AQUI: A linha que adicionava o t�tulo "HALL OF FAME" foi removida. ---

            // Agora, o loop adiciona diretamente os recordes.
            foreach (var scoreEntry in topScores)
            {
                string entryText = $"{scoreEntry.name} - {scoreEntry.score}";
                rankingDropdown.options.Add(new TMP_Dropdown.OptionData(entryText));
            }
        }

        rankingDropdown.RefreshShownValue();
        isRankingLoaded = true;
    }

    private void OnDropdownValueChanged(int index)
    {
        // Esta l�gica ainda � �til. Se o jogador selecionar um item,
        // o dropdown "fecha" visualmente, mostrando o texto inicial.
        // Para fazer isso, precisamos adicionar o texto inicial de volta.
        if (index >= 0) // Qualquer sele��o
        {
            // Limpa a lista de ranking e adiciona o texto inicial de volta
            rankingDropdown.ClearOptions();
            rankingDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "VER RANKING" });
            rankingDropdown.SetValueWithoutNotify(0); // Mostra o texto "VER RANKING"
            isRankingLoaded = false; // Permite que o ranking seja recarregado da pr�xima vez
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}