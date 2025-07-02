using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Profile", menuName = "FPS/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    [Header("Identificação e Visuais")]
    public GameObject pickupPrefab;

    public string weaponName = "New Weapon";
    public Sprite weaponSprite;
    public float damagePerShot = 50f;
    public float fireRate = 5f;
    public float projectileSpeed = 30f;
    public int projectilesPerShot = 1;
    public float spreadAngle = 0f;
    public int maxAmmo = 100;
    public int startingAmmo = 50;
    public GameObject projectilePrefab;
}