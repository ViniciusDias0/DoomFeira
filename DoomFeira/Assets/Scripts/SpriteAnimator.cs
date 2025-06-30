using UnityEngine;
using System.Collections.Generic;

// Esta pequena classe � um "container" para os dados de cada anima��o.
// [System.Serializable] faz com que ela apare�a no Inspector da Unity.
[System.Serializable]
public class AnimationState
{
    public string stateName; // Ex: "Walk", "Idle", "Death"
    public Sprite[] frames;
    public float framesPerSecond = 10f;
}

public class SpriteAnimator : MonoBehaviour
{
    public List<AnimationState> animationStates; // Uma lista para voc� colocar todas as anima��es no Inspector

    private SpriteRenderer spriteRenderer;
    private Dictionary<string, AnimationState> stateDictionary; // Para acesso r�pido �s anima��es
    private AnimationState currentState;
    private int currentFrameIndex;
    private float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Preenche o dicion�rio para n�o ter que procurar na lista toda vez
        stateDictionary = new Dictionary<string, AnimationState>();
        foreach (AnimationState state in animationStates)
        {
            stateDictionary[state.stateName] = state;
        }
    }

    void Update()
    {
        // Se n�o houver uma anima��o tocando, n�o faz nada
        if (currentState == null) return;

        // Avan�a o timer
        timer += Time.deltaTime;

        // Se for hora de trocar de frame...
        if (timer >= 1f / currentState.framesPerSecond)
        {
            timer = 0; // Reseta o timer
            // Avan�a para o pr�ximo frame, voltando ao in�cio se chegar ao fim
            currentFrameIndex = (currentFrameIndex + 1) % currentState.frames.Length;
            // Aplica o novo sprite ao Sprite Renderer
            spriteRenderer.sprite = currentState.frames[currentFrameIndex];
        }
    }

    // Esta � a fun��o que outros scripts (como o Enemy.cs) v�o chamar para tocar uma anima��o
    public void Play(string stateName)
    {
        // Se j� estivermos tocando esta anima��o, n�o faz nada
        if (currentState != null && currentState.stateName == stateName) return;

        // Procura a anima��o no dicion�rio
        if (stateDictionary.TryGetValue(stateName, out AnimationState newState))
        {
            currentState = newState;
            currentFrameIndex = 0; // Reseta para o primeiro frame
            timer = 0;
            // Aplica o primeiro frame imediatamente para uma resposta visual r�pida
            spriteRenderer.sprite = currentState.frames[0];
        }
        else
        {
            Debug.LogWarning("Anima��o n�o encontrada: " + stateName);
        }
    }
}