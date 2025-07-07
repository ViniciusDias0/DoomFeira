using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Necessário para usar Listas

// A classe de estado de animação que definimos acima
[System.Serializable]
public class FaceAnimationState
{
    public string stateName;
    public Sprite[] frames;
    public float framesPerSecond = 8f;
    public bool loop = true;
}

public class HUDManager : MonoBehaviour
{
    [Header("Stats")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI ammoText;

    [Header("Face")]
    public UISpriteAnimator faceAnimator; // <<-- MUDANÇA: Referência para o nosso novo animador
    public List<FaceAnimationState> faceAnimations; // <<-- MUDANÇA: Lista para todas as animações do rosto

    private string currentFaceState = ""; // Guarda o estado atual para evitar reiniciar a mesma animação

    // Função para atualizar vida e armadura
    public void UpdateStatus(int health, int armor)
    {
        if (healthText != null) healthText.text = $"{health}%";
        if (armorText != null) armorText.text = $"{armor}%";

        // --- LÓGICA DO ROSTO MODIFICADA ---
        string newStateName = GetStateNameForHealth(health);

        // Se o estado de saúde mudou (ex: de Saudavel para Machucado), toca a nova animação
        if (newStateName != currentFaceState)
        {
            currentFaceState = newStateName;

            // Encontra a animação correspondente na nossa lista pelo nome
            FaceAnimationState stateToPlay = faceAnimations.Find(anim => anim.stateName == newStateName);

            if (stateToPlay != null && faceAnimator != null)
            {
                // Diz ao animador para tocar os frames desta animação com as configurações dela
                faceAnimator.PlayAnimation(stateToPlay.frames, stateToPlay.framesPerSecond, stateToPlay.loop);
            }
        }
    }

    // Função auxiliar que decide o nome do estado baseado na porcentagem de vida
    private string GetStateNameForHealth(int health)
    {
        if (health > 80) return "Saudavel";
        if (health > 60) return "LevementeMachucado";
        if (health > 40) return "Machucado";
        if (health > 20) return "MuitoMachucado";
        return "Morrendo";
    }

    // ... suas outras funções (UpdateAmmo, etc.) continuam aqui ...
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}