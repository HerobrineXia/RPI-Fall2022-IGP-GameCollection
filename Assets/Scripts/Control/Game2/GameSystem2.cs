using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameSystem2 : MonoBehaviour
{
    private int score;
    private int highScore;
    private bool highScoreRound;
    private int nextDifficulty;

    private Stack<GameObject> basketList;

    public GameObject basketPrefab;

    private int lastHit = 0;

    // Use this for initialization
    void Start()
    {
        basketList = new Stack<GameObject>();
        float bottomEdge = transform.position.y + 0.5f;

        GameObject basket;
        Vector3 pos;
        for (int i = 0; i < 3; ++i) {
            basket = Instantiate<GameObject>(basketPrefab);
            pos = basket.transform.position;
            pos.y = bottomEdge + (i * 0.5f);
            basket.transform.position = pos;
            // IGP feature point: lists/arrays?
            // Note: I use stack to store the basket since I think it is a better
            // data structure if we only want to add/remove basket on the top.
            basketList.Push(basket);
        }

        highScoreRound = false;
        score = 0;
        if (PlayerPrefs.HasKey("Game2.HighScore")) {
            highScore = PlayerPrefs.GetInt("Game2.HighScore");
        } else {
            PlayerPrefs.SetInt("Game2.HighScore", 0);
            highScore = 0;
        }
        nextDifficulty = 100;
        RefreshText();
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public bool NewHit(GameObject hit) {
        if(hit.GetInstanceID() != lastHit) {
            lastHit = hit.GetInstanceID();
            return true;
        }
        return false;
    }

    // IGP feature point: Scoring system
    public void IncrementScore(int score) {
        this.score += score;
        Debug.Log("Score: " + this.score);
        if(highScoreRound || this.score > highScore) {
            if (!highScoreRound) {
                highScoreRound = true;
                // IGP feature point: triggered sounds
                AudioSource source = GameObject.Find("Audio").GetComponent<Audio>().newRecord;
                if (source != null) {
                    source.Play();
                }
            }
            highScore = this.score;
            Debug.Log("High Score: " + this.score);
        }
        RefreshText();
        nextDifficulty -= score;
        if(nextDifficulty <= 0) {
            GameObject.Find("Tree").GetComponent<Tree>().IncrementDifficulty();
            nextDifficulty = 100;
        }
    }

    public void DecrementBasket() {
        foreach (GameObject fallingObject in GameObject.FindGameObjectsWithTag("FallingObject")) {
            if (fallingObject.name.Equals("Pumpkin") || fallingObject.name.Equals("Spike")) {
                Destroy(fallingObject);
            }
        }
        if (basketList.Count > 0) {
            Destroy(basketList.Pop());
        }
        if (basketList.Count == 0){
            OnLose();
        }
    }

    public void IncrementBasket() {
        if(basketList.Count < 5) {
            // IGP feature point: instantiating prefabs
            GameObject basket = Instantiate<GameObject>(basketPrefab);
            float bottomEdge = transform.position.y + 0.5f;
            Vector3 pos = basket.transform.position;
            pos.y = bottomEdge + (basketList.Count * 0.5f);
            basket.transform.position = pos;
            basketList.Push(basket);
        } else {
            IncrementScore(100);
        }
    }

    private void OnLose() {
        if (highScoreRound) {
            // IGP feature point: playerprefs
            PlayerPrefs.SetInt("Game2.HighScore", highScore);
        }
        AudioSource source = GameObject.Find("Audio").GetComponent<Audio>().gameOver;
        if(source != null) {
            source.Play();
        }
        // IGP feature point: loadscene
        StartCoroutine(ChangeScene.LoadScene("Game2/GameEnd"));
    }

    private void RefreshText() {
        // IGP feature point: UI Text
        GameObject.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>().text = "Highest Score: " + highScore;
    }
}

