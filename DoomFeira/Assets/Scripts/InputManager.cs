// InputManager.cs (Com a Correção Final do Toque)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum ControlScheme { PC, Mobile }
    public ControlScheme currentScheme;

    public float VerticalAxis { get; private set; }
    public float HorizontalAxis { get; private set; }
    public bool IsShooting { get; private set; }

    public float LookX { get; private set; }
    public float LookY { get; private set; }

    private Joystick movementJoystick;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentScheme = (ControlScheme)PlayerPrefs.GetInt("ControlScheme", 0);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene") SetupMainMenuDropdown();
        ApplyCurrentScheme();
    }

    void Update()
    {
        // A cada frame, simplesmente chamamos o método de leitura correto.
        // A lógica de reset foi movida para dentro de ReadPCInput.
        if (currentScheme == ControlScheme.PC)
        {
            ReadPCInput();
        }
        else // Mobile
        {
            ReadMobileInput();
        }
    }

    // --- FUNÇÃO CORRIGIDA ---
    // Agora o reset do LookX/Y acontece APENAS no PC.
    private void ReadPCInput()
    {
        // 1. Reseta os valores do frame anterior.
        LookX = 0;
        LookY = 0;

        // 2. Lê os novos valores.
        VerticalAxis = Input.GetAxis("Vertical");
        HorizontalAxis = Input.GetAxis("Horizontal");
        IsShooting = Input.GetMouseButton(0);
        LookX = Input.GetAxis("Mouse X");
        LookY = Input.GetAxis("Mouse Y");
    }

    private void ReadMobileInput()
    {
        if (movementJoystick != null)
        {
            VerticalAxis = movementJoystick.Vertical;
            HorizontalAxis = movementJoystick.Horizontal;
        }
        else { VerticalAxis = 0; HorizontalAxis = 0; }
        // No mobile, NÃO resetamos LookX/LookY.
        // Eles são definidos pelo TouchLookArea e zerados por ele no OnPointerUp.
    }

    // Esta função é chamada pelo TouchLookArea
    public void SetLookInput(Vector2 delta)
    {
        LookX = delta.x;
        LookY = delta.y;
    }

    // O resto do seu código InputManager permanece o mesmo.
    #region Código Intacto
    public void SetControlScheme(int schemeIndex)
    {
        currentScheme = (ControlScheme)schemeIndex;
        PlayerPrefs.SetInt("ControlScheme", schemeIndex);
        ApplyCurrentScheme();
    }
    private void ApplyCurrentScheme()
    {
        bool isMobile = (currentScheme == ControlScheme.Mobile);
        GameObject mobileControlsContainer = GameObject.Find("MobileControlsContainer");
        if (mobileControlsContainer != null)
        {
            mobileControlsContainer.SetActive(isMobile);
            if (isMobile)
            {
                movementJoystick = mobileControlsContainer.GetComponentInChildren<Joystick>();
                SetupMobileAttackButton();
            }
        }
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = isMobile ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMobile;
        }
    }
    private void SetupMainMenuDropdown()
    {
        TMP_Dropdown dropdown = FindObjectOfType<TMP_Dropdown>();
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(SetControlScheme);
            dropdown.SetValueWithoutNotify((int)currentScheme);
        }
    }
    private void SetupMobileAttackButton()
    {
        GameObject attackButtonObject = GameObject.Find("Atacar");
        if (attackButtonObject == null) return;
        EventTrigger trigger = attackButtonObject.GetComponent<EventTrigger>() ?? attackButtonObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDownEntry.callback.AddListener((data) => { SetShooting(true); });
        trigger.triggers.Add(pointerDownEntry);
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUpEntry.callback.AddListener((data) => { SetShooting(false); });
        trigger.triggers.Add(pointerUpEntry);
    }
    public void SetShooting(bool isShooting)
    {
        if (currentScheme == ControlScheme.Mobile)
        {
            IsShooting = isShooting;
        }
    }
    #endregion
}