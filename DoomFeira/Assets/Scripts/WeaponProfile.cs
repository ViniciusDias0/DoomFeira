using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Profile", menuName = "FPS/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    [Header("Identificação e Visuais")]
    public GameObject pickupPrefab;

    public string weaponName = "New Weapon";
    public Sprite handSprite;
    public Sprite[] shootAnimationFrames;
    public float shootAnimationFPS = 15f;  // <<-- ADICIONE ESTA LINHA
    public Sprite pickupSprite;

    [Header("Atributos de Combate")] // Movi os atributos para baixo para melhor organização
    public float damagePerShot = 50f;
    public float fireRate = 5f;
    public float projectileSpeed = 30f;
    public int projectilesPerShot = 1;
    public float spreadAngle = 0f;

    [Header("Munição")] // Movi os atributos para baixo para melhor organização
    public int maxAmmo = 100;
    public int startingAmmo = 50;

    [Header("Componentes")] // Movi os atributos para baixo para melhor organização
    public GameObject projectilePrefab;
}