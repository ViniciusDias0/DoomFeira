using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    // Agora ele segura a refer�ncia para o PERFIL da arma
    public WeaponProfile weaponProfile;

    // ... c�digo de rota��o ...

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponManager_Profiled weaponManager = other.GetComponent<WeaponManager_Profiled>();
            if (weaponManager != null)
            {
                weaponManager.PickupWeapon(weaponProfile);
                Destroy(gameObject);
            }
        }
    }
}