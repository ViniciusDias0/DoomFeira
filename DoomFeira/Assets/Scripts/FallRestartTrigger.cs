using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para gerenciar cenas

public class FallRestartTrigger : MonoBehaviour
{
    [Tooltip("O nome exato do seu arquivo de cena de jogo.")]
    public string gameSceneName = "GameScene"; // Mude aqui se o nome da sua cena for diferente

    // A função OnTriggerEnter é chamada pela Unity quando um outro Collider (marcado como Trigger) entra neste.
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que tocou no nosso trigger tem a tag "Player".
        if (other.CompareTag("Player"))
        {
            // Se for o jogador, reinicia a cena.
            Debug.Log("Jogador tocou o Kill Plane! Reiniciando a cena...");
            RestartScene();
        }
    }

    private void RestartScene()
    {
        // Carrega a cena do jogo novamente.
        SceneManager.LoadScene(gameSceneName);
    }
}