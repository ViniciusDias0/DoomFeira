using UnityEngine;
using System.Collections.Generic;

public class WeaponManager_Profiled : MonoBehaviour
{
    [Header("Configuração")]
    public WeaponProfile startingWeaponProfile;
    public WeaponStats weaponScriptInstance; // A referência para o script da arma na cena

    private PlayerController playerController;
    // Usamos um HashSet para rastrear os perfis que o jogador JÁ PODE USAR.
    private HashSet<WeaponProfile> unlockedProfiles = new HashSet<WeaponProfile>();

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (weaponScriptInstance == null)
        {
            Debug.LogError("ERRO CRÍTICO: 'Weapon Script Instance' não foi definido no Player!");
            return;
        }

        // Conecta a instância da arma ao PlayerController desde o início
        if (playerController != null)
        {
            playerController.currentWeapon = weaponScriptInstance;
        }

        unlockedProfiles.Clear();

        if (startingWeaponProfile != null)
        {
            // Pega a arma inicial
            PickupWeapon(startingWeaponProfile);
        }
        else
        {
            // Começa desarmado
            weaponScriptInstance.gameObject.SetActive(false);
        }
    }

    public void PickupWeapon(WeaponProfile newProfile)
    {
        if (newProfile == null) return;

        // Torna a arma visível, caso estivesse desativada
        weaponScriptInstance.gameObject.SetActive(true);

        // Pega o perfil da arma que está equipada NO MOMENTO
        WeaponProfile currentlyEquippedProfile = weaponScriptInstance.GetCurrentProfile();

        // CASO 1: A arma pega é a MESMA que está na mão. Apenas recarrega.
        if (currentlyEquippedProfile == newProfile)
        {
            Debug.Log($"Pegou a mesma arma ({newProfile.weaponName}). Recarregando.");
            int ammoToAdd = Mathf.FloorToInt(newProfile.maxAmmo * 0.5f);
            weaponScriptInstance.AddAmmo(ammoToAdd);
        }
        // CASO 2: A arma pega é DIFERENTE da que está na mão. Troca para ela.
        else
        {
            Debug.Log($"Trocando para a nova arma: {newProfile.weaponName}");
            // Carrega o novo perfil, o que automaticamente muda o sprite e os stats
            weaponScriptInstance.LoadProfile(newProfile);
        }

        // Adiciona o perfil à lista de desbloqueados (não faz mal adicionar de novo)
        if (!unlockedProfiles.Contains(newProfile))
        {
            unlockedProfiles.Add(newProfile);
        }
    }
}