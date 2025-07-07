using UnityEngine;
using System.Collections;

public class WeaponAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private Sprite[] shootFrames;
    private float animationSpeed;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Função para ser chamada pelo WeaponStats para configurar a animação
    public void SetAnimationData(Sprite idle, Sprite[] shoot, float fps)
    {
        idleSprite = idle;
        shootFrames = shoot;
        // A velocidade da animação é baseada na cadência de tiro
        animationSpeed = fps;
    }

    // Inicia a animação de tiro
    public void PlayShootAnimation()
    {
        // Para a animação anterior se estiver tocando e inicia uma nova
        StopAllCoroutines();
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        // Se não houver frames de animação, não faz nada
        if (shootFrames == null || shootFrames.Length == 0) yield break;

        // Calcula quanto tempo cada frame deve ficar na tela
        float frameDuration = 1f / animationSpeed;

        // Passa por cada frame da animação
        foreach (Sprite frame in shootFrames)
        {
            spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(frameDuration);
        }

        // Ao final, volta para o sprite de "parado"
        spriteRenderer.sprite = idleSprite;
    }
}