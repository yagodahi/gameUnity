using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgGenerator : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private GameObject bgGround;
    [SerializeField]
    private GameObject bgClouds;
    [Space]
    [SerializeField]
    private List<Color> fogColors; // 0 - standart, 1 - bonus, 2 - onLevelUp, 3 - death

    private bool inLvl = true;
    private Vector3 curPosObj;
    private Vector3 curPosObj2;
    private int colorIndex;
    private int previousIndex;
    private Renderer groundCol;
    private Coroutine bonusDelay;
    private Coroutine bonusIn;
    private Coroutine bonusOut;
    private Coroutine onLvlUp;
    private Coroutine onLvlUpEnd;

    // Start is called before the first frame update
    void Start()
    {
        GM.OnGameOver += OnGameOver;
        GM.OnBonus += OnBonus;
        GM.OnLevelUp += OnLevelUp;
        GM.OnLevelUpEnd += OnLevelUpEnd;
        GM.OnSecondLife += OnSecondLife;

        groundCol = bgGround.GetComponent<Renderer>();

        RenderSettings.fogColor = fogColors[colorIndex];
        //groundCol.sharedMaterial.SetColor("_Color", fogColors[colorIndex]);

        curPosObj = bgGround.transform.position;
        curPosObj2 = bgClouds.transform.position;
        BgSpawn();
        BgSpawn();
    }

    public void BgSpawn()
    {
        Instantiate(bgGround, curPosObj, bgGround.transform.rotation);
        Instantiate(bgClouds, curPosObj2, bgClouds.transform.rotation);

        curPosObj = new Vector3(curPosObj.x + 956.6f, curPosObj.y, curPosObj.z);
        curPosObj2 = new Vector3(curPosObj2.x + 956.6f, curPosObj2.y, curPosObj2.z);
    }

    void OnBonus()
    {
        float duration = GV.bonusDuration;

        previousIndex = colorIndex;
        colorIndex = 1;

        bonusDelay = null;
        bonusDelay = StartCoroutine(BonusDelay(duration));
    }

    void OnLevelUp()
    {
        previousIndex = colorIndex;
        colorIndex = 2;
        inLvl = false;

        Stop(bonusDelay);
        Stop(bonusIn);
        Stop(bonusOut);

        onLvlUp = null;
        onLvlUp = StartCoroutine(ChangeColors(.5f, previousIndex, colorIndex));
    }

    void OnLevelUpEnd()
    {
        previousIndex = colorIndex;
        colorIndex = 0;
        inLvl = true;

        Stop(onLvlUp);

        onLvlUpEnd = null;
        onLvlUpEnd = StartCoroutine(ChangeColors(.5f, previousIndex, colorIndex));
    }

    void OnGameOver()
    {
        previousIndex = colorIndex;
        colorIndex = 3;

        StartCoroutine(ChangeColors(.5f, previousIndex, colorIndex));
    }

    void OnSecondLife()
    {
        previousIndex = colorIndex;
        colorIndex = 0;

        StartCoroutine(ChangeColors(.5f, previousIndex, colorIndex));
    }

    void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator BonusDelay(float time)
    {
        bonusIn = null;
        bonusIn = StartCoroutine(ChangeColors(time * .2f, previousIndex, colorIndex));

        yield return new WaitForSeconds(time * .8f);

        if (inLvl)
        {
            previousIndex = colorIndex;
            colorIndex = 0;

            bonusOut = null;
            bonusOut = StartCoroutine(ChangeColors(time * .2f, previousIndex, colorIndex));

            //yield return new WaitForSeconds(time * .2f);
        }
    }

    IEnumerator ChangeColors(float duration, int a, int b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            RenderSettings.fogColor = UnityEngine.Color.Lerp(fogColors[a], fogColors[b], elapsed / duration);
            //groundCol.sharedMaterial.SetColor("_Color", UnityEngine.Color.Lerp(fogColors[a], fogColors[b], elapsed / duration));

            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
