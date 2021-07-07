using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GO_SecondScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject coinSumObj;
    [SerializeField]
    private TextMeshProUGUI coinSum;
    [Space]

    [SerializeField]
    private GameObject coinScoreObj;
    [SerializeField]
    private TextMeshProUGUI coinScore;
    [Space]

    [SerializeField]
    private GameObject coinMultButton;
    [SerializeField]
    private Image coinMultBar;
    [Space]

    [SerializeField]
    private Button continueButt;
    [SerializeField]
    private TextMeshProUGUI continueText;
    [Space]

    [SerializeField]
    private UIParticleSystem coinPart;

    private Color32 a;
    private Color32 b;
    private bool onCoinMultDoOnce = true;

    // Start is called before the first frame update
    void OnEnable()
    {
        OnGameOver();

        a = new Color32(255, 255, 255, 0);
        b = new Color32(255, 255, 255, 255);

        continueButt.interactable = false;
        continueText.color = a;

        this.Delay(ShowContinue, 2f);
    }

    void ShowContinue()
    {
        StartCoroutine(TapToCont(1.5f, a, b));
    }

    void OnGameOver()
    {
        coinSum.text = (GV.coins - GV.coinsScore).ToString();
        coinScore.text = GV.coinsScore.ToString();
    }

    public void OnCoinMult()
    {
        if (onCoinMultDoOnce)
        {
            GV.coins -= GV.coinsScore;
            GV.coinsScore *= 3;
            GV.coins += GV.coinsScore;

            coinSum.text = (GV.coins - GV.coinsScore).ToString();
            coinScore.text = GV.coinsScore.ToString();

            OnCoinMultCollaps();
            coinPart.Play();

            onCoinMultDoOnce = false;
        }
    }

    public void OnCoinMultCollaps()
    {
        Coroutine a = StartCoroutine(ScreenMoneyCount(1f, coinSum, GV.coins - GV.coinsScore, GV.coins));
        Coroutine b = StartCoroutine(ScreenMoneyCount(1f, coinScore, GV.coinsScore, 0));
    }

    IEnumerator TapToCont(float duration, Color32 a, Color32 b)
    {
        float elapsed = 0f;
        continueButt.interactable = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            continueText.color = Color.Lerp(a, b, elapsed / duration);

            yield return null;
        }
    }

    IEnumerator ScreenMoneyCount(float duration, TextMeshProUGUI text, int a, int b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            text.text = Mathf.Round(Mathf.Lerp(a, b, elapsed / duration)).ToString();

            yield return null;
        }
    }
}
