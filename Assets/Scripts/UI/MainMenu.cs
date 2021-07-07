using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private GameObject bestObj;
    [SerializeField]
    private GameObject startText;
    [SerializeField]
    private TextMeshProUGUI bestText;
    [SerializeField]
    private GameObject moneyObj;
    [SerializeField]
    private TextMeshProUGUI moneyText;

    // Start is called before the first frame update
    private void Start()
    {
        GM.OnCoinPickup += MoneyOnScreen;

        BestOnScreen();
        MoneyOnScreen();
        StartTextAnim();
    }

    //call from ui
    

    void BestOnScreen()
    {
        if (GV.best == 0)
        {
            bestObj.SetActive(false);
        }
        else
        {
            bestObj.SetActive(true);
            bestText.text = GV.best.ToString();
        }
    }

    void MoneyOnScreen()
    {
        if (GV.coins == 0)
        {
            moneyObj.SetActive(false);
        }
        else
        {
            moneyObj.SetActive(true);
            moneyText.text = GV.coins.ToString();
        }
    }

    void StartTextAnim()
    {
        Sequence seq = DOTween.Sequence();
        Vector3 scale = startText.transform.localScale;

        this.Delay(StartTextAnim, 3.5f);

        seq.Append(startText.transform.DOScale(scale * 1.1f, 1.5f))
            .PrependInterval(.5f)
            .Append(startText.transform.DOScale(scale, 1.5f));
    }
}
