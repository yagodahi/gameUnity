using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnLvlUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Charc charc = other.GetComponent<Charc>();

        if (other.transform.tag == "Char" || other.transform.tag == "SuperChar")
        { 
            if (GV.activeInput == true)
            {
                charc.OnLevelUp();
                Destroy(gameObject);   
            }
            else if (GV.activeInput == false)
            {
                charc.OnLevelUpEnd();
                Destroy(gameObject);
            }
        }
    }
}
