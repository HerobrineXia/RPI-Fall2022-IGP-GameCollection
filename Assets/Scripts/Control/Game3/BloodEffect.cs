using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public float liveTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, liveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
