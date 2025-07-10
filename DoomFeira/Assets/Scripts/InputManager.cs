using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    // ... (suas variáveis e Singleton) ...
    public static InputManager Instance { get; private set; }
    public enum ControlScheme { PC, Mobile }
    public ControlScheme currentScheme;
    private GameObject mobileControlsUI;
    private Joystick movementJoystick;
    public float VerticalAxis { get; private set; }
    public float HorizontalAxis { get; private set; }
    public float MouseX { get; private set; }
    public bool IsShooting { get; private set; }


    void Awake()
    {
        // ... (seu código Awake permanece o mesmo) ...
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        currentScheme = (ControlScheme)PlayerPrefs.GetInt("ControlScheme", 0);
    }

    // ... (OnSceneLoaded permanece a mesma) ...
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentScheme == ControlScheme.Mobile)
        {
            mobileControlsUI = GameObject.Find("MobileControlsContainer");
            if (mobileControlsUI != null)
            {
                movementJoystick = mobileControlsUI.GetComponentInChildren<Joystick>();
            }
        }
        ApplyScheme(scene.name);
    }

    void Update()
    {
        // --- ADIÇÃO CRUCIAL AQUI ---
        // Verifica a cada frame se estamos na cena do menu.
        if (SceneManager.GetActiveScene().name == "MainMenuScene") // <<-- Use o nome exato da sua cena de menu
        {
            // Se estivermos no menu, FORÇA o cursor a ficar visível e destravado.
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }
        }
        // --- FIM DA ADIÇÃO ---


        // O resto da sua lógica de Update continua normalmente
        switch (currentScheme)
        {
            case ControlScheme.PC: ReadPCInput(); break;
            case ControlScheme.Mobile: ReadMobileInput(); break; // <<-- LINHA CORRIGIDA
        }
    }

    // O resto das suas funções (ApplyScheme, SetControlScheme, ReadInputs, etc.) permanece exatamente o mesmo.
    // Você não precisa mudar mais nada.
    #region FuncoesRestantes
    private void ReadPCInput()
    {
        VerticalAxis = Input.GetAxis("Vertical");
        HorizontalAxis = Input.GetAxis("Horizontal");
        MouseX = Input.GetAxis("Mouse X");
        IsShooting = Input.GetKey(KeyCode.Space);
    }

    private void ReadMobileInput()
    {
        if (movementJoystick != null)
        {
            VerticalAxis = movementJoystick.Vertical;
            HorizontalAxis = movementJoystick.Horizontal;
        }
        else { VerticalAxis = 0; HorizontalAxis = 0; }
        MouseX = 0;
    }

    private void ApplyScheme(string sceneName)
    {
        bool isMobile = currentScheme == ControlScheme.Mobile;

        if (mobileControlsUI != null)
        {
            mobileControlsUI.SetActive(isMobile);
        }

        if (sceneName == "MainMenuScene")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = isMobile;
            Cursor.lockState = isMobile ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void SetControlScheme(int schemeIndex)
    {
        currentScheme = (ControlScheme)schemeIndex;
        PlayerPrefs.SetInt("ControlScheme", schemeIndex);
        ApplyScheme(SceneManager.GetActiveScene().name);
    }

    public void SetShooting(bool isShooting)
    {
        if (currentScheme == ControlScheme.Mobile) IsShooting = isShooting;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
}