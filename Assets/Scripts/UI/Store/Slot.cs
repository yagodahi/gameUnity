using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;
using Firebase.Analytics;

public class Slot : MonoBehaviour, IUnityAdsListener
{
    public Image iconPrice;

    [SerializeField]
    private RawImage ri;
    [SerializeField]
    private GameObject bgColor;
    [Space]

    [SerializeField]
    private GameObject priceObj;
    [SerializeField]
    private GameObject selectButton;
    [SerializeField]
    private TextMeshProUGUI price;
    [SerializeField]
    private GameObject warningPanel;
    [Space]

    [SerializeField]
    private GameObject modal;
    [SerializeField]
    private GameObject cameraIcon;
    [Space]

    [SerializeField]
    private Color[] bgColors;
    [SerializeField]
    private Color[] buttonColors;

    [HideInInspector]
    public SkinStructSave skinData;
    [HideInInspector]
    public Sprite spritePrice;
    [HideInInspector]
    public GameObject skinPref;
    [HideInInspector]
    public Charc character;
    [HideInInspector]
    public GameManager GM;
    [HideInInspector]
    public Store store;
    [HideInInspector]
    public Vector3 iconSetupOffset;

    private Camera cam;
    private GameObject camIconInst;
    private GameObject skinPrefIconInst;
    private GameObject modalInst;
    private RenderTexture rt;

    void Start()
    {
        camIconInst = Instantiate(cameraIcon, cameraIcon.transform.position + iconSetupOffset, cameraIcon.transform.rotation);
        skinPrefIconInst = Instantiate(skinPref, skinPref.transform.position + camIconInst.transform.position, skinPref.transform.rotation);
        skinPrefIconInst.GetComponentInChildren<ParticleSystem>().Stop();

        cam = camIconInst.GetComponent<Camera>();

        rt = new RenderTexture(230, 230, 16, RenderTextureFormat.ARGB32);

        SkinCondCheck();

        cam.targetTexture = rt;
        ri.texture = rt;

        iconPrice.sprite = spritePrice;

        UpdateSlot();
    }

    //call from ui
    public void CheckSkin()
    {
        if (GV.currentSkin != skinData)
        {
            if (skinData.skinCond == SkinCond.forSale)
            {
                switch (skinData.priceType)
                {
                    case PriceType.onMoney:
                        if (GV.coins >= skinData.price)
                        {

                            modalInst = Instantiate(modal, store.transform);

                            modalInst.GetComponent<ModalSkin>().itemIcon.texture = rt;
                            modalInst.GetComponent<ModalSkin>().slot = this;
                            modalInst.GetComponent<ModalSkin>().priceType = skinData.priceType;
                        }
                        else
                        {
                            Reaction("Collect gems and buy this item");
                        }
                        break;
                    case PriceType.onLevel:
                        break;
                    case PriceType.onAd:
                        modalInst = Instantiate(modal, store.transform);

                        modalInst.GetComponent<ModalSkin>().itemIcon.texture = rt;
                        modalInst.GetComponent<ModalSkin>().slot = this;
                        modalInst.GetComponent<ModalSkin>().priceType = skinData.priceType;
                        break;
                }
            }

            if (skinData.skinCond == SkinCond.sold)
            {
                SkinSetup();
                store.SlotChanges();
            }
        }
    }

    void UpdateSlot()
    {
        switch (skinData.priceType)
        {
            case PriceType.onMoney:
                
                if (GV.coins < skinData.price && skinData.skinCond == SkinCond.forSale)
                {
                    ri.color = new Color32(20, 20, 20, 255);
                }

                bgColor.GetComponent<Image>().color = bgColors[0];
                priceObj.GetComponent<Image>().color = buttonColors[0];

                price.text = skinData.price.ToString();

                break;
            case PriceType.onLevel:

                bgColor.GetComponent<Image>().color = bgColors[2];
                priceObj.GetComponent<Image>().color = buttonColors[2];

                price.text = "level " + skinData.price.ToString();

                break;
            case PriceType.onAd:

                bgColor.GetComponent<Image>().color = bgColors[1];
                priceObj.GetComponent<Image>().color = buttonColors[1];

                price.text = skinData.adWatched.ToString() + "/" + skinData.price.ToString();

                break;
        }
    }

    public void SkinBuyingForMoney()
    {
        skinData.skinCond = SkinCond.sold;
        GM.CoinPickup(-skinData.price);

        SkinSetup();
        store.SlotChanges();
        modalInst.GetComponent<ModalSkin>().Destroing();

        FirebaseAnalytics.LogEvent("skin_buying", "money", skinPref.name);
    }

    void SkinBuyingForAd()
    {
        if (skinData.adWatched == skinData.price)
        {
            skinData.skinCond = SkinCond.sold;
            SkinSetup();
            store.SlotChanges();
            modalInst.GetComponent<ModalSkin>().Destroing();
            FirebaseAnalytics.LogEvent("skin_buying", "ad", skinPref.name);
        }
    }

    public void SkinBuingForLevel()
    {
        if (skinData.skinCond == SkinCond.forSale)
        {
            if (GV.level >= skinData.price)
            {
                skinData.skinCond = SkinCond.sold;

                SkinDataRewrite();

                FirebaseAnalytics.LogEvent("skin_buying", "on_level", skinPref.name);
            }
        }
    }

    public void ShowAd()
    {
        if (skinData.adWatched < skinData.price)
        {
            Advertisement.AddListener(this);
            Advertisement.Show(GV.rewardedId);
        }
    }

    public void OnAdWatchFinish()
    {
        skinData.adWatched++;

        for (int i = 0; i < GV.skinsData.Count; i++)
        {
            if (skinData.name == GV.skinsData[i].name)
            {
                GV.skinsData[i] = skinData;
                break;
            }
        }

        SkinBuyingForAd();
    }

    void SkinDataRewrite()
    {
        for (int i = 0; i < GV.skinsData.Count; i++)
        {
            if (skinData.name == GV.skinsData[i].name)
            {
                GV.skinsData[i] = skinData;
                break;
            }
        }
    }

    void SkinSetup()
    {
        SkinDataRewrite();

        GV.currentSkin = skinData;

        SaveLoad.Save(new GameDataSave(), "game");

        character.SkinSetup(skinPref);
    }

    public void SkinCondCheck()
    {
        if (skinData.skinCond == SkinCond.forSale)
        {
            priceObj.SetActive(true);
            selectButton.SetActive(false);
            UpdateSlot();
        }
        else
        {
            priceObj.SetActive(false);
            selectButton.SetActive(true);
        }

        if (GV.currentSkin.name == skinData.name)
        {
            priceObj.SetActive(false);
            selectButton.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (camIconInst != null)
        {
            camIconInst.SetActive(false);
        }

        if (skinPrefIconInst != null)
        {
            skinPrefIconInst.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (camIconInst != null)
        {
            camIconInst.SetActive(true);
        }

        if (skinPrefIconInst != null)
        {
            skinPrefIconInst.SetActive(true);
            skinPrefIconInst.GetComponentInChildren<ParticleSystem>().Stop();
        }
        
    }

    void Reaction(string message)
    {
        GameObject rcInst = Instantiate(warningPanel, store.transform);

        rcInst.GetComponent<WarningPanel>().Reaction(message);
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
            OnAdWatchFinish();
            UpdateSlot();
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
