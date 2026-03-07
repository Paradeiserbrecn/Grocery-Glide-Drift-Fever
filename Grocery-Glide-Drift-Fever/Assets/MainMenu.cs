using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject dialog;
    
    
    void Start()
    {
        ShowMainMenu();
        HideDialog();
        HideLevelSelect();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        levelSelect.SetActive(true);
    }

    public void HideLevelSelect()
    {
        levelSelect.SetActive(false);
    }

    public void ShowDialog()
    {
        dialog.SetActive(true);
    }

    public void HideDialog()
    {
        dialog.SetActive(false);
    }
}
