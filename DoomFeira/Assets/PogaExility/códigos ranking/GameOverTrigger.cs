// GameOverTrigger.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameOverTrigger: GameManager não encontrado na cena!");
        }
    }

    public void TriggerGameOver()
    {
        if (gameManager == null) return;

        int finalScore = gameManager.GetFinalScore();

        ScoreData.PlayerScore = finalScore;
        ScoreData.HasNewScore = true;

        SceneManager.LoadScene("GameOverScene");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TriggerGameOver();
        }
    }
}