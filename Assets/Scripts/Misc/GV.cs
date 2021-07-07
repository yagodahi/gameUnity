using UnityEngine;
using System.Collections.Generic;

public static class GV //global variables
{
    //for save stuff
    public static int best { get; set; }
    public static int coins { get; set; }
    public static int level { get; set; }
    public static bool sound { get; set; }
    public static bool vibro { get; set; }
    public static bool showAds { get; set; }
    public static float levelFillAmount { get; set; }
    public static SkinStructSave currentSkin { get; set; }

    //common stuff
    public static List<SkinStructSave> skinsData { get; set; }
    public static GameObject[] skinsPrefabs { get; set; }
    public static bool inLvl { get; set; }
    public static bool activeInput { get; set; }
    public static bool interstitial { get; set; }
    public static bool review { get; set; }
    public static bool wasLevelUp { get; set; }
    public static int coinsScore { get; set; }
    public static int missCounter { get; set; }
    public static int experience { get; set; }
    public static int score { get; set; }
    public static float bonusFillAmount { get; set; }    
    public static float bonusDuration { get; set; }
    public static string rewardedId { get; set; }

    //once per launch
    public static bool oncePerLaunch { get; set; }
    public static int interstPeriod { get; set; }
    public static int reviewPeriod { get; set; }
}
