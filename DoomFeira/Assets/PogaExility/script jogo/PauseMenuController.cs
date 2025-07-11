// PauseMenuController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject pauseMenuPanel;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public Button openPauseButton;
    public Button resumeButton;
    public Button giveUpButton;

    [Header("Configuração")]
    public string mainMenuSceneName = "MainMenuScene";

    private PlayerController playerController;
    private GameOverTrigger gameOverTrigger;

    public static bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        playerController = FindObjectOfType<PlayerController>();
        gameOverTrigger = FindObjectOfType<GameOverTrigger>();

        if (openPauseButton != null) openPauseButton.onClick.AddListener(PauseGame);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (giveUpButton != null) giveUpButton.onClick.AddListener(GiveUpAndQuit);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        LoadSensitivity();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        if (InputManager.Instance != null && InputManager.Instance.currentScheme == InputManager.ControlScheme.PC)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void GiveUpAndQuit()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (gameOverTrigger != null)
        {
            gameOverTrigger.TriggerGameOver();
        }
    }

    private void OnSensitivityChanged(float value)
    {
        if (playerController != null)
        {
            playerController.SetLookSensitivity(value);
        }
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = value.ToString("F0");
        }
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    private void LoadSensitivity()
    {
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
        if (sensitivitySlider != null) sensitivitySlider.value = savedSens;
        if (playerController != null) playerController.SetLookSensitivity(savedSens);
        if (sensitivityValueText != null) sensitivityValueText.text = savedSens.ToString("F0");
    }
}