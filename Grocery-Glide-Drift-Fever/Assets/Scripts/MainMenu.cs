using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject dialog;
    [SerializeField] private Animator cameraAnimator;
    void Start()
    {
        levelSelect.SetActive(false);
        backButton.SetActive(false);
        dialog.SetActive(false);
    }

    public void SwitchToMainMenu()
    {
        mainMenu.SetActive(true);
        levelSelect.SetActive(false);
        backButton.SetActive(false);
        dialog.SetActive(false);
        cameraAnimator.SetTrigger("switch");
    }

    public void SwitchToLevelSelect()
    {
        mainMenu.SetActive(false);
        levelSelect.SetActive(true);
        cameraAnimator.SetTrigger("switch");
        backButton.SetActive(true);
        dialog.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
