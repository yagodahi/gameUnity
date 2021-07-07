using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UIManager : MonoBehaviour
{
    [Header("Frameworks")]
    [SerializeField]
    private GameManager GM;
    [Space]

    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject skinForLevel;
    [SerializeField]
    private GameObject gift;
    [SerializeField]
    private GameObject review;
    [SerializeField]
    private GameObject hud;
    [SerializeField]
    private GameObject tutorial;
    [SerializeField]
    private GameObject gameOverMenu;
    [SerializeField]
    private GameObject store;

    [Header("Misc")]
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject uiCamera;
    [Space]

    [SerializeField]
    private List<int> giftOnLevel;
    [Space]

    public List<SkinStructSave> skinsData;
    public GameObject[] skinsPrefabs;

    private string myPlacementId = "video";
    private int interstPeriod;
    

#if UNITY_IOS
    private string gameId = "1486551";
#elif UNITY_ANDROID
    private string gameId = "4029527";
#endif

    private void Awake()
    {
        if (!SaveLoad.SaveExists("skins"))
        {
            SaveLoad.Save<List<SkinStructSave>>(skinsData, "skins");
            
            GV.currentSkin = skinsData[0];
        }
        else
        {
            skinsData = SaveLoad.Load<List<SkinStructSave>>("skins");
        }

        GV.skinsData = skinsData;
        GV.skinsPrefabs = skinsPrefabs;

        Advertisement.Initialize(gameId, false);
    }

    private void Start()
    {
        mainCamera.SetActive(true);
        uiCamera.SetActive(false);

        GM.OnStartGame += OnGameStart;
        GM.OnGameOver += OnGameOverDelay;
        GM.OnSecondLife += OnSecondLife;
        GM.OnTutorial += OnTutorial;
        GM.OnLevelReload += OnLevelReload;

        GroupOn(menu);
        GroupOff(hud);
        GroupOff(gameOverMenu);
        GroupOff(store);

        StartCoroutine(RegularCheck(1f));

        SkinCheck();
        OnLevel();

        if (GV.review)
        {
            GroupOn(review);

            GV.review = false;
        }        
    }

    void OnLevel()
    {
        for (int i = 0; i < giftOnLevel.Count; i++)
        {
            if (GV.level == giftOnLevel[i] + 1 && GV.wasLevelUp)
            {
                GroupOn(gift);
                GV.wasLevelUp = false;
            }
        }
    }

    private void GroupOn(GameObject go)
    {
        if (go != null && !go.activeSelf)
        {
            go.SetActive(true);
        }
    }

    private void GroupOff(GameObject go)
    {
        if (go != null && go.activeSelf)
        {
            go.SetActive(false);
        }
    }

    public void OnGameStart()
    {
        GroupOff(menu);
        GroupOn(hud);

        Destroy(title.gameObject, 2f);
    }

    void OnGameOverDelay()
    {
        this.Delay(OnGameOver, 1f);
    }

    public void OnGameOver()
    {
        GroupOff(hud);
        GroupOff(tutorial);
        GroupOn(gameOverMenu);
    }

    void OnTutorial()
    {
        GroupOff(hud);
        GroupOn(tutorial);
    }

    void OnSecondLife()
    {
        GroupOff(gameOverMenu);
        GroupOn(hud);
    }

    public void OpenStore()
    {
        GetComponent<Banner>().HideBanner();
        GM.Pause(true);

        mainCamera.SetActive(false);
        uiCamera.SetActive(true);
        
        GroupOff(menu);
        GroupOn(store);
    }

    public void CloseStore()
    {
        GetComponent<Banner>().ShowBanner();
        GM.Pause(false);

        mainCamera.SetActive(true);
        uiCamera.SetActive(false);
        
        GroupOn(menu);
        GroupOff(store);
    }

    void OnLevelReload()
    {
        if (GV.interstitial && GV.showAds)
        {
            Advertisement.Show(myPlacementId);
            this.Delay(Reload, .3f);
            GV.interstitial = false;
        }
        else
        {
            Reload();
        }
    }

    public void Reload()
    {
        GM.Reload();
    }

    void SkinCheck()
    {
        foreach (SkinStructSave item in skinsData)
        {
            bool onBreak = false;

            switch (item.priceType)
            {
                case PriceType.onMoney:
                    break;
                case PriceType.onLevel:

                    if (item.skinCond == SkinCond.forSale)
                    {
                        if (GV.level >= item.price)
                        {
                            item.skinCond = SkinCond.sold;

                            for (int i = 0; i < GV.skinsData.Count; i++)
                            {
                                if (item.name == GV.skinsData[i].name)
                                {
                                    GV.skinsData[i] = item;
                                    onBreak = true;
                                    break;
                                }
                            }

                            GroupOn(skinForLevel);

                            skinForLevel.GetComponent<SkinScreen>().skinData = item;

                            foreach (GameObject skinPref in GV.skinsPrefabs)
                            {
                                if (skinPref.name == item.name)
                                {
                                    skinForLevel.GetComponent<SkinScreen>().skinPref = skinPref;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case PriceType.onAd:
                    break;
            }

            if (onBreak)
            {
                break;
            }
        }
    }

    IEnumerator RegularCheck(float timer)
    {
        int sessionTime = (int)Time.realtimeSinceStartup; //rounded seconds

        //Debug.Log(sessionTime);

        //na dannyi moment esli zakryt' igru i otkryt' snova, to schetchik obnulitsa i reklama budet rezhe
        if (sessionTime % GV.interstPeriod == 0 && GV.showAds)
        {
            GV.interstitial = true;

            GV.interstPeriod -= 4;
            Mathf.Clamp(GV.interstPeriod, 30, 300);
        }

        if (sessionTime % GV.reviewPeriod == 0)
        {
            GV.review = true;

            GV.reviewPeriod += 120;
            Mathf.Clamp(GV.reviewPeriod, 180, 2000);
        }

        yield return new WaitForSeconds(timer);

        StartCoroutine(RegularCheck(timer));
    }    
}
