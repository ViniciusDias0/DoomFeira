// MainMenuController.cs (Sem o título "Hall of Fame")
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown rankingDropdown;

    [Header("Configuração de Cenas")]
    public string gameSceneName = "SampleScene";

    private bool isRankingLoaded = false;

    void Start()
    {
        if (rankingDropdown == null)
        {
            Debug.LogError("Ranking Dropdown não foi definido no Inspector!");
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
            // --- ALTERAÇÃO AQUI: A linha que adicionava o título "HALL OF FAME" foi removida. ---

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
        // Esta lógica ainda é útil. Se o jogador selecionar um item,
        // o dropdown "fecha" visualmente, mostrando o texto inicial.
        // Para fazer isso, precisamos adicionar o texto inicial de volta.
        if (index >= 0) // Qualquer seleção
        {
            // Limpa a lista de ranking e adiciona o texto inicial de volta
            rankingDropdown.ClearOptions();
            rankingDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "VER RANKING" });
            rankingDropdown.SetValueWithoutNotify(0); // Mostra o texto "VER RANKING"
            isRankingLoaded = false; // Permite que o ranking seja recarregado da próxima vez
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