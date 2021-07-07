using UnityEngine;

public class Review : MonoBehaviour
{
    public void OpenStore()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
