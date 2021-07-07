using Firebase;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public delegate void CoinHandler();
    public event CoinHandler OnCoinPickup;

    public delegate void StartGameHandler();
    public event StartGameHandler OnStartGame;

    public delegate void GameOverHandler();
    public event GameOverHandler OnGameOver;

    public delegate void TutorialHandler();
    public event TutorialHandler OnTutorial;

    public delegate void BonusHandler();
    public event BonusHandler OnBonus;

    public delegate void LevelUpHandler();
    public event LevelUpHandler OnLevelUp;

    public delegate void LevelReloadHandler();
    public event LevelReloadHandler OnLevelReload;

    public delegate void SecondLifeHandler();
    public event SecondLifeHandler OnSecondLife;

    public delegate void LevelUpEndHandler();
    public event LevelUpEndHandler OnLevelUpEnd;

    public delegate void ScoreHandler(int scoreReaction);
    public event ScoreHandler OnScore;

    public delegate void DiffLvlHandler(int diffLvl);
    public event DiffLvlHandler OnDiffLvl; //difficulty level

    public delegate void MissHandler();
    public event MissHandler OnMiss;

    public delegate void BonusProgressHandler(float step);
    public event BonusProgressHandler OnBonusProgress;

    [SerializeField]
    private int quantityOfGlassForBonus;
    [SerializeField]
    private float bonusDuration;
    [SerializeField]
    private string rewardedId;

    private int glassCounter;
    private bool alive;
    

    private void Awake()
    {
        LoadGameData();

        //static vars default
        GV.score = 0;
        GV.bonusFillAmount = 0f;
        GV.inLvl = false;
        GV.activeInput = false;
        GV.missCounter = 0;
        GV.coinsScore = 0;
        GV.experience = 0;
        GV.bonusDuration = bonusDuration;
        GV.rewardedId = rewardedId;

        Debug.Log(GV.interstPeriod);
        Debug.Log(GV.reviewPeriod);

        if (GV.oncePerLaunch == false)
        {
            GV.interstPeriod = 90;
            GV.reviewPeriod = 150;
            GV.oncePerLaunch = true;
        }


    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);} );

        AppsFlyerSDK.AppsFlyerAndroid.startSDK();
    }

    public void SaveGameData()
    {
        if (GV.score >= GV.best)
        {
            GV.best = GV.score;

            SaveLoad.Save(new GameDataSave(), "game");
        }
        else
        {
            SaveLoad.Save(new GameDataSave(), "game");
        }
    }

    public void LoadGameData()
    {
        if (!SaveLoad.SaveExists("game"))
        {
            //default settings
            GV.vibro = true;
            GV.sound = true;
            GV.showAds = true;
            GV.level = 1;

            SaveLoad.Save(new GameDataSave(), "game");
        }
        else
        {
            GameDataSave loadData = SaveLoad.Load<GameDataSave>("game");

            GV.best = loadData.best;
            GV.coins = loadData.money;
            GV.vibro = loadData.vibro;
            GV.sound = loadData.sound;
            GV.showAds = loadData.showAds;
            GV.level = loadData.level;
            GV.levelFillAmount = loadData.levelAmount;
            GV.currentSkin = loadData.currentSkin;
        }
    }

    public void Pause(bool b)
    {
        if (b)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    //call from UI
    public void StartGame()
    {
        alive = true;

        GV.inLvl = true;
        GV.coinsScore = 0;

        OnStartGame?.Invoke();
    }

    public void GameOver()
    {
        alive = false;

        if (GV.score == 0 && GV.level == 1)
        {
            OnTutorial?.Invoke();
        }
        else
        {
            OnGameOver?.Invoke();
        }
    }

    //call from ui
    public void SecondLife()
    {
        alive = true;
        OnSecondLife?.Invoke();
    }

    //call from UI
    public void LevelReload()
    {
        OnLevelReload?.Invoke();
    }

    public void Reload()
    {
        SaveGameData();
        SceneManager.LoadScene(0);
    }

    public void LevelUp()
    {
        if (GV.score > 400 && GV.score < 900)
        {
            Debug.Log("lvl 1");
            OnDiffLvl?.Invoke(1);
        }

        if (GV.score > 900 && GV.score < 1400)
        {
            Debug.Log("lvl 2");
            OnDiffLvl?.Invoke(2);
        }

        if (GV.score > 1400 && GV.score < 1900)
        {
            Debug.Log("lvl 3");
            OnDiffLvl?.Invoke(3);
        }

        if (GV.score > 1900 && GV.score < 2400)
        {
            Debug.Log("lvl 4");
            OnDiffLvl?.Invoke(4);
        }

        if (GV.score > 2400 && GV.score < 2900)
        {
            Debug.Log("lvl 5");
            OnDiffLvl?.Invoke(5);
        }

        OnLevelUp?.Invoke();

        GV.inLvl = false;
    }

    public void LevelUpEnd()
    {
        OnLevelUpEnd?.Invoke();

        GV.inLvl = true;
    }

    public void Bonus()
    {
        if (alive)
        {
            OnBonus?.Invoke();
        }
    }

    public void BonusProgress(int points)
    {
        Score(points);
        Experience(2);

        glassCounter++;

        if (glassCounter > quantityOfGlassForBonus)
        {
            glassCounter = 0;
        }

        if (GV.inLvl)
        {
            OnBonusProgress?.Invoke(1f / quantityOfGlassForBonus);
        }
    }

    public void Score(int points)
    {
        GV.score += points;

        OnScore?.Invoke(points);
    }

    public void CoinPickup(int points)
    {
        GV.coinsScore += points;
        GV.coins += points;

        OnCoinPickup?.Invoke();
    }

    void Experience(int points)
    {
        GV.experience += points;
    }

    public void Miss()
    {
        OnMiss?.Invoke();
    }

    public void GitTest()
    {

    }
}
