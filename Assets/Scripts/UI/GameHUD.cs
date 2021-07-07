using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    [Header("Frameworks")]
    [SerializeField]
    private GameManager GM;
    [Space]

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private GameObject coinObj;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private GameObject bonusObj;
    [SerializeField]
    private Image progressBonus;

    [Space]
    [SerializeField]
    private GameObject textReaction;
    [SerializeField]
    private GameObject scoreReaction;

    private bool giveBonusOnce;
    private bool progressBarUpdate = true;
    private CanvasGroup coinCanvGr;
    private Coroutine barUpd;
    private Coroutine barBonus;
    private Coroutine coin;

    private void OnEnable()
    {
        GM.OnCoinPickup += MoneyCount;
        GM.OnLevelUp += OnLevelUp;
        GM.OnLevelUpEnd += OnLevelUpEnd;
        GM.OnBonusProgress += BonusProgressBar;
        GM.OnScore += ScoreCount;
        GM.OnMiss += OnMiss;
        GM.OnCoinPickup += OnCoinPickup;

        pauseScreen.SetActive(false);
        coinCanvGr = coinObj.GetComponent<CanvasGroup>();
        coinCanvGr.alpha = 0f;
    }

    private void OnDisable()
    {
        GM.OnCoinPickup -= MoneyCount;
        GM.OnLevelUp -= OnLevelUp;
        GM.OnLevelUpEnd -= OnLevelUpEnd;
        GM.OnBonusProgress -= BonusProgressBar;
        GM.OnScore -= ScoreCount;
        GM.OnMiss -= OnMiss;
        GM.OnCoinPickup -= OnCoinPickup;
    }

    public void BonusProgressBar(float stepToBonus)
    {
        if (progressBarUpdate)
        {
            Vector3 scale = bonusObj.transform.localScale;
            Sequence seq = DOTween.Sequence();
            seq.Append(bonusObj.transform.DOScale(scale * 1.2f, .1f))
                .Append(bonusObj.transform.DOScale(scale, .1f));

            progressBonus.color = new Color32(0, 150, 0, 255);
            giveBonusOnce = true;

            barUpd = null;
            barUpd = StartCoroutine(ProgressAnim(.25f, GV.bonusFillAmount, GV.bonusFillAmount + stepToBonus));
        }
    }

    void OnCoinPickup()
    {
        if (coin != null)
        {
            StopCoroutine(coin);
            coin = null;
        }
        
        coin = StartCoroutine(CoinOnScreen(.5f));
    }

    void OnBonus()
    {
        GM.Bonus();

        progressBarUpdate = false;
        progressBonus.color = new Color32(255, 66, 75, 255);

        StartCoroutine(BonusDelay(GV.bonusDuration));

        barBonus = null;
        barBonus = StartCoroutine(ProgressAnim(GV.bonusDuration, 1f, 0f));
    }

    void OnLevelUp()
    {
        progressBarUpdate = false;

        if (GV.bonusFillAmount > 0f)
        {
            progressBonus.color = new Color32(0, 150, 0, 255);

            StopCoroutine(barUpd);
            StopCoroutine(barBonus);
            StartCoroutine(ProgressAnim(.3f, GV.bonusFillAmount, 0f));
        }
    }

    void OnLevelUpEnd()
    {
        progressBarUpdate = true;
    }

    void OnMiss()
    {
        string message;
        string[] reacts = { "miss", "oops", "hmm" };
        int i = Random.Range(0, 3);
        message = reacts[i];
        ScoreReaction(message);

        //if (GV.missCounter == 1)
        //{
        //    string message;
        //    string[] reacts = { "miss", "oops", "hmm" };
        //    int i = Random.Range(0, 3);
        //    message = reacts[i];
        //    ScoreReaction(message);
        //}
    }

    void ScoreCount(int scoreReaction)
    {
        scoreText.text = GV.score.ToString();

        string messageScore;
        string messageText;

        if (scoreReaction == 1)
        {
            messageScore = "+" + scoreReaction.ToString();
        }
        else
        {
            messageScore = "x" + scoreReaction.ToString();
        }

        if (scoreReaction == 3)
        {
            string[] reacts = { "Wow", "Cool!", "Bull'seye!"};
            int i = Random.Range(0, 3);
            messageText = reacts[i];
            TextReaction(messageText);
        }

        if (scoreReaction == 5)
        {
            messageText = "Great!";
            TextReaction(messageText);
        }

        if (scoreReaction == 7)
        {
            messageText = "Awesome!";
            TextReaction(messageText);
        }

        ScoreReaction(messageScore);
    }

    void ScoreReaction(string message)
    {
        Vector3 offset = new Vector3(Random.Range(-100f, 100f), Random.Range(50f, -50f), 0f);
        
        GameObject rcInst = Instantiate(scoreReaction, transform);

        rcInst.transform.position += offset;

        rcInst.GetComponent<WarningPanel>().Reaction(message);
    }

    void TextReaction(string message)
    {
        GameObject rcInst = Instantiate(textReaction, transform);

        rcInst.GetComponent<WarningPanel>().Reaction(message);
    }

    void MoneyCount()
    {
        moneyText.text = GV.coinsScore.ToString();
    }

    private void OnApplicationFocus(bool pause)
    {
        if (!pauseScreen.activeSelf)
        {
            Pause(!pause);
        }
    }

    //call from ui
    public void Pause(bool b)
    {
        GM.Pause(b);
        pauseScreen.SetActive(b);
    }

    void FixedUpdate()
    {
        if (GV.bonusFillAmount > 0f && giveBonusOnce)
        {
            GV.bonusFillAmount -= .002f;

        }

        if (GV.bonusFillAmount >= .99f && giveBonusOnce)
        {
            OnBonus();
            giveBonusOnce = false;

        }

        progressBonus.fillAmount = GV.bonusFillAmount;
    }

    IEnumerator ProgressAnim(float duration, float a, float b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            GV.bonusFillAmount = Mathf.Clamp01(Mathf.Lerp(a, b, elapsed / duration));

            yield return null;
        }
    }

    IEnumerator BonusDelay(float duration)
    {
        yield return new WaitForSeconds(duration * .8f);

        Vector3 scale = bonusObj.transform.localScale;
        Sequence seq = DOTween.Sequence();
        seq.Append(bonusObj.transform.DOScale(scale * 1.3f, .15f))
            .Append(bonusObj.transform.DOScale(scale, .15f))
            .Append(bonusObj.transform.DOScale(scale * 1.3f, .15f))
            .Append(bonusObj.transform.DOScale(scale, .15f));

        yield return new WaitForSeconds(duration * .2f);
        progressBarUpdate = true;
    }

    IEnumerator CoinOnScreen(float duration)
    {
        coinCanvGr.DOFade(1f, .1f);

        yield return new WaitForSeconds(duration);
        
        coinCanvGr.DOFade(0f, .4f);
    }
}
