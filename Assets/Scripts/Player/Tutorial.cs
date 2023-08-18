using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private List<GameObject> pages = new();
    int currentPage = 0;

    public void NextPage()
    {
        if(currentPage == pages.Count - 1)
        {
            gameObject.SetActive(false);

            if(mainMenu != null)
            {
                mainMenu.SetActive(true);
            }

            return;
        }

        ShowPage(currentPage + 1);
    }

    private void OnEnable()
    {
        ShowPage(0);
    }

    private void ShowPage(int page)
    {
        for(int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(false);
        }

        currentPage = page;
        pages[page].SetActive(true);

        // if last page
        if(currentPage == pages.Count - 1)
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Start");
        }
    }
}
