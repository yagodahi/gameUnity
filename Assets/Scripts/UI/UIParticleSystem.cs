using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Particle System
/// </summary>
public class UIParticleSystem : MonoBehaviour {

    public RectTransform startRect;
    public RectTransform endRect;
    public bool withoutStartEnd;
    /// <summary>
    /// Particle Image
    /// </summary>
    public Sprite Particle;

    /// <summary>
    /// Play Duration
    /// </summary>
    public float Duration = 5f;

    /// <summary>
    /// Loop Emission
    /// </summary>
    public bool Looping = true;

    /// <summary>
    /// Play Lifetime (if not loopable)
    /// </summary>
    public float Lifetime = 5f;

    /// <summary>
    /// Particle Emission Speed
    /// </summary>
    public float Speed = 5f;

    /// <summary>
    /// Particle Size (will be multiplied with the size over lifetime)
    /// </summary>
    public float Size = 1f;
    public bool randomizeSize;

    /// <summary>
    /// Particle Rotation per Second
    /// </summary>
    public float Rotation = 0f;
    public bool randomizeRotation;

    /// <summary>
    /// Play Particle Effect On Awake
    /// </summary>
    public bool PlayOnAwake = true;

    public Vector3 direction;
    /// <summary>
    /// Gravity
    /// </summary>
    public float Gravity = -9.81f;

    /// <summary>
    /// Emission Per Second
    /// </summary>
    public float EmissionsPerSecond = 10f;

    /// <summary>
    /// Initial Direction
    /// </summary>
    public Vector2 EmissionDirection = new Vector2(0,1f);

    /// <summary>
    /// Random Range where particles are emitted
    /// </summary>
    public float EmissionAngle = 90f;

    /// <summary>
    /// Color Over Lifetime
    /// </summary>
    public Gradient ColorOverLifetime;

    /// <summary>
    /// Size Over Lifetime
    /// </summary>
    public AnimationCurve SizeOverLifetime;

    /// <summary>
    /// Speed Over Lifetime
    /// </summary>
    public AnimationCurve SpeedOverLifetime;

    [HideInInspector]
    public bool IsPlaying { get; protected set; }

    protected float Playtime = 0f;
    protected Image[] ParticlePool;
    protected int ParticlePoolPointer;

    private Vector3 start;
    private Vector3 end;

	// Use this for initialization
	void Start ()
    {
        this.Delay(Position, .2f);
        this.Delay(PlayOnAwaked, .2f);

        if (ParticlePool == null)
            Init();
        
    }

    void PlayOnAwaked()
    {
        if (PlayOnAwake)
            Play();
    }

    void Position()
    {
        start = startRect.transform.position;
        end = endRect.transform.position;
    }

    private void Init()
    {
        ParticlePoolPointer = 0;
        ParticlePool = new Image[(int)(Lifetime * EmissionsPerSecond * 1.1f + 1)];
        for (int i = 0; i < ParticlePool.Length; i++)
        {

            var gameObject = new GameObject("Particle");
            gameObject.transform.SetParent(transform);
            gameObject.SetActive(false);
            ParticlePool[i] = gameObject.AddComponent<Image>();
            ParticlePool[i].transform.localRotation = Quaternion.identity;
            ParticlePool[i].transform.localPosition = Vector3.zero;
            ParticlePool[i].sprite = Particle;
        }
    }

    public void Play()
    {
        IsPlaying = true;
        StartCoroutine(CoPlay());
    }

    private IEnumerator CoPlay()
    {
        Playtime = 0f;
        var particleTimer = 0f;
        while (IsPlaying && (Playtime < Duration || Looping))
        {
            Playtime += Time.deltaTime;
            particleTimer += Time.deltaTime;
            while (particleTimer > 1f / EmissionsPerSecond)
            {
                particleTimer -= 1f / EmissionsPerSecond;
                ParticlePoolPointer = (ParticlePoolPointer + 1) % ParticlePool.Length;
                if(!ParticlePool[ParticlePool.Length - 1 - ParticlePoolPointer].gameObject.activeSelf)
                    StartCoroutine(CoParticleFly(ParticlePool[ParticlePool.Length - 1 - ParticlePoolPointer]));
            }
            
            yield return new WaitForEndOfFrame();
        }
        IsPlaying = false;
    }

    private IEnumerator CoParticleFly(Image particle)
    {
        float localSize;
        float localRotation;

        if (randomizeSize)
        {
            localSize = Random.Range(Size - 2f, Size + 2f);
        }
        else
        {
            localSize = Size;
        }

        if (randomizeRotation)
        {
            localRotation = Random.Range(Rotation -2f, Rotation + 2f);
        }
        else
        {
            localRotation = Rotation;
        }

        particle.gameObject.SetActive(true);
        particle.transform.localPosition = Vector3.zero;
        var particleLifetime = 0f;

        float randomF = Random.Range(-500f, 500f);
        //get default velocity
        var emissonAngle = new Vector3(EmissionDirection.x,EmissionDirection.y,0f);
        //apply angle
        emissonAngle = Quaternion.AngleAxis(Random.Range(-EmissionAngle / 2f, EmissionAngle / 2f), Vector3.forward) * emissonAngle;
        //normalize
        emissonAngle.Normalize();

        var gravityForce = Vector3.zero;

        while (particleLifetime < Lifetime)
        {
            particleLifetime += Time.deltaTime;

            //apply gravity
            gravityForce = direction * Gravity * particleLifetime; //vmesto direction bylo Vector3.up

            //particle.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(start, end, particleLifetime / Lifetime);

            if (!withoutStartEnd)
            {
                particle.transform.position = Vector3.Lerp(start + (Vector3.up * randomF * SizeOverLifetime.Evaluate(particleLifetime / Lifetime)), end, particleLifetime / Lifetime);
            }
            //set position
            //particle.transform.position += emissonAngle * SpeedOverLifetime.Evaluate(particleLifetime / Lifetime) * Speed + gravityForce;

            //set scale
            particle.transform.localScale = Vector3.one * SizeOverLifetime.Evaluate(particleLifetime / Lifetime) * localSize;

            //set rortaion
            particle.transform.localRotation = Quaternion.AngleAxis(localRotation * particleLifetime, Vector3.forward);

            //set color
            particle.color = ColorOverLifetime.Evaluate(particleLifetime / Lifetime);

            yield return new WaitForEndOfFrame();
        }

        particle.gameObject.SetActive(false);
    }
}
