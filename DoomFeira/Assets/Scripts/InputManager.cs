// InputManager.cs (Vers�o Final com bool e Reconex�o de Dropdown)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // --- SUA IDEIA IMPLEMENTADA AQUI ---
    // A bool p�blica que define o controle. 'true' = PC, 'false' = Mobile.
    // [Tooltip("Marque para usar controles de PC, desmarque para Mobile.")]
    // public bool isPCMode; // Removido para usar o enum, que � mais seguro. O controle ser� via Dropdown.

    public enum ControlScheme { PC, Mobile }
    public ControlScheme currentScheme;

    // Propriedades de Input
    public float VerticalAxis { get; private set; }
    public float HorizontalAxis { get; private set; }
    public bool IsShooting { get; private set; }
    public float LookX { get; private set; }
    public float LookY { get; private set; }

    private Joystick movementJoystick;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Carrega a escolha salva (0 para PC, 1 para Mobile)
        currentScheme = (ControlScheme)PlayerPrefs.GetInt("ControlScheme", 0);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    // Chamado TODA VEZ que uma cena � carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se a cena for o menu, procuramos e nos reconectamos ao Dropdown.
        // ESTA � A CORRE��O QUE RESOLVE O PROBLEMA DA SELE��O TRAVADA.
        if (scene.name == "MainMenuScene")
        {
            SetupMainMenuDropdown();
        }

        // Aplica as configura��es corretas para a cena atual.
        ApplyCurrentScheme();
    }

    void Update()
    {
        ReadInput();
    }

    // Fun��o p�blica chamada pelo Dropdown
    public void SetControlScheme(int schemeIndex)
    {
        currentScheme = (ControlScheme)schemeIndex;
        PlayerPrefs.SetInt("ControlScheme", schemeIndex);
        ApplyCurrentScheme();
    }

    // Fun��o central que aplica todas as regras
    private void ApplyCurrentScheme()
    {
        // A bool � definida aqui, baseada na escolha do Dropdown
        bool isPC = (currentScheme == ControlScheme.PC);

        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            GameObject mobileControlsContainer = GameObject.Find("MobileControlsContainer");
            if (mobileControlsContainer != null)
            {
                // A UI mobile S� aparece se N�O for PC.
                mobileControlsContainer.SetActive(!isPC);
                if (!isPC)
                {
                    movementJoystick = mobileControlsContainer.GetComponentInChildren<Joystick>();
                    SetupMobileAttackButton(mobileControlsContainer);
                }
            }
        }

        ApplyCursorState();
    }

    private void ApplyCursorState()
    {
        bool inMenu = (SceneManager.GetActiveScene().name == "MainMenuScene" || SceneManager.GetActiveScene().name == "GameOverScene");
        if (inMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            bool isPC = (currentScheme == ControlScheme.PC);
            Cursor.lockState = isPC ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isPC;
        }
    }

    // Procura e se conecta ao dropdown do menu
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

    // O resto do c�digo permanece o mesmo
    #region Fun��es Auxiliares
    private void SetupMobileAttackButton(GameObject container)
    {
        Transform attackButtonT = container.transform.Find("Atacar");
        if (attackButtonT == null) return;
        EventTrigger trigger = attackButtonT.GetComponent<EventTrigger>() ?? attackButtonT.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();
        EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener((d) => SetShooting(true));
        trigger.triggers.Add(down);
        EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener((d) => SetShooting(false));
        trigger.triggers.Add(up);
    }

    public void SetShooting(bool shooting) { if (currentScheme == ControlScheme.Mobile) IsShooting = shooting; }
    public void SetLookInput(Vector2 delta) { if (currentScheme == ControlScheme.Mobile) { LookX = delta.x; LookY = delta.y; } }

    private void ReadInput()
    {
        bool isPC = (currentScheme == ControlScheme.PC);
        if (isPC)
        {
            VerticalAxis = Input.GetAxis("Vertical");
            HorizontalAxis = Input.GetAxis("Horizontal");
            IsShooting = Input.GetMouseButton(0);
            LookX = Input.GetAxis("Mouse X");
            LookY = Input.GetAxis("Mouse Y");
        }
        else // Mobile
        {
            if (movementJoystick != null)
            {
                VerticalAxis = movementJoystick.Vertical;
                HorizontalAxis = movementJoystick.Horizontal;
            }
            else { VerticalAxis = 0; HorizontalAxis = 0; }
        }
    }
    #endregion
}