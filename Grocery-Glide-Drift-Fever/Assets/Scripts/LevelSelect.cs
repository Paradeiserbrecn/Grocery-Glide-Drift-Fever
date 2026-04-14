using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private List<LevelData> levelList;
    [SerializeField] private GameObject buttonPrefab;
    
    private List<Button> buttons = new List<Button>();
    

    private void Start()
    {
        ShowAllLevels();
    }

    private void ShowAllLevels()
    {
        foreach (LevelData level in levelList)
        {
            Button newButton = Instantiate(buttonPrefab, content).GetComponent<Button>();
            newButton.onClick.AddListener(() =>
            {
                LoadLevel(level);
                Globals.currentLevel = level;
            });
        }
    }

 

    private void LoadLevel(LevelData level)
    {
        StartCoroutine(LoadYourAsyncScene(level));
    }

    IEnumerator LoadYourAsyncScene(LevelData level)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level.scenePath, LoadSceneMode.Single);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}