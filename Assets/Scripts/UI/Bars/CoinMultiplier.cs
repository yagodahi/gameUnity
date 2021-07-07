using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using DG.Tweening;

public class CoinMultiplier : MonoBehaviour, IUnityAdsListener
{
    [SerializeField]
    private Image coinMultBar;
    [SerializeField]
    private UIParticleSystem coinPart;
    [SerializeField]
    private GO_SecondScreen secScreen;

    private RectTransform coinMult;
    private bool mult;

    private void OnEnable()
    {
        float duration = 3f;
        coinMult = GetComponent<RectTransform>();

        coinMultBar.DOFillAmount(1f, duration).From();
        this.Delay(Collapse, duration);
    }

    public void ShowAd()
    {
        mult = true;
        Advertisement.AddListener(this);
        Advertisement.Show(GV.rewardedId);
    }

    void Collapse()
    {
        float duration = .2f;

        if (!mult)
        {
            coinMult.DOScale(Vector3.zero, duration);
            this.Delay(coinPart.Play, duration);
            this.Delay(secScreen.OnCoinMultCollaps, duration);
        }
        
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
            secScreen.OnCoinMult();

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
