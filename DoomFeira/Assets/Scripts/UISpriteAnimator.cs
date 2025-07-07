using UnityEngine;
using UnityEngine.UI; // Essencial para componentes de UI como Image
using System.Collections; // Essencial para Coroutines

// Esta linha garante que o script só pode ser adicionado a um objeto que já tenha um componente Image.
[RequireComponent(typeof(Image))]
public class UISpriteAnimator : MonoBehaviour
{
    private Image imageComponent;
    private Coroutine currentAnimationCoroutine; // Guarda a referência da animação que está tocando

    void Awake()
    {
        // Pega o componente Image no mesmo objeto para podermos trocar os sprites
        imageComponent = GetComponent<Image>();
    }

    // Função pública que o HUDManager vai chamar para iniciar uma nova animação
    public void PlayAnimation(Sprite[] frames, float fps, bool loop)
    {
        // Se já houver uma animação tocando, pare-a para não criar conflitos
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }

        // Inicia a nova animação e guarda sua referência
        currentAnimationCoroutine = StartCoroutine(AnimateRoutine(frames, fps, loop));
    }

    // A rotina (Coroutine) que efetivamente faz a animação acontecer
    private IEnumerator AnimateRoutine(Sprite[] frames, float fps, bool loop)
    {
        // Verifica se há algo para animar
        if (frames == null || frames.Length == 0 || fps <= 0)
        {
            // Se houver pelo menos um frame, mostra ele como uma imagem estática e para
            if (frames != null && frames.Length > 0) imageComponent.sprite = frames[0];
            yield break; // Encerra a rotina
        }

        int index = 0;
        float frameDuration = 1f / fps; // Calcula quanto tempo cada frame deve ficar na tela

        while (true) // Um loop que será controlado pela lógica interna
        {
            // Mostra o frame atual na tela
            imageComponent.sprite = frames[index];

            // Avança o índice para o próximo frame
            index++;

            // Verifica se a animação chegou ao fim
            if (index >= frames.Length)
            {
                if (loop)
                {
                    index = 0; // Se for para repetir, volta para o início
                }
                else
                {
                    yield break; // Se não for para repetir, encerra a animação aqui
                }
            }

            // Espera a duração do frame antes de continuar o loop
            yield return new WaitForSeconds(frameDuration);
        }
    }
}