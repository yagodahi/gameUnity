using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Frameworks")]
    [SerializeField]
    private GameManager GM;

    [Header("Objects")]
    [SerializeField]
    private GameObject[] obstacles;
    [SerializeField]
    private GameObject smallObstacle;
    [SerializeField]
    private GameObject window;
    [SerializeField]
    private GameObject dangWindow;
    [SerializeField]
    private GameObject cameraRef;
    [SerializeField]
    private GameObject coin;
    [SerializeField]
    private GameObject endTrigger;

    [Header("Settings")]
    [SerializeField]
    private int virtLevelSize; // Based on Windows quantity | min size 8 
    [SerializeField]
    private int coinQuantOnFin;

    [Header("Level colors")]
    [SerializeField]
    private List<Color> colors;

    private bool obsOrientUpOrDown = true; //True = up, false = down
    private bool onBonus = false;
    private bool firstGeneration = true;
    private bool flipFlop = true;
    private bool shortSegments;
    private bool smallObs;
    private bool obsOrientZ;
    private float selfHalfWidthObj;
    private float lastObjHalfWidth;
    private float localScale = 200f;
    private float windowWidth;
    private float windowScaleMult;
    private float spawnDistanceMult;
    private Vector3 lastObjectPos;
    private Vector3 obsPosOffset = new Vector3(12f, 0f, 0f);
    private Vector3 curObjPos;
    private int quantityObsInSegment = 19;
    private int quantInWave = 6;
    private int windowsCounter;
    private int virtLevelSizeConst;
    private int shortSegmentsCount = 5;
    private int smallObsCount;
    private int longSegmentsCount = 2;
    private int colorIndex = 0;
    private GameObject obstacle;

    void Start()
    {
        GM.OnBonus += OnBonus;
        GM.OnLevelUp += OnLevelUp;
        GM.OnMiss += OnMiss;
        GM.OnDiffLvl += OnDiffLvl;

        obstacle = obstacles[Random.Range(0, 3)];
        colorIndex = Random.Range(0, colors.Count);
        windowWidth = 4f;
        windowScaleMult = .7f;
        spawnDistanceMult = 4.5f;
        virtLevelSizeConst = virtLevelSize;

        Renderer mc = obstacle.GetComponent<Renderer>();
        mc.sharedMaterial.SetColor("_Color", colors[colorIndex]);

        LevelUpdate();
    }

    //---------------------------------
    //----- Methods of generation -----
    //---------------------------------

    //Called from Level Cleaner
    public void LevelUpdate()
    {
        if (obsPosOffset.x * spawnDistanceMult + cameraRef.transform.position.x > lastObjectPos.x || firstGeneration)
        {
            ObsQuantSetup();

            if (smallObs)
            {
                SmallObstaclesSpawn();

                if (StFunc.RandomBool())
                {
                    WindowSpawn(true, windowWidth);
                }
                else
                {
                    DangWindowSpawn(false, windowWidth * 1.5f);
                }
            }
            else
            {
                ObstaclesSpawn();
                WindowSpawn(true, windowWidth);
            }
        }
    }

    void ObsQuantSetup()
    {
        if (firstGeneration)
        {
            quantityObsInSegment = 15;
            firstGeneration = false;
        }
        else
        {
            if (onBonus)
            {
                if (longSegmentsCount > -1)
                {
                    if (longSegmentsCount == 0)
                    {
                        Debug.Log("long");
                        onBonus = false;
                        ObsQuantSetup();
                    }
                    else
                    {
                        longSegmentsCount--;
                        quantityObsInSegment = Random.Range(22, 25);
                    }
                }
            }
            else
            {
                if (shortSegments)
                {
                    if (shortSegmentsCount > -1)
                    {
                        if (shortSegmentsCount == 0)
                        {
                            Debug.Log("short");
                            shortSegments = false;
                            ObsQuantSetup();
                        }
                        else
                        {
                            shortSegmentsCount--;
                            quantityObsInSegment = Random.Range(2, 4);
                        }
                    }
                }
                else
                {
                    if (smallObs)
                    {
                        if (smallObsCount > -1)
                        {
                            if (smallObsCount == 0)
                            {
                                Debug.Log("small");
                                smallObs = false;
                                ObsQuantSetup();
                            }
                            else
                            {
                                smallObsCount--;
                                quantityObsInSegment = Random.Range(2, 4);
                            }
                        }
                    }
                    else
                    {
                        if (flipFlop)
                        {
                            quantityObsInSegment = Random.Range(quantInWave - 1, quantInWave + 1);
                            quantInWave--;

                            if (quantInWave < 4)
                            {
                                flipFlop = false;
                                quantInWave++;
                            }
                        }
                        else
                        {
                            quantityObsInSegment = Random.Range(quantInWave - 1, quantInWave + 1);
                            quantInWave++;

                            if (quantInWave > 9)
                            {
                                flipFlop = true;
                                quantInWave--;
                            }
                        }
                    }
                }
            }
        }
    }

    void OnBonus()
    {
        onBonus = true;
        longSegmentsCount = 2;
        shortSegments = false;
        shortSegmentsCount = 5;
        smallObs = false;
        smallObsCount = 5;
        spawnDistanceMult = 8f;

        this.Delay(OnBonusEnd, GV.bonusDuration);
    }

    void OnBonusEnd()
    {
        spawnDistanceMult = 4.5f;
    }

    void OnMiss()
    {
        if (GV.missCounter >= 2)
        {
            bool shortOrNot = StFunc.RandomBool();

            if (shortOrNot)
            {
                shortSegments = true;
                shortSegmentsCount = 5;
            }
            else
            {
                smallObs = true;
                smallObsCount = 3;
            }
            

            GV.missCounter = 0;
        }
    }

    //------------------------
    //----- Object spawn -----
    //------------------------

    void ObstaclesSpawn()
    {
        Quaternion orientation;
        selfHalfWidthObj = 1.5f;
        GameObject obstacleInst;

        for (int i = 0; i < quantityObsInSegment; i++)
        {
            //Install offset and orientation
            if (obsOrientUpOrDown)
            {
                if (obsOrientZ)
                {
                    orientation = Quaternion.Euler(0, 0, 0);
                    obsOrientZ = false;
                }
                else
                {
                    orientation = Quaternion.Euler(0, 180, 0);
                    obsOrientZ = true;
                }
            } 
            else
            {
                if (obsOrientZ)
                {
                    orientation = Quaternion.Euler(180, 0, 0);
                    obsOrientZ = false;
                }
                else
                {
                    orientation = Quaternion.Euler(180, 180, 0);
                    obsOrientZ = true;
                }
            }

            curObjPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0f, 0f) - obsPosOffset;

            obstacleInst = Instantiate(obstacle, curObjPos, orientation);

            lastObjectPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0f, 0f);

            lastObjHalfWidth = selfHalfWidthObj;

            obstacleInst.transform.localScale = new Vector3(selfHalfWidthObj * localScale, Random.Range(300, 450), localScale);
            
        }

        obsOrientUpOrDown = !obsOrientUpOrDown;
    }

    void SmallObstaclesSpawn()
    {
        selfHalfWidthObj = 1.5f;
        GameObject obstacleInst;
        bool withCoin;

        for (int i = 0; i < quantityObsInSegment; i++)
        {
            withCoin = StFunc.RandomBool();

            curObjPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0) - obsPosOffset;

            if (withCoin)
            {
                Vector3 offset;
                
                if (obsOrientUpOrDown)
                {
                    offset = new Vector3(0, 5.5f, 0);
                }
                else
                {
                    offset = new Vector3(0, -5.5f, 0);
                }

                obstacleInst = Instantiate(smallObstacle, curObjPos, Quaternion.identity);
                Instantiate(coin, curObjPos + offset, coin.transform.rotation);
            }
            else
            {
                obstacleInst = Instantiate(smallObstacle, curObjPos, Quaternion.identity);
            }

            lastObjectPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0);

            lastObjHalfWidth = selfHalfWidthObj;

            obstacleInst.transform.localScale = new Vector3(selfHalfWidthObj * localScale, Random.Range(300, 450), localScale);
        }

        obsOrientUpOrDown = !obsOrientUpOrDown;
    }

    //Window it's end of any Level segment
    void WindowSpawn(bool Counted, float halfWidth)
    {
        selfHalfWidthObj = halfWidth;
        GameObject windowInst;

        curObjPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0) - obsPosOffset;

        windowInst = Instantiate(window, curObjPos, window.transform.rotation);

        lastObjectPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0);

        lastObjHalfWidth = selfHalfWidthObj;

        windowInst.transform.localScale = new Vector3(localScale * windowScaleMult, localScale * windowScaleMult, localScale * windowScaleMult);

        if (Counted)
        {
            windowsCounter++;

            if (windowsCounter > virtLevelSize)
            {
                CoinOnLevelEndSpawn();
            }
        }
    }

    void DangWindowSpawn(bool Counted, float halfWidth)
    {
        selfHalfWidthObj = halfWidth;
        GameObject windowInst;

        curObjPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0) - obsPosOffset;

        windowInst = Instantiate(dangWindow, curObjPos, dangWindow.transform.rotation);

        lastObjectPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0);

        lastObjHalfWidth = selfHalfWidthObj;

        windowInst.transform.localScale = new Vector3(localScale * windowScaleMult, localScale * windowScaleMult, localScale * windowScaleMult);

        if (Counted)
        {
            windowsCounter++;

            if (windowsCounter > virtLevelSize)
            {
                CoinOnLevelEndSpawn();
            }
        }
    }

    void CoinOnLevelEndSpawn()
    {
        LvlEndTriggerSpawn();
        WindowSpawn(false, windowWidth);

        for (int i = 0; i < coinQuantOnFin; i++)
        {
            selfHalfWidthObj = 2.5f;

            curObjPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0) - obsPosOffset;
            
            Instantiate(coin, curObjPos, coin.transform.rotation);

            lastObjectPos = new Vector3(lastObjectPos.x + lastObjHalfWidth + selfHalfWidthObj, 0, 0);

            lastObjHalfWidth = selfHalfWidthObj;                       
        }
        obsOrientUpOrDown = true;
        LvlEndTriggerSpawn();
        WindowSpawn(false, windowWidth);
        WindowSpawn(false, windowWidth);
        WindowSpawn(false, windowWidth);
    }

    void LvlEndTriggerSpawn()
    {
        Instantiate(endTrigger, curObjPos, endTrigger.transform.rotation);
    }

    //-----------------
    //----- Tails -----
    //-----------------

    void OnLevelUp()
    {
        virtLevelSize += virtLevelSizeConst;
        onBonus = false;
        longSegmentsCount = 2;
        shortSegments = false;
        shortSegmentsCount = 5;
        smallObs = false;
        smallObsCount = 5;

        this.Delay(ObsChangeColor, .5f);
    }

    void OnDiffLvl(int diffLvl)
    {
        switch (diffLvl)
        {
            case 1:
                windowWidth = 3.7f;
                break;
            case 2:
                windowWidth = 3.4f;
                break;
            case 3:
                windowWidth = 3.1f;
                break;
            case 4:
                windowWidth = 2.8f;
                break;
            case 5:
                windowWidth = 2.5f;
                break;
        }
    }

    void ObsChangeColor()
    {
        colorIndex++;

        if (colorIndex > colors.Count - 1)
        {
            colorIndex = 0;
        }

        Renderer mc = obstacle.GetComponent<Renderer>();
        mc.sharedMaterial.SetColor("_Color", colors[colorIndex]);
    }
}
