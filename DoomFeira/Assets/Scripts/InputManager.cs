// InputManager.cs (Versão Final com Conexão Automática de UI)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro; // <<< --- ADIÇÃO ESSENCIAL para encontrar o TMP_Dropdown

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum ControlScheme { PC, Mobile }
    public ControlScheme currentScheme;

    public float VerticalAxis { get; private set; }
    public float HorizontalAxis { get; private set; }
    public bool IsShooting { get; private set; }

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamado toda vez que uma cena é carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se a cena for a do menu, configura o dropdown.
        if (scene.name == "MainMenuScene") // Use o nome exato da sua cena de menu
        {
            SetupMainMenuDropdown();
        }

        // Aplica o esquema correto para a cena atual.
        ApplyCurrentScheme();
    }

    void Update()
    {
        if (currentScheme == ControlScheme.PC) ReadPCInput();
        else ReadMobileInput();
    }

    // Função chamada pelo Dropdown do menu
    public void SetControlScheme(int schemeIndex)
    {
        currentScheme = (ControlScheme)schemeIndex;
        PlayerPrefs.SetInt("ControlScheme", schemeIndex);
        ApplyCurrentScheme();
    }

    // Função central que configura TUDO
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

    // --- NOVA FUNÇÃO QUE RESOLVE O PROBLEMA DO MENU ---
    private void SetupMainMenuDropdown()
    {
        // Encontra o dropdown na cena. Certifique-se que o nome do seu Dropdown é "Dropdown".
        // Se tiver outro nome, mude o nome aqui no código.
        TMP_Dropdown dropdown = FindObjectOfType<TMP_Dropdown>();

        if (dropdown != null)
        {
            // Limpa qualquer conexão antiga (feita pelo Inspector).
            dropdown.onValueChanged.RemoveAllListeners();
            // Conecta o dropdown à nossa função SetControlScheme.
            dropdown.onValueChanged.AddListener(SetControlScheme);
            // Atualiza o valor do dropdown para mostrar a seleção atual.
            dropdown.SetValueWithoutNotify((int)currentScheme);

            Debug.Log("Dropdown do menu configurado automaticamente!");
        }
        else
        {
            Debug.LogWarning("Dropdown não encontrado na cena do menu.");
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

    private void ReadPCInput()
    {
        VerticalAxis = Input.GetAxis("Vertical");
        HorizontalAxis = Input.GetAxis("Horizontal");
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
    }
}