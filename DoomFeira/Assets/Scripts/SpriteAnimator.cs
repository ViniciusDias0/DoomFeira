using UnityEngine;
using System.Collections.Generic;

// Esta pequena classe é um "container" para os dados de cada animação.
// [System.Serializable] faz com que ela apareça no Inspector da Unity.
[System.Serializable]
public class AnimationState
{
    public string stateName; // Ex: "Walk", "Idle", "Death"
    public Sprite[] frames;
    public float framesPerSecond = 10f;
    public bool loop = true; // <<-- ADICIONE ESTA LINHA (Por padrão, as animações repetem)
}

public class SpriteAnimator : MonoBehaviour
{
    public List<AnimationState> animationStates; // Uma lista para você colocar todas as animações no Inspector

    private SpriteRenderer spriteRenderer;
    private Dictionary<string, AnimationState> stateDictionary; // Para acesso rápido às animações
    private AnimationState currentState;
    private int currentFrameIndex;
    private float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Preenche o dicionário para não ter que procurar na lista toda vez
        stateDictionary = new Dictionary<string, AnimationState>();
        foreach (AnimationState state in animationStates)
        {
            stateDictionary[state.stateName] = state;
        }
    }

    void Update()
    {
        if (currentState == null) return;

        // Se a animação não deve repetir E já estamos no último frame, pare aqui.
        if (!currentState.loop && currentFrameIndex == currentState.frames.Length - 1)
        {
            return; // A animação "congela" no último frame.
        }

        if (currentState.frames.Length <= 1 || currentState.framesPerSecond <= 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / currentState.framesPerSecond)
        {
            timer = 0;
            currentFrameIndex = (currentFrameIndex + 1) % currentState.frames.Length;
            spriteRenderer.sprite = currentState.frames[currentFrameIndex];
        }
    }

    // Esta é a função que outros scripts (como o Enemy.cs) vão chamar para tocar uma animação
    public void Play(string stateName)
    {
        // Se já estivermos tocando esta animação, não faz nada
        if (currentState != null && currentState.stateName == stateName) return;

        // Procura a animação no dicionário
        if (stateDictionary.TryGetValue(stateName, out AnimationState newState))
        {
            currentState = newState;
            currentFrameIndex = 0; // Reseta para o primeiro frame
            timer = 0;
            // Aplica o primeiro frame imediatamente para uma resposta visual rápida
            spriteRenderer.sprite = currentState.frames[0];
        }
        else
        {
            Debug.LogWarning("Animação não encontrada: " + stateName);
        }
    }
}