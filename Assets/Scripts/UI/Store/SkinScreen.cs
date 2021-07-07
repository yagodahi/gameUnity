using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;

public class SkinScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mainText;
    [SerializeField]
    private RawImage ri;
    [SerializeField]
    private Charc character;
    [SerializeField]
    private GameObject cameraIcon;

    [HideInInspector]
    public GameObject skinPref;
    [HideInInspector]
    public SkinStructSave skinData;

    private Camera cam;
    private GameObject camIconInst;
    private GameObject skinPrefIconInst;
    private RenderTexture rt;

    // Start is called before the first frame update
    void Start()
    {
        camIconInst = Instantiate(cameraIcon, cameraIcon.transform.position, cameraIcon.transform.rotation);
        skinPrefIconInst = Instantiate(skinPref, skinPref.transform.position + camIconInst.transform.position, skinPref.transform.rotation);
        skinPrefIconInst.GetComponentInChildren<ParticleSystem>().Stop();

        cam = camIconInst.GetComponent<Camera>();

        rt = new RenderTexture(230, 230, 16, RenderTextureFormat.ARGB32);

        cam.targetTexture = rt;
        ri.texture = rt;
        mainText.text = "Level " + skinData.price + " achieved!";

        FirebaseAnalytics.LogEvent("skin_buying", "on_level", skinPref.name);
    }

    public void SkinSetup()
    {
        GV.currentSkin = skinData;

        SaveLoad.Save(new GameDataSave(), "game");

        character.SkinSetup(skinPref);

        Destroing();
    }

    public void Destroing()
    {
        SaveLoad.Save(GV.skinsData, "skins");
        Destroy(gameObject);
    }
}
