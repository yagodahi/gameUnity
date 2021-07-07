[System.Serializable]
public class GameDataSave
{
    public int best;
    public int level;
    public int money;
    public float levelAmount;
    public bool sound;
    public bool vibro;
    public bool showAds;
    public SkinStructSave currentSkin;

    public GameDataSave()
    {
        best = GV.best;
        vibro = GV.vibro;
        money = GV.coins;
        sound = GV.sound;
        showAds = GV.showAds;
        level = GV.level;
        levelAmount = GV.levelFillAmount;
        currentSkin = GV.currentSkin;
    }
}
