using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FishNet.Object;

public class UserInterface : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private ClientPlayer clientPlayer;
    [SerializeField] private List<TextMeshProUGUI> lbUsernames = new();
    [SerializeField] private List<TextMeshProUGUI> lbTimes = new();
    [SerializeField] private TextMeshProUGUI txtTimeAlive;
    [SerializeField] private TextMeshProUGUI txtPersonalBest;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI txtItemName;
    [SerializeField] private TextMeshProUGUI txtItemDescription;
    [SerializeField] private ItemScriptableObject emptyItem;

    private void Update()
    {
        if(IsOwner)
        {
            UpdateLeaderboard();
            UpdateTimes();
        }
    }

    private void UpdateLeaderboard()
    {
        for(int i = 0; i < lbUsernames.Count; i++)
        {
            try
            {
                lbUsernames[i].text = GameManager.Instance.playersSortedByTime[i].Username;
                lbTimes[i].text = ConvertTime(GameManager.Instance.playersSortedByTime[i].LongestTime);
            }
            catch { }
        }
    }

    private void UpdateTimes()
    {
        txtTimeAlive.SetText("Time Alive: " + ConvertTime(clientPlayer.CurrentTime));
        txtPersonalBest.SetText("Personal Best: " + ConvertTime(clientPlayer.LongestTime));
    }

    // converts seconds to mins and seconds
    private string ConvertTime(float seconds)
    {
        string msg = "";

        seconds = Mathf.Round(seconds);

        float mins = seconds / 60f;

        seconds = mins - (float)Math.Truncate(mins);

        if(mins < 1)
        {
            mins = 0;
        }

        mins = Mathf.Floor(mins);

        seconds = Mathf.Round(seconds * 60f);

        msg = mins + "mins " + seconds + "s";

        return msg;
    }

    public void UpdateHealthBar(int health, int maxHealth)
    {
        healthBar.value = (float)health / (float)maxHealth;
    }

    public void UpdateItemSlot(ItemScriptableObject itemSO)
    {
        // when player uses an item
        if (itemSO == null)
        {
            itemSO = emptyItem;
        }

        // reset item icon if possible
        if (itemIcon.sprite != null)
        {
            itemIcon.sprite = null;
        }

        txtItemName.SetText(itemSO.itemName);
        txtItemDescription.SetText(itemSO.description);

        if(itemSO.itemIcon != null)
        {
            itemIcon.sprite = itemSO.itemIcon;
        }
    }
}
