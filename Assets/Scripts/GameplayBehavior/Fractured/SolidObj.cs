using UnityEngine;

public class SolidObj : MonoBehaviour
{
    public GameObject fractured;

    [SerializeField]
    private GameObject burst;
    [SerializeField]
    private GameObject middleburst;

    [HideInInspector] public MeshCollider mc;
    [HideInInspector] public MeshRenderer mr;
    [HideInInspector] public GameObject fracInst;

    protected virtual void Start()
    {
        mc = gameObject.GetComponent<MeshCollider>();
        mr = gameObject.GetComponent<MeshRenderer>();
        fracInst = Instantiate(fractured, transform.position, transform.rotation);
        fracInst.transform.localScale = transform.localScale / 100f;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Char" || collision.transform.tag == "SuperChar")
        {
            Vector3 contact = collision.contacts[0].point;
            SpawnBurst();
            fracInst.GetComponent<FracturedObj>().Explode(contact);
            mc.isTrigger = true;
            mr.enabled = false;
            
        }
    }

    protected virtual void SpawnBurst()
    {
        GameObject burstInstance;
        GameObject burstInstance2;

        burstInstance = Instantiate(burst, transform.position, transform.rotation);
        burstInstance2 = Instantiate(middleburst, transform.position, transform.rotation);

        Destroy(burstInstance2, .4f);
        Destroy(burstInstance, .4f);
    }

    protected virtual void OnDestroy()
    {
        if (fracInst != null)
        {
            Destroy(fracInst);
        }
    }
}
