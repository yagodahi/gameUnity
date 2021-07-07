using UnityEngine;

public class LevelCleaner : MonoBehaviour
{
    [SerializeField]
    private GameObject LevelGenerator;
    [SerializeField]
    private GameObject bgGenerator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obs")
        {
            Destroy(other.gameObject);
        } 

        if(other.gameObject.tag == "Glass")
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "bgCrystal")
        {
            bgGenerator.GetComponent<BgGenerator>().BgSpawn();
        }

        Destroy(other.gameObject, 1f);

        LevelGenerator.GetComponent<LevelGenerator>().LevelUpdate();
    }
}
