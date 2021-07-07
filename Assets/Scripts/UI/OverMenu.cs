using System.Collections;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using DG.Tweening;

public class OverMenu : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private GameObject firstScreen;
    [SerializeField]
    private GameObject secondScreen;

    private bool withSecondScreen;

    void OnEnable()
    {
        OnGameOver();

        firstScreen.SetActive(true);
        secondScreen.SetActive(false);
    }

    void OnGameOver()
    {
        if (GV.coinsScore > 0)
        {
            withSecondScreen = true;
        }
        else
        {
            withSecondScreen = false;
        }
    }

    //call from ui
    public void OnSecondScreen()
    {
        if (withSecondScreen)
        {
            firstScreen.SetActive(false);
            secondScreen.SetActive(true);
        }
        else
        {
            GM.LevelReload();
        }
    }
}
