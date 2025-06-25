using UnityEngine;
using UnityEngine.UI; // Para o componente Image
using TMPro; // Para os componentes TextMeshPro

public class HUDManager : MonoBehaviour
{
    // Referências para os elementos da UI que vamos arrastar no Inspector
    [Header("Stats")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;

    [Header("Face")]
    public Image faceImage;
    public Sprite[] faceSprites; // Array para guardar as 5 imagens do rosto

    // Função pública que o jogador vai chamar para atualizar tudo
    public void UpdateHUD(int health, int armor)
    {
        // Atualiza os textos
        healthText.text = $"{health}%";
        armorText.text = $"{armor}%";

        // Lógica para mudar o rosto
        if (health > 80)
        {
            faceImage.sprite = faceSprites[0]; // 100-81% : Saudável
        }
        else if (health > 60)
        {
            faceImage.sprite = faceSprites[1]; // 80-61% : Aranhado
        }
        else if (health > 40)
        {
            faceImage.sprite = faceSprites[2]; // 60-41% : Sangrando
        }
        else if (health > 20)
        {
            faceImage.sprite = faceSprites[3]; // 40-21% : Bem machucado
        }
        else
        {
            faceImage.sprite = faceSprites[4]; // 20-0% : Quase morrendo
        }
    }
}