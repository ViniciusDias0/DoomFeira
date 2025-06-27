using UnityEngine;
using UnityEngine.UI; // Necessário para Image
using TMPro;          // Necessário para TextMeshProUGUI

public class HUDManager : MonoBehaviour
{
    [Header("Stats")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI ammoText;  // <<-- A variável está aqui

    [Header("Face")]
    public Image faceImage;
    public Sprite[] faceSprites;

    // Função para atualizar vida e armadura
    public void UpdateStatus(int health, int armor)
    {
        if (healthText != null) healthText.text = $"{health}%";
        if (armorText != null) armorText.text = $"{armor}%";

        if (faceImage == null || faceSprites.Length < 5) return;

        // Lógica do rosto
        if (health > 80) faceImage.sprite = faceSprites[0];
        else if (health > 60) faceImage.sprite = faceSprites[1];
        else if (health > 40) faceImage.sprite = faceSprites[2];
        else if (health > 20) faceImage.sprite = faceSprites[3];
        else faceImage.sprite = faceSprites[4];
    }

    // Função para atualizar a munição
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}