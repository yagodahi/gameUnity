using UnityEngine;

public class ObstacleCrystal : SolidObj
{
    protected override void Start()
    {
        
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "SuperChar")
        {
            Vector3 contact = collision.contacts[0].point;

            fracInst.GetComponent<FracturedObj>().Explode(contact);
            mc.isTrigger = true;
            mr.enabled = false;
        }

        if (collision.transform.tag == "Char")
        {
            collision.gameObject.GetComponent<Charc>().GameOver(collision.contacts[0].point.y);
        }
    }

    protected override void SpawnBurst()
    {
       
    }

    public void OnBonusTrigger()
    {
        //Debug.Log(11);
        mc = gameObject.GetComponent<MeshCollider>();
        mr = gameObject.GetComponent<MeshRenderer>();
        fracInst = Instantiate(fractured, transform.position, transform.rotation);
        fracInst.transform.localScale = transform.localScale / 100f;
    }
}
