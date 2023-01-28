using UnityEngine;
using System.Collections;

public class FallingObject : MonoBehaviour
{
    private int existTime = 0;

    // Use this for initialization
    void Start()
    {
        Invoke("ExpireTime", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ExpireTime() {
        existTime += 1;
        if(existTime > 5) {
            Destroy(this.gameObject);
        }
        Invoke("ExpireTime", 1);
    }
}

