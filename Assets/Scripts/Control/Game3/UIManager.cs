using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Player3 player;

    // Heart
    public Vector3 HeartStartPosition;
    public GameObject HeartPrefab;
    public GameObject HeartObjectContainer;
    private List<GameObject> heartList;
    public Sprite EmptyHeart;
    public Sprite FullHeart;

    // Score
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    // Announce
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Heart
        heartList = new List<GameObject>();
        player =  GameObject.FindObjectOfType<Player3>();
        for(int i = 0; i < player.stats.maxHealth; ++i) {
            GameObject heart = Instantiate<GameObject>(HeartPrefab);
            heart.transform.parent = HeartObjectContainer.transform;
            heart.transform.position = HeartStartPosition + new Vector3(i % 6 * 0.5f, - i / 6 * 0.5f);
            heartList.Add(heart);
        }
        instance.titleText.enabled = false;
        instance.subtitleText.enabled = false;
    }

    public static void AddHeart() {
        GameObject heart = Instantiate<GameObject>(instance.HeartPrefab);
        heart.transform.parent = instance.HeartObjectContainer.transform;
        heart.transform.position = instance.HeartStartPosition + new Vector3(instance.heartList.Count % 6 * 0.5f, - instance.heartList.Count / 6 * 0.5f);
        instance.heartList.Add(heart);
        UpdateHeart();
    }

    public static void UpdateHeart() {
        Debug.Log("Max: " + instance.player.stats.maxHealth + ", Current: " + instance.player.stats.currentHealth);
        for (int i = 0; i < instance.heartList.Count; ++i) {
            if(instance.player.stats.currentHealth < i + 1) {
                instance.heartList[i].GetComponent<SpriteRenderer>().sprite = instance.EmptyHeart;
            } else {
                instance.heartList[i].GetComponent<SpriteRenderer>().sprite = instance.FullHeart;
            }
        }
    }

    public static void UpdateScore(int score, int highScore) {
        instance.scoreText.text = "Score: " + score;
        instance.highscoreText.text = "Highest Score: " + highScore;
    }

    // IGP feature point: UI Toggle & UI Text
    public static void Announce(string title, string subtitle) {
        if(title != null) {
            instance.titleText.text = title;
            instance.titleText.enabled = true;
        } else {
            instance.titleText.enabled = false;
        }
        if (subtitle != null) {
            instance.subtitleText.text = subtitle;
            instance.subtitleText.enabled = true;
        } else {
            instance.subtitleText.enabled = false;
        }
        instance.Invoke(nameof(AnnounceEnd), 2.0f);
    }

    private void AnnounceEnd() {
        instance.titleText.enabled = false;
        instance.subtitleText.enabled = false;
    }
}
