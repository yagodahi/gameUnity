using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AppsFlyerSDK;

public class LevelBar : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI currLevel;
    [SerializeField]
    private TextMeshProUGUI nextLevel;
    [SerializeField]
    private RectTransform levelBar;

    private float width = 274.4f;
    private float duration = 0f;
    private float prevFillAmount;
    private Coroutine anim;

    private void OnEnable()
    {
        LevelOnScreen();
        //this.Delay(LevelOnScreen, .1f);
    }
    private void Start()
    {
        LevelOnScreen();
    }

    public void LevelScoring()
    {
        int overallExp = 100 + (GV.level * 5);
        int leftExp = overallExp - (int)(GV.levelFillAmount * overallExp);
        prevFillAmount = GV.levelFillAmount;
        float currFillAmount;

        if (GV.experience > 0)
        {
            if (GV.experience < leftExp)
            {
                currFillAmount = prevFillAmount + (float)GV.experience / (float)overallExp;

                GV.levelFillAmount = currFillAmount;
                GV.experience = 0;
                float dur = duration;

                this.Delay(UpdateBar, dur);
            }

            if (GV.experience > leftExp)
            {
                GV.experience -= leftExp;
                GV.levelFillAmount = 0f;
                float dur = duration;
                
                this.Delay(UpdateBarOnLevelUp, dur);

                duration++;

                LevelUp();
                LevelScoring();
                
            }
        }
    }

    void UpdateBar()
    {
        anim = null;
        anim = StartCoroutine(BarAnim(1f, prevFillAmount.Remap(0f, 1f, 0f, width), GV.levelFillAmount.Remap(0f, 1f, 0f, width)));
    }

    void UpdateBarOnLevelUp()
    {
        anim = null;
        anim = StartCoroutine(BarAnim(1f, prevFillAmount.Remap(0f, 1f, 0f, width), width));
        this.Delay(OnLevelUp, 1f);
    }

    void OnLevelUp()
    {
        levelBar.sizeDelta = new Vector2(0f, 36f);

        LevelOnScreen();
    }

    void LevelUp()
    {
        GV.level++;
        GV.wasLevelUp = true;

        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_up", "level", GV.level);

        Dictionary<string, string> level = new Dictionary<string, string>();
        level.Add("level", GV.level.ToString());
        AppsFlyer.sendEvent(AFInAppEvents.LEVEL_ACHIEVED, level);
    }

    void LevelOnScreen()
    {
        currLevel.text = GV.level.ToString();
        nextLevel.text = (1 + GV.level).ToString();

        levelBar.sizeDelta = new Vector2(GV.levelFillAmount.Remap(0f, 1f, 0f, width), 36f);
    }

    IEnumerator BarAnim(float duration, float a, float b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            levelBar.sizeDelta = new Vector2(Mathf.Lerp(a, b, elapsed / duration), 36f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
