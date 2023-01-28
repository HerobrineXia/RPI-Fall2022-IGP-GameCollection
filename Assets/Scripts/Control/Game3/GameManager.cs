using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int waves;
    public int _score;
    public float cooldown;
    public int monsterCount;

    public Vector2 gridWorldSize;
    private Vector3 worldBottomLeft;
    private Vector3 worldTopRight;

    private bool highestScore;
    public int preHighScore;

    public int score {
        get {
            return _score;
        }
        set {
            _score = value;
            if(!highestScore && _score > preHighScore) {
                highestScore = true;
                preHighScore = score;
                AudioManager.PlayAudio("NewRecord");
                // IGP feature point: Scoring system
                UIManager.Announce("New Highest Score", "You are now the leader!");
            }else if (highestScore) {
                preHighScore = score;
            }
            UIManager.UpdateScore(_score, preHighScore);
        }
    }

    // IGP feature point: implement the singleton pattern
    private void Awake() {
        instance = this;
        Physics2D.gravity = Vector2.zero;
        monsterCount = 0;
        if (PlayerPrefs.HasKey("Game3.HighScore")) {
            preHighScore = PlayerPrefs.GetInt("Game3.HighScore");
        } else {
            PlayerPrefs.SetInt("Game3.HighScore", 0);
            preHighScore = 0;
        }
    }

    private void OnDestroy() {
        Physics2D.gravity = new Vector2(0.0f, -9.81f);
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        worldTopRight = transform.position + Vector3.right * gridWorldSize.x / 2 + Vector3.up * gridWorldSize.y / 2;
        waves = 1;
        monsterCount = 5;
        Invoke(nameof(NewWaves), 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(waves % 5 != 0 && monsterCount == 0 && GameObject.FindObjectOfType<Enemy>() == null) {
            WavesEnd();
        }
        if(waves % 5 == 0 && monsterCount == 0 && GameObject.FindObjectOfType<Powerups>() == null) {
            WavesEnd();
        }
    }

    public static void OnDead() {
        if (instance.highestScore) {
            PlayerPrefs.SetInt("Game3.HighScore", instance.score);
        }
        // IGP feature point: StopCoroutine
        instance.StopAllCoroutines();
        instance.CancelInvoke();
        instance.StartCoroutine(ChangeScene.LoadScene("Game3/GameEnd"));
    }

    private void WavesEnd() {
        ++waves;
        if (waves % 5 != 0) {
            monsterCount = (waves / 5) * 2 + 5;
            ObjectGenerator.SpawnRandomPickUps(SafePosition());
            Invoke(nameof(NewWaves), 2.0f);
        } else {
            ObjectGenerator.SpawnRandomPowerUps(SafePosition());
        }

    }

    private void NewWaves() {
        StopCoroutine(nameof(GenerateWaves));
        AudioManager.PlayAudio("StartWave");
        UIManager.Announce("Stage " + (waves / 5 + 1) + " Wave " + (waves % 5), null);
        StartCoroutine(nameof(GenerateWaves)); 
    }

    public IEnumerator GenerateWaves() {
        while(monsterCount > 0) {
            --monsterCount;
            ObjectGenerator.SpawnRandomEnemy(SafePosition());
        }
        yield return null;
    }

    // IGP feature point: “safe” spawning
    private Vector3 SafePosition() {
        // IGP feature point: lerping
        Vector3 position = Vector3.Lerp(worldBottomLeft, worldTopRight, Random.Range(0.0f, 1.0f));
        while (!IsSafeSpawn(position)) {
            position = Vector3.Lerp(worldBottomLeft, worldTopRight, Random.Range(0.0f, 1.0f));
        }
        return position;
    }

    private bool IsSafeSpawn(Vector3 worldPos) {
        return Physics2D.OverlapCircle(worldPos, 2.5f, LayerMask.GetMask("Player")) == null &&
               Physics2D.OverlapCircle(worldPos, 0.9f, LayerMask.GetMask("Boundary")) == null;
    }
}
