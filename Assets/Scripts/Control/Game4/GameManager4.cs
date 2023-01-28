using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager4 : MonoBehaviour
{
    public static GameManager4 instance;
    [Header("Reference Object")]
    public GameObject player;
    public GameObject theCamera;
    public GameObject boundary;
    public GameObject levelManager;

    private bool highestScore;
    public int preHighScore;

    private int _score;
    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            if (!highestScore && _score > preHighScore)
            {
                highestScore = true;
                preHighScore = score;
                AudioManager.PlayAudio("NewRecord");
                // IGP feature point: Scoring system
                UIManager4.Announce("New Highest Score");
            }
            else if (highestScore)
            {
                preHighScore = score;
            }
            UIManager4.UpdateScore(_score, preHighScore);
        }
    }

    private void Awake() {
        instance = this;
        if (PlayerPrefs.HasKey("Game4.HighScore"))
        {
            preHighScore = PlayerPrefs.GetInt("Game4.HighScore");
        }
        else
        {
            PlayerPrefs.SetInt("Game4.HighScore", 0);
            preHighScore = 0;
        }
    }

    public static void OnDead()
    {
        if (instance.highestScore)
        {
            PlayerPrefs.SetInt("Game4.HighScore", instance.score);
        }
        // IGP feature point: StopCoroutine
        instance.StopAllCoroutines();
        instance.CancelInvoke();
        instance.StartCoroutine(ChangeScene.LoadScene("Game4/GameEnd"));
    }

    private IEnumerator AddScoreOverTime()
    {
        while (true)
        {
            score += 5; // Add 1 point to the score
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Start() {
        StartCoroutine(AddScoreOverTime());
    }
}
