using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;

public class GameSystem : MonoBehaviour
{
    public Player player;
    public RemoveTrap trapRemover;
    public AudioSource goalNoice;
    public AudioSource trapNoice;
    private int _score = 0;

    public TextMeshProUGUI information;
    public TextMeshProUGUI scoreInfo;

    // IGP feature point: scoring system
    // IGP feature point: implement class properties using get/set
    public int score {
        set {
            _score = value;
            // IGP feature point: UI Toggle
            scoreInfo.text = "Score: " + _score;
        }

        get => _score;
    }

    private float nextTime;
    // Use this for initialization
    void Start()
    {
        score = 0;
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetStatus() == 1) {
            // Win
            information.text = "You win! Starting a new round...";
            ++score;
            NewGame();
        }else if(player.GetStatus() == -1) {
            // Lose
            information.text = "You lose! The score reset! Starting a new round...";
            StartCoroutine(ChangeScene.LoadScene("Game1/GameEnd"));
        }
        if (Time.time >= nextTime) {
            //do something here every interval
            nextTime += 2;
            foreach (InGameObject tempObject in FindObjectsOfType(typeof(InGameObject)).Cast<InGameObject>()) {
                tempObject.Noice(player.transform.position);
            }
        }
        
    }

    private void NewGame() {
        trapRemover.UpdateTrapCount(5);
        player.NewGame();
        // IGP feature point: foreach
        // IGP feature point: grounding
        foreach (InGameObject tempObject in FindObjectsOfType(typeof(InGameObject)).Cast<InGameObject>()) {
            if (!tempObject.Equals(player)) {
                Destroy(tempObject.gameObject);
            }
        }
        bool occupied;
        float x = 0.0f, y = 0.0f;
        // IGP feature point: for
        for (int i = 0; i <= 10; ++i) {
            // IGP feature point: “safe” spawning
            occupied = true;
            while (occupied) {
                x = Random.Range(-5f, 5f);
                y = Random.Range(-5f, 5f);
                occupied = false;
                foreach (InGameObject tempObject in FindObjectsOfType(typeof(InGameObject)).Cast<InGameObject>()) {
                    if (tempObject.Collision(new Vector2(x, y))) {
                        occupied = true;
                    }
                }
            }
            int type = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : 0;
            if(i == 10) {
                type = 2;
            }
            GameObject newObject;
            string name;
            if (type == 0) {
                newObject = new("", typeof(Stone));
                name = "Stone";
            } else if(type == 1) {
                newObject = new("", typeof(Trap));
                newObject.GetComponent<Trap>().noice = trapNoice;
                name = "Trap";
            } else {
                newObject = new("", typeof(Goal));
                newObject.GetComponent<Goal>().noice = goalNoice;
                name = "Goal";
            }
            newObject.transform.position = new Vector3(x, y);
            newObject.name = string.Format("{0}({1},{2})", name, (newObject.transform.position.x), (newObject.transform.position.y));
        }
    }
}

