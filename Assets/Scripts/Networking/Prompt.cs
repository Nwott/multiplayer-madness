using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Prompt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject prompt;
    [SerializeField] private TextMeshProUGUI txtDescription;

    public static Prompt Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPrompt(string message)
    {
        prompt.SetActive(true);
        txtDescription.text = message;
    }

    public void OnOkayBtnClicked()
    {
        prompt.SetActive(false);
    }
}
