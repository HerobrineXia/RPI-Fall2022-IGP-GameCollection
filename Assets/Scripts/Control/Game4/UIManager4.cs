using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager4 : MonoBehaviour
{
    public static UIManager4 instance;
    private Player3 player;

    // Heart
    [Header("Heart")]
    public GameObject heartContainer;
    public GameObject heartPrefab;
    private List<GameObject> heartList;
    public static void RefreshHealth(int count)
    {
        if(count >= 0)
        {
            while (instance.heartList.Count != count)
            {
                if (instance.heartList.Count > count)
                {
                    Destroy(instance.heartList[instance.heartList.Count - 1]);
                    instance.heartList.RemoveAt(instance.heartList.Count - 1);
                }
                else
                {
                    GameObject heart = Instantiate<GameObject>(instance.heartPrefab, instance.heartContainer.transform);
                    heart.transform.localPosition = new Vector2(30.0f * instance.heartList.Count, 0.0f);
                    instance.heartList.Add(heart);
                }
            }
        }
    }

    // Score
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    // Announce
    public TextMeshProUGUI titleText;

    private void Awake() {
        instance = this;
        heartList = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        instance.titleText.enabled = false;
    }

    public static void UpdateScore(int score, int highScore) {
        instance.scoreText.text = "Score: " + score;
        instance.highscoreText.text = "Highest Score: " + highScore;
    }

    // IGP feature point: UI Toggle & UI Text
    public static void Announce(string title) {
        if(title != null) {
            instance.titleText.text = title;
            instance.titleText.enabled = true;
        } else {
            instance.titleText.enabled = false;
        }
        instance.Invoke(nameof(AnnounceEnd), 2.0f);
    }

    private void AnnounceEnd() {
        instance.titleText.enabled = false;
    }
}
