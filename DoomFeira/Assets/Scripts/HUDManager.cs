using UnityEngine;
using UnityEngine.UI; // Necess�rio para Image
using TMPro;          // Necess�rio para TextMeshProUGUI

public class HUDManager : MonoBehaviour
{
    [Header("Stats")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI ammoText;  // <<-- A vari�vel est� aqui

    [Header("Face")]
    public Image faceImage;
    public Sprite[] faceSprites;

    // Fun��o para atualizar vida e armadura
    public void UpdateStatus(int health, int armor)
    {
        if (healthText != null) healthText.text = $"{health}%";
        if (armorText != null) armorText.text = $"{armor}%";

        if (faceImage == null || faceSprites.Length < 5) return;

        // L�gica do rosto
        if (health > 80) faceImage.sprite = faceSprites[0];
        else if (health > 60) faceImage.sprite = faceSprites[1];
        else if (health > 40) faceImage.sprite = faceSprites[2];
        else if (health > 20) faceImage.sprite = faceSprites[3];
        else faceImage.sprite = faceSprites[4];
    }

    // Fun��o para atualizar a muni��o
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}