using System.Collections;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GO_FirstScreen : MonoBehaviour
{
    [SerializeField]
    private LevelBar levelBar;
    [Space]

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI score;
    [Space]

    [SerializeField]
    private GameObject bestObj;
    [SerializeField]
    private TextMeshProUGUI bestScore;
    [Space]

    [SerializeField]
    private GameObject secChanceButton;
    [Space]

    [SerializeField]
    private Button continueButt;
    [SerializeField]
    private TextMeshProUGUI continueText;
    [Space]

    [SerializeField]
    private ParticleSystem confetti;

    private Color32 a;
    private Color32 b;

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
        if (GV.score <= 3)
        {
            levelBar.gameObject.SetActive(false);
            secChanceButton.SetActive(false);
            bestObj.SetActive(false);

            if (GV.score > GV.best)
            {
                //Debug.Log(1);
                GV.best = GV.score;

                scoreText.text = "New best";
                scoreText.color = new Color32(255, 250, 0, 255);
                score.color = new Color32(255, 250, 0, 255);

                confetti.Play();
            }
            else
            {
                //Debug.Log(2);
                scoreText.text = "Score";
            }
        }
        else
        {
            if (GV.score > GV.best)
            {
                //Debug.Log(5);
                levelBar.gameObject.SetActive(true);
                secChanceButton.SetActive(false);
                bestObj.SetActive(false);

                GV.best = GV.score;

                FirebaseAnalytics.LogEvent("best_score", "best", GV.best);

                confetti.Play();

                scoreText.text = "New best";
                scoreText.color = new Color32(255, 250, 0, 255);
                score.color = new Color32(255, 250, 0, 255);
            }
            else
            {
                if (GV.score > GV.best * .5f)
                {
                    //Debug.Log(6);
                    secChanceButton.SetActive(true);
                }
                else
                {
                    //Debug.Log(7);
                    secChanceButton.SetActive(false);
                }

                levelBar.gameObject.SetActive(true);
                bestObj.SetActive(true);
                scoreText.text = "Score";
            }
        }

        levelBar.LevelScoring();
        score.text = GV.score.ToString();
        bestScore.text = GV.best.ToString();
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
}
