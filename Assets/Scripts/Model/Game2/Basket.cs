using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Basket : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // IGP feature point: tracking the mouse
        Vector3 rawMouse = Input.mousePosition;
        Vector3 convertedMouse = Camera.main.ScreenToWorldPoint(rawMouse);
        Vector3 pos = transform.position;
        pos.x = convertedMouse.x;
        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // IGP feature point: tags
        if (collision.gameObject.CompareTag("FallingObject")
            && GameObject.Find("GameSystem").GetComponent<GameSystem2>().NewHit(collision.gameObject)) {
            string name = collision.gameObject.name;
            AudioSource source = null;
            switch (name) {
                case "Pumpkin":
                    GameObject.Find("GameSystem").GetComponent<GameSystem2>().IncrementScore(10);
                    source = GameObject.Find("Audio").GetComponent<Audio>().pumpkinHit;
                    break;
                case "Coin":
                    GameObject.Find("GameSystem").GetComponent<GameSystem2>().IncrementScore(50);
                    source = GameObject.Find("Audio").GetComponent<Audio>().coinHit;
                    break;
                case "Spike":
                    GameObject.Find("GameSystem").GetComponent<GameSystem2>().DecrementBasket();
                    source = GameObject.Find("Audio").GetComponent<Audio>().spikeHit;
                    break;
                case "Heart":
                    GameObject.Find("GameSystem").GetComponent<GameSystem2>().IncrementBasket();
                    source = GameObject.Find("Audio").GetComponent<Audio>().heartHit;
                    break;
            }
            if (source != null) {
                source.Play();
            }
            Destroy(collision.gameObject);
        }
    }
}
