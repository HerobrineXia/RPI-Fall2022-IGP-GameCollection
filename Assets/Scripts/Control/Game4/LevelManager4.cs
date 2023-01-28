using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager4 : MonoBehaviour
{
    public static LevelManager4 instance;

    [Header("Reference Object")]
    public GameObject grid;
    public GameObject player;
    public GameObject[] background;
    public GameObject[] farCloud;
    public GameObject[] closeCloud;

    [Header("Prefabs")]
    public GameObject normalPlatform;
    public GameObject coinPlatform;
    public GameObject heartPlatform;
    public GameObject spikePlatform;
    public GameObject verticalPlatform;
    public GameObject laserPlatform;

    [Header("Setting")]
    public int heartMin = 10;
    public int coinMin = 40;
    public int spikeMin = 60;
    public int vertMin = 80;
    public int laserMin = 100;
    public float screenWidth = 16.0f;
    public float screenHeight = 9.0f;
    public float scrollingSpeed = 5.0f;
    public float[] platformHeight;

    private List<GameObject> platformList;
    private Vector2 scrollingDirection;
    private int difficulties = 1;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        platformList = new List<GameObject>();
        GameObject platform = Instantiate<GameObject>(normalPlatform, new Vector3(0.0f, 0.0f), Quaternion.identity, grid.transform);
        platformList.Add(platform);
        for(int i = 0; i < 5; ++i)
        {
            generatePlatform(8 + i * 8, Random.Range(1, platformHeight.Length + 1));
        }
        scrollingDirection = new Vector2(-1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // IGP feature point: enforcing bounded movement
        // Player Position Boundary
        if (player.transform.position.y < -screenHeight)
        {
            player.transform.position += new Vector3(0f, screenHeight * 2);
            player.GetComponent<Player4>().OnHurt();
        }
        player.transform.position = new Vector2(Mathf.Clamp(player.transform.position.x,-screenWidth * 0.5f,screenWidth * 0.5f), Mathf.Clamp(player.transform.position.y,-screenHeight,screenHeight));
        // Scrolling
        bool needNewPlatform = false;
        for (int i = 0; i < platformList.Count; ++i)
        {
            if (platformList[i].transform.position.x < - screenWidth * 0.5f - 8.0f) 
            {
                Destroy(platformList[i]);
                platformList.RemoveAt(i);
                --i;
                needNewPlatform = true;
            }
            else
            {
                platformList[i].GetComponent<Rigidbody2D>().velocity = scrollingDirection * scrollingSpeed;
            }
        }
        // New Platform
        if(needNewPlatform)
        {
            generatePlatform(0.5f * screenWidth + 8.0f, Random.Range(1, platformHeight.Length + 1));
        }

        // Increase Difficulties
        if(GameManager4.instance.score / 2000 + 1 > difficulties)
        {
            increaseDifficulties();
        }
    }

    private void FixedUpdate() {
        // IGP feature point: parallax scrolling in code
        // Background
        scrollBackground(background, scrollingDirection * scrollingSpeed * 0.5f);
        scrollBackground(farCloud, scrollingDirection * scrollingSpeed * 0.7f);
        scrollBackground(closeCloud, scrollingDirection * scrollingSpeed * 0.9f);
    }

    // IGP cool feature: difficulties control
    private void increaseDifficulties()
    {
        scrollingSpeed = Mathf.Clamp(scrollingSpeed + 1.0f, 0.0f, 20.0f);
        difficulties++;
        heartMin = Mathf.Clamp(heartMin - 1, 5, 100);
        coinMin = Mathf.Clamp(coinMin - 2, heartMin + 10, 100);
        spikeMin = Mathf.Clamp(spikeMin - 1, coinMin + 30, 100);
        vertMin = Mathf.Clamp(vertMin - 2, spikeMin + 10, 100);
    }

    // IGP feature point: algorithmic level generation
    private void generatePlatform(float x, int count)
    {
        List<float> temp = new List<float>();
        for(int i = 0; i < platformHeight.Length; ++i)
        {
            temp.Add(platformHeight[i]);
        }
        int tempIndex, rand;
        int bonusCount = count - 1;
        Vector3 pos;
        for(int i = 0; i < count && i < platformHeight.Length; ++i)
        {
            tempIndex = Random.Range(0, temp.Count);
            pos = new Vector3(x, temp[tempIndex]);
            // IGP feature point: effectors (Normal platform prefabs use platform effectors to disable side friction
            addPlatform(normalPlatform, pos);
            if (bonusCount > 0) {
                rand = Random.Range(0, 100);
                if (rand < heartMin)
                {
                    addPlatform(heartPlatform, pos);
                }
                else if(rand < coinMin)
                {
                    addPlatform(coinPlatform, pos);
                }
                else if(rand < spikeMin)
                {
                    addPlatform(spikePlatform, pos);
                }else if(rand < vertMin)
                {
                    addPlatform(verticalPlatform, pos);
                }
                else if(rand < laserMin)
                {
                    addPlatform(laserPlatform, pos);
                }
                --bonusCount;
            }
            temp.RemoveAt(tempIndex);
        }
    }

    private void addPlatform(GameObject platform, Vector3 position)
    {
        platformList.Add(Instantiate<GameObject>(platform, position, Quaternion.identity, grid.transform));
    }

    private void scrollBackground(GameObject[] objList, Vector3 vec)
    {
        foreach (GameObject obj in objList)
        {
            obj.transform.position += vec * Time.deltaTime;
            if(obj.transform.position.x <= -screenWidth)
            {
                obj.transform.position += new Vector3(screenWidth * 2, 0.0f);
            }
        }
    }
}
