using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Encontra a câmera principal uma vez para otimizar
        cameraTransform = Camera.main.transform;
    }

    // Usamos LateUpdate para garantir que o billboard só gire depois que a câmera se moveu
    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Faz o objeto olhar na direção da câmera
        transform.LookAt(cameraTransform);

        // Corrige a rotação para que ele não incline para cima ou para baixo, apenas no eixo Y
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}