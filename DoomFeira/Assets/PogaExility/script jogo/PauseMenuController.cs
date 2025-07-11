// PauseMenuController.cs (Vers�o Final Corrigida)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Globalization;

public class PauseMenuController : MonoBehaviour
{
    [Header("Refer�ncias da UI")]
    public GameObject pauseMenuPanel;
    public TMP_InputField sensitivityInputField;

    public Button openPauseButton;
    public Button resumeButton;
    public Button giveUpButton;

    [Header("Configura��o")]
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

        if (sensitivityInputField != null)
        {
            sensitivityInputField.onEndEdit.AddListener(OnSensitivityChanged);
        }

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

    // --- CORRE��O PRINCIPAL AQUI ---
    // Agora chama a fun��o correta: SetLookSensitivity()
    private void OnSensitivityChanged(string newTextValue)
    {
        if (float.TryParse(newTextValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out float sensitivityValue))
        {
            if (playerController != null)
            {
                // ANTES (ERRADO): playerController.rotationSpeed = sensitivityValue;
                // DEPOIS (CORRETO): Chama a fun��o p�blica que criamos para isso.
                playerController.SetLookSensitivity(sensitivityValue);
            }

            PlayerPrefs.SetFloat("MouseSensitivity", sensitivityValue);
            Debug.Log($"Sensibilidade alterada para: {sensitivityValue}");
            sensitivityInputField.text = sensitivityValue.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            LoadSensitivity();
        }
    }

    // --- CORRE��O PRINCIPAL AQUI TAMB�M ---
    private void LoadSensitivity()
    {
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 50f); // Padr�o mais baixo e sensato

        if (playerController != null)
        {
            // ANTES (ERRADO): playerController.rotationSpeed = savedSens;
            // DEPOIS (CORRETO): Usa a mesma fun��o p�blica para definir o valor inicial.
            playerController.SetLookSensitivity(savedSens);
        }

        if (sensitivityInputField != null)
        {
            sensitivityInputField.text = savedSens.ToString(CultureInfo.InvariantCulture);
        }
    }
}