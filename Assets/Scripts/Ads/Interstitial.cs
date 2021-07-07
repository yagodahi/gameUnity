using UnityEngine;
using UnityEngine.Advertisements;

public class Interstitial : MonoBehaviour
{
    string myPlacementId = "video";

    public void ShowInterstitial()
    {
        // Show an ad:
        Advertisement.Show(myPlacementId);
    }
}
