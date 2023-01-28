using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private float movementSpeed = 2.0f;
    private float droppingSpeed = 2.0f;

    public GameObject pumpkinPrefab;
    public GameObject spikePrefab;
    public GameObject coinPrefab;
    public GameObject heartPrefab;

    private int pumpkinThredhold = 50;
    private int coinThredhold = 70;
    private int spikeThredhold = 90;
    private int heartThredhold = 100;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DropItem", droppingSpeed);
        RandomFlip();
    }

    // Update is called once per frame
    void Update()
    {
        // IGP feature point: time.deltatime
        transform.Translate(movementSpeed * Time.deltaTime, 0, 0);
    }

    public void IncrementDifficulty() {
        if(movementSpeed <= 10.0f) {
            movementSpeed *= 1.25f;
        }
        if (droppingSpeed >= 0.2f) {
            droppingSpeed /= 1.25f;
        }
        if(pumpkinThredhold > 20) {
            pumpkinThredhold -= 5;
        }
        if(coinThredhold > pumpkinThredhold + 3) {
            coinThredhold -= 5;
        }
        if(heartThredhold > spikeThredhold + 2) {
            heartThredhold -= 1;
        }
    }

    private void RandomFlip() {
        movementSpeed *= -1.0f;
        // IGP feature point: invoke
        Invoke("RandomFlip", Random.Range(5, 10));
    }

    private void DropItem() {
        AudioSource source = GameObject.Find("Audio").GetComponent<Audio>().itemSpawn;
        if(source != null) {
            source.Play();
        }
        // IGP feature point: Random.Range
        float generate = Random.Range(0.0f, heartThredhold);
        GameObject item;
        if(generate <= pumpkinThredhold) {
            item = Instantiate<GameObject>(pumpkinPrefab);
            item.name = "Pumpkin";
        } else if(generate <= coinThredhold) {
            item = Instantiate<GameObject>(coinPrefab);
            item.name = "Coin";
        } else if(generate <= spikeThredhold) {
            item = Instantiate<GameObject>(spikePrefab);
            item.name = "Spike";
        } else {
            item = Instantiate<GameObject>(heartPrefab);
            item.name = "Heart";
        }
        item.transform.position = transform.position;
        Invoke("DropItem", droppingSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // IGP feature point: enforcing bounded movement
        if (collision.gameObject.CompareTag("Boundary")) {
            movementSpeed *= -1.0f;
        }
    }
}
