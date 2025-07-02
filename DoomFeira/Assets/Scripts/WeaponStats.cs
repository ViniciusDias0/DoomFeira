using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    private WeaponProfile currentProfile;
    private int currentAmmo;
    private float nextFireTime = 0f;

    private SpriteRenderer weaponSpriteRenderer;
    private Transform firePoint;
    private HUDManager hudManager;

    void Awake()
    {
        weaponSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        firePoint = transform.Find("FirePoint");
        hudManager = FindObjectOfType<HUDManager>();
    }

    // Função para carregar um novo perfil de arma (chamada pelo Manager)
    public void LoadProfile(WeaponProfile profile)
    {
        currentProfile = profile;
        currentAmmo = profile.startingAmmo;

        // --- MUDANÇA AQUI ---
        // LINHA ANTIGA: weaponSpriteRenderer.sprite = profile.weaponSprite;
        // LINHA NOVA:
        weaponSpriteRenderer.sprite = profile.handSprite;
        // --- FIM DA MUDANÇA ---

        UpdateAmmoUI(); 

    }

    // Função para adicionar munição (chamada pelo Manager)
    public void AddAmmo(int amount)
    {
        if (currentProfile == null) return;
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, currentProfile.maxAmmo);
        UpdateAmmoUI();
    }

    public void TryToShoot()
    {
        if (currentProfile == null || Time.time < nextFireTime || currentAmmo <= 0) return;
        nextFireTime = Time.time + 1f / currentProfile.fireRate;
        Shoot();
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        Quaternion baseRotation = Camera.main.transform.rotation;
        for (int i = 0; i < currentProfile.projectilesPerShot; i++)
        {
            Quaternion spread = Quaternion.Euler(Random.Range(-currentProfile.spreadAngle, currentProfile.spreadAngle), Random.Range(-currentProfile.spreadAngle, currentProfile.spreadAngle), 0);
            InstantiateProjectile(baseRotation * spread);
        }
    }

    private void InstantiateProjectile(Quaternion rotation)
    {
        // ... (código do InstantiateProjectile permanece o mesmo) ...
        GameObject projectileObj = Instantiate(currentProfile.projectilePrefab, firePoint.position, rotation);
        PlayerProjectile_Modular projScript = projectileObj.GetComponent<PlayerProjectile_Modular>();
        if (projScript != null)
        {
            projScript.damage = currentProfile.damagePerShot;
            projScript.speed = currentProfile.projectileSpeed;
        }
    }

    public void UpdateAmmoUI()
    {
        if (hudManager != null && currentProfile != null)
        {
            hudManager.UpdateAmmo(currentAmmo, currentProfile.maxAmmo);
        }
    }

    // Função para saber qual perfil está carregado
    public WeaponProfile GetCurrentProfile()
    {
        return currentProfile;
    }
}