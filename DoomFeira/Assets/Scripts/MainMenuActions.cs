// MainMenuActions.cs (Versão para arrastar o botão)
using UnityEngine;
using UnityEngine.UI; // Precisamos disso para acessar o componente Button
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Configuração da Cena")]
    // Coloque o nome da sua cena de jogo aqui no Inspector.
    public string gameSceneName = "SampleScene";

    [Header("Referências da UI")]
    // --- ADIÇÃO AQUI ---
    // Criei um campo público para você arrastar o seu objeto de texto
    // "Aperte para Iniciar" (que agora tem um componente Button).
    public Button startGameButton;

    void Start()
    {
        // Verifica se você arrastou o botão para o campo no Inspector.
        if (startGameButton != null)
        {
            // Adiciona a nossa função StartGame() à lista de eventos do botão via código.
            // "() => StartGame()" é uma forma curta de dizer "quando clicar, execute a função StartGame".
            startGameButton.onClick.AddListener(() => StartGame());
        }
        else
        {
            // Um aviso útil caso você esqueça de arrastar o botão.
            Debug.LogWarning("O botão 'Start Game' não foi definido no Inspector do MainMenuActions.");
        }
    }

    // A função que será chamada pelo botão.
    public void StartGame()
    {
        Debug.Log($"Iniciando jogo, carregando cena: {gameSceneName}");
        SceneManager.LoadScene(gameSceneName);
    }
}