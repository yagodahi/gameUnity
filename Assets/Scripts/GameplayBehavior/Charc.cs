using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charc : MonoBehaviour
{
    [Header("Frameworks")]
    [SerializeField]
    private GameManager GM;    
    [SerializeField]
    private GameObject exploreOnDeath;    
    [SerializeField]
    private ParticleSystem animeZoom;
    

    [Header("Settings")]
    [SerializeField]
    private float charPosY;
    [SerializeField]
    private float charSpeedX;
    [SerializeField]
    private float bonusSpeedMult;
    [SerializeField]
    private float transVertDuration; //.16f

    [Header("Colors")]
    [SerializeField]
    [ColorUsage(true, true)]
    private Color bonusColor;

    [ColorUsage(true, true)]
    private Color defaultCharColor;
    [ColorUsage(true, true)]
    private Color defaultTrailColor;
    private GameObject skinPrefab;
    private ParticleSystem trail;
    private Renderer charRenderer;
    private CharRotationController crc;

    private bool glassHit;
    private bool alive;
    private float charPosYconst;
    private float charPosYtemp;
    private float charSpeedXconst;
    private float speedIncrease;
    private Vector3 velocity = Vector3.zero;
    private float waveTime;
    private float waveDistance = 0f;
    private float waveRot = 0f;
    private float waveRotAmpl = 5f;
    private float waveAmplitudeY = .5f;
    private float wavePeriodInSec = .16f;
    private Renderer particalTrailRenderer;
    private TrailRenderer trailRenderer;
    private Coroutine bonusDelay;
    private List<Coroutine> bonusAnim = new List<Coroutine>();
    private int scoreStack;


    void Start()
    {
        GM.OnStartGame += OnGameStart;
        GM.OnBonus += Bonus;
        GM.OnSecondLife += SecondLife;
        GM.OnDiffLvl += OnDiffLvl;

        crc = GetComponent<CharRotationController>();

        foreach (GameObject item in GV.skinsPrefabs)
        {
            if (item.name == GV.currentSkin.name)
            {
                SkinSetup(item);
                //Debug.Log(item);
                break;
            }
        }

        //Start setup
        transform.tag = "Char";
        charPosYconst = charPosY;
        charPosYtemp = charPosY;
        charPosY = -charPosY;
        charSpeedXconst = charSpeedX;
        charSpeedX = 0f;
        speedIncrease = 1f;
    }

    public void SkinSetup(GameObject skinPrefabL)
    {
        if (skinPrefab != null)
        {
            Destroy(skinPrefab.gameObject);
        }

        Vector3 prefabPos = Vector3.zero;
        Quaternion quat = Quaternion.Euler(0f, 0f, 0f);
        float scale = GetComponent<SphereCollider>().radius;

        skinPrefab = Instantiate(skinPrefabL, transform.position + prefabPos, quat);
        skinPrefab.transform.localScale = new Vector3(scale, scale, scale);
        skinPrefab.transform.parent = transform;

        trailRenderer = skinPrefab.GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = false;
        charRenderer = skinPrefab.GetComponentInChildren<MeshRenderer>().GetComponent<Renderer>();
        crc.rotatedObj = charRenderer.gameObject;
        trail = skinPrefab.GetComponentInChildren<ParticleSystem>();
        particalTrailRenderer = trail.GetComponent<Renderer>();
        defaultCharColor = charRenderer.material.GetColor("_EmissionColor");
        defaultTrailColor = particalTrailRenderer.material.GetColor("_EmissionColor");

        if (GV.currentSkin.rotType != RotationType.none)
        {
            crc.enabled = true;
        }
        else
        {
            crc.enabled = false;
        }
    }

    void Update()
    {
        //Input block
        if (GV.activeInput)
        {
            if (Input.GetMouseButton(0))
            {
                charPosY = charPosYtemp;
            }
            else
            {
                charPosY = -charPosYtemp;
            }
        }

        //IDLE movement
        waveTime = Time.timeSinceLevelLoad / wavePeriodInSec;
        waveDistance = waveAmplitudeY * Mathf.Sin(waveTime);
        waveRot = waveRotAmpl * Mathf.Sin(waveTime);
    }


    private void FixedUpdate()
    {
        float xLoc;

        if (alive)
        {
            xLoc = transform.position.x + (charSpeedX * transVertDuration);
        }
        else
        {
            xLoc = transform.position.x;
        }

        transform.position = Vector3.SmoothDamp
                (transform.position,
                new Vector3(xLoc, charPosY + waveDistance, 0),
                ref velocity,
                transVertDuration,
                100f,
                Time.fixedDeltaTime);

        transform.rotation = Quaternion.Euler(0f, 0f, waveRot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Miss" && tag == "Char")
        {
            this.Delay(ScoreHandler, .1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Glass")
        {
            glassHit = true;
        }

        if (collision.gameObject.tag == "Coin")
        {
            GM.CoinPickup(1);

            if (GV.vibro)
            {
                Vibration.Vibrate(10L);
            }
        }

        if (collision.gameObject.tag == "Obs" && tag == "SuperChar")
        {
            GM.Score(1);
            scoreStack = 0;

            if (GV.vibro)
            {
                Vibration.Vibrate(16L);
            }
        }
    }

    void ScoreHandler()
    {
        if (glassHit)
        {
            glassHit = false;
            scoreStack++;

            GM.BonusProgress(scoreStack);

            if (GV.vibro)
            {
                Vibration.Vibrate(16L);
            }
        }
        else
        {
            Miss();
        }
    }

    public void Miss()
    {
        scoreStack = 0;
        GV.missCounter++;
        GM.Miss();
    }

    //--------------------------------
    //----- Player start/end Game ----
    //--------------------------------

    void OnGameStart()
    {
        alive = true;

        trailRenderer.enabled = true;
        StartCoroutine(SpeedChanges(.2f, charSpeedX, charSpeedXconst));
        StartCoroutine(TrailAnim(.5f, 0f, 90f, 0f, -12f, .5f, .1f));
        this.Delay(InputTrue, 1f);
    }

    void InputTrue()
    {
        GV.activeInput = true;
    }

    public void GameOver(float hit)
    {
        alive = false;
        trailRenderer.enabled = false;
        charPosY = hit;
        GV.activeInput = false;
        charSpeedX = 0f;

        GM.GameOver();

        animeZoom.Stop();
        Stop(bonusDelay);

        foreach (var item in bonusAnim)
        {
            Stop(item);
        }

        if (GV.vibro)
        {
            Vibration.Vibrate(50L);
        }

        charRenderer.enabled = false;
        GetComponent<SphereCollider>().enabled = false;

        GameObject burstInstance;
        burstInstance = Instantiate(exploreOnDeath, transform.position, transform.rotation);
        Destroy(burstInstance, .4f);

        trail.Stop();
    }

    void SecondLife()
    {
        alive = true;
        GV.activeInput = true;
        charRenderer.enabled = true;
        trailRenderer.enabled = true;

        StartCoroutine(SpeedChanges(.2f, charSpeedX, charSpeedXconst));
        StartCoroutine(TrailAnim(.5f, 0f, 90f, 0f, -12f, .5f, .1f));
        StartCoroutine(OnSecondLifeMeshAnim(4f));

        this.Delay(OnSecondLife, 4f);
    }

    void OnSecondLife()
    {
        trail.Play();
        GetComponent<SphereCollider>().enabled = true;
    }

    //----------------
    //-----States-----
    //----------------

    void Bonus()
    {
        float duration = GV.bonusDuration;

        transform.tag = "SuperChar";
        waveAmplitudeY = 1.5f;
        waveRotAmpl = 10f;
        wavePeriodInSec = .1f;

        bonusDelay = StartCoroutine(BonusPowerDelay(duration));
    }

    //---------------
    //-----Level-----
    //---------------

    public void OnLevelUp()
    {
        animeZoom.Stop();

        waveAmplitudeY = 1f;
        waveRotAmpl = 7f;
        wavePeriodInSec = .2f;

        charSpeedXconst += speedIncrease;
        charSpeedX = charSpeedXconst;

        Stop(bonusDelay);

        foreach (var item in bonusAnim)
        {
            Stop(item);
        }

        GM.LevelUp();
        StartCoroutine(YoffsetAnim(.5f, charPosYconst, 0f, false));
        StartCoroutine(SpeedChanges(.5f, charSpeedX, charSpeedXconst * 1.2f));
        StartCoroutine(CharColor(.5f, charRenderer.material.GetColor("_EmissionColor"), defaultCharColor));
        StartCoroutine(TrailColor(.5f, particalTrailRenderer.material.GetColor("_EmissionColor"), defaultTrailColor));
    }

    public void OnLevelUpEnd()
    {
        waveAmplitudeY = .5f;
        waveRotAmpl = 5f;
        wavePeriodInSec = .16f;

        GM.LevelUpEnd();
        GV.activeInput = true;

        StartCoroutine(YoffsetAnim(.5f, 0f, charPosYconst, true));
        StartCoroutine(SpeedChanges(.5f, charSpeedX, charSpeedXconst));

        if (transform.tag == "SuperChar")
        {
            transform.tag = "Char";
        }
    }

    void OnDiffLvl(int diffLvl)
    {
        switch (diffLvl)
        {
            case 1:
                speedIncrease = 1.2f;
                break;
            case 2:
                speedIncrease = 1.4f;
                break;
            case 3:
                speedIncrease = 1.5f;
                break;
            case 4:
                speedIncrease = 1.6f;
                break;
            case 5:
                speedIncrease = 1.7f;
                break;
        }
    }

    //----------------------------
    //-----Animation / Delays-----
    //----------------------------

    void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator BonusPowerDelay(float time)
    {
        animeZoom.Play();

        bonusAnim.Add((Coroutine)StartCoroutine(SpeedChanges(time * .1f, charSpeedX, charSpeedXconst * bonusSpeedMult)));
        bonusAnim.Add((Coroutine)StartCoroutine(CharColor(time * .1f, defaultCharColor, bonusColor)));
        bonusAnim.Add((Coroutine)StartCoroutine(TrailColor(time * .1f, defaultTrailColor, bonusColor)));

        yield return new WaitForSeconds(time * .1f);

        bonusAnim.Clear();

        yield return new WaitForSeconds(time * .7f);

        if (GV.inLvl)
        {
            animeZoom.Stop();

            bonusAnim.Add((Coroutine)StartCoroutine(SpeedChanges(time * .2f, charSpeedX, charSpeedXconst)));
            bonusAnim.Add((Coroutine)StartCoroutine(CharColor(time * .2f, charRenderer.material.GetColor("_EmissionColor"), defaultCharColor)));
            bonusAnim.Add((Coroutine)StartCoroutine(TrailColor(time * .2f, particalTrailRenderer.material.GetColor("_EmissionColor"), defaultTrailColor)));

            yield return new WaitForSeconds(time * .2f);

            transform.tag = "Char";
            waveAmplitudeY = .5f;
            waveRotAmpl = 5f;
            wavePeriodInSec = .16f;

            bonusDelay = null;
            bonusAnim.Clear();
        }
    }

    IEnumerator SpeedChanges(float duration, float a, float b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            charSpeedX = Mathf.Lerp(a, b, elapsed / duration);

            yield return null;
        }

        charSpeedX = b;
    }

    IEnumerator YoffsetAnim(float duration, float yCurOffset, float yNewOffset, bool activeInp)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            charPosYtemp = Mathf.Lerp(yCurOffset, yNewOffset, elapsed / duration);

            elapsed += Time.deltaTime;

            yield return null;
        }

        GV.activeInput = activeInp;
        charPosYtemp = yNewOffset;
    }

    IEnumerator TrailAnim(float duration, float aRot, float bRot, float aVel, float bVel, float aLT, float bLT)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            var Velocity = trail.velocityOverLifetime;
            var Lifetime = trail.main.startLifetime;

            trail.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(aRot, bRot, elapsed / duration));

            Velocity.yMultiplier = Mathf.Lerp(aVel, bVel, elapsed / duration);
            Lifetime = Mathf.Lerp(aLT, bLT, elapsed / duration);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator CharColor(float duration, Color a, Color b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            charRenderer.material.SetColor("_EmissionColor", Color.Lerp(a, b, elapsed / duration));

            elapsed += Time.deltaTime;

            yield return null;
        }

        charRenderer.material.SetColor("_EmissionColor", b);
    }

    IEnumerator TrailColor(float duration, Color a, Color b)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            particalTrailRenderer.material.SetColor("_EmissionColor", Color.Lerp(a, b, elapsed / duration));

            elapsed += Time.deltaTime;

            yield return null;
        }

        particalTrailRenderer.material.SetColor("_EmissionColor", b);
    }

    IEnumerator OnSecondLifeMeshAnim(float duration)
    {
        int count = 16;

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(duration / count);

            if (charRenderer.enabled == true)
            {
                charRenderer.enabled = false;
                trailRenderer.enabled = false;
            }
            else
            {
                charRenderer.enabled = true;
                trailRenderer.enabled = true;
            }
        }
    }
}

