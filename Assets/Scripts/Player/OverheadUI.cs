using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class OverheadUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI txtUsername;
    [SerializeField] private Slider healthbar;

    [SyncVar] [HideInInspector] private ClientPlayer clientPlayer;

    public ClientPlayer ClientPlayer { get { return clientPlayer; } set { clientPlayer = value; } }

    private void Update()
    {
        UpdateHealthbar(clientPlayer.Health, clientPlayer.MaxHealth);
    }

    private void UpdateHealthbar(int health, int maxHealth)
    {
        healthbar.value = (float)health / (float)maxHealth;
    }

    public void UpdateUsername(string username)
    {
        txtUsername.text = username;
    }

    [ObserversRpc]
    public void InitializeOnClients(string username)
    {
        txtUsername.text = username;
    }
}
