using UnityEngine;
using Unity.Netcode;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TMP_Text healthText;

    private PlayerHealth playerHealth;

    void Update()
    {
        //If we don't have a player yet, try to find our own
        if (playerHealth == null)
        {
            foreach (var player in FindObjectsOfType<PlayerHealth>())
            {
                if (player.IsOwner)
                {
                    playerHealth = player;
                    break;
                }
            }
        }

        //If still no player, stop
        if (playerHealth == null) return;

        //Update UI
        healthText.text = "Health: " + playerHealth.currentHealth.Value;
    }
}