using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using DG.Tweening;

public class SecChanceBar : MonoBehaviour, IUnityAdsListener
{
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private Image secChanceBar;

    private RectTransform secChance;

    private void OnEnable()
    {
        float duration = 3f;
        secChance = GetComponent<RectTransform>();

        secChanceBar.DOFillAmount(1f, duration).From();
        this.Delay(Scale, duration);
    }

    void Scale()
    {
        float duration = .2f;

        secChance.DOScale(Vector3.zero, duration);
    }

    public void ShowAd()
    {
        Advertisement.AddListener(this);
        Advertisement.Show(GV.rewardedId);
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == GV.rewardedId)
        {
            //slot.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            GM.SecondLife();
            Scale();
            //OnAdWatchFinish();
            //UpdateSlot();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}
