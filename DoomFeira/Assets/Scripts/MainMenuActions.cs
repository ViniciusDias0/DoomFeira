// MainMenuActions.cs (Vers�o para arrastar o bot�o)
using UnityEngine;
using UnityEngine.UI; // Precisamos disso para acessar o componente Button
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Configura��o da Cena")]
    // Coloque o nome da sua cena de jogo aqui no Inspector.
    public string gameSceneName = "SampleScene";

    [Header("Refer�ncias da UI")]
    // --- ADI��O AQUI ---
    // Criei um campo p�blico para voc� arrastar o seu objeto de texto
    // "Aperte para Iniciar" (que agora tem um componente Button).
    public Button startGameButton;

    void Start()
    {
        // Verifica se voc� arrastou o bot�o para o campo no Inspector.
        if (startGameButton != null)
        {
            // Adiciona a nossa fun��o StartGame() � lista de eventos do bot�o via c�digo.
            // "() => StartGame()" � uma forma curta de dizer "quando clicar, execute a fun��o StartGame".
            startGameButton.onClick.AddListener(() => StartGame());
        }
        else
        {
            // Um aviso �til caso voc� esque�a de arrastar o bot�o.
            Debug.LogWarning("O bot�o 'Start Game' n�o foi definido no Inspector do MainMenuActions.");
        }
    }

    // A fun��o que ser� chamada pelo bot�o.
    public void StartGame()
    {
        Debug.Log($"Iniciando jogo, carregando cena: {gameSceneName}");
        SceneManager.LoadScene(gameSceneName);
    }
}