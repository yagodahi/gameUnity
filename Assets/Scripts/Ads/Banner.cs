using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class Banner : MonoBehaviour
{

    private string gameId = "4029527";
    private string placementId = "banner";

    void Start()
    {
        // Initialize the SDK if you haven't already done so:
        Advertisement.Initialize(gameId, false);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        StartCoroutine(ShowBannerWhenReady());
        
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void ShowBanner()
    {
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementId))
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (GV.showAds)
        {
            Advertisement.Banner.Show(placementId);
        }
    }
}
