using UnityEngine;

public class FracturedObj : MonoBehaviour
{
    [SerializeField]
    private float maxForce;
    [SerializeField]
    private float radius;
    [SerializeField]
    private Vector3 torque;

    void Start()
    {
        foreach (Transform t in transform)
        {
            t.GetComponent<MeshRenderer>().enabled = false;
            t.GetComponent<MeshCollider>().enabled = false;
            t.GetComponent<Rigidbody>().isKinematic = true;
            t.GetComponent<Collider>().isTrigger = true;
        }
    }

    public void Explode(Vector3 hit)
    {

        foreach (Transform t in transform)
        {
            t.GetComponent<MeshRenderer>().enabled = true;
            t.GetComponent<MeshCollider>().enabled = true;
            t.GetComponent<Rigidbody>().isKinematic = false;
            t.GetComponent<Collider>().isTrigger = true;

            var rb = t.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(maxForce, hit, radius);
                rb.AddTorque(torque.Random(torque, Vector3.zero));
            }
        }
        
        Destroy(gameObject, 3f);
    }
}
