using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Music")) {
            Destroy(o);
        }
    }

    // Update is called once per frame
    void Update()
    {
            
    }
}

