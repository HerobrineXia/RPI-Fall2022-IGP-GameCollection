using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Boundary : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // IGP feature point: colliders
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("FallingObject") && collision.gameObject.transform.position.y < -4.0f) {
            if (collision.gameObject.name.Equals("Pumpkin")) {
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameSystem2>().DecrementBasket();
                AudioSource source = GameObject.Find("Audio").GetComponent<Audio>().pumpkinGround;
                if (source != null) {
                    source.Play();
                }
            }
            Destroy(collision.gameObject);
        }
    }
}
