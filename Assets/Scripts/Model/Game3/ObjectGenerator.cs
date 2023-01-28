using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PickUps;

public class ObjectGenerator : MonoBehaviour
{
    public static ObjectGenerator instance;

    // PowerUps
    public GameObject[] powerUps;

    // PickUps
    public GameObject[] pickUps;

    // Enemy
    public GameObject[] enemies;

    private void Awake() {
        instance = this;
    }

    public static GameObject SpawnRandomPowerUps(Vector3 worldSpace) {
        GameObject obj = null;
        if(instance.powerUps.Length > 0) {
            // IGP feature point: Random.value or range
            int rand = Mathf.FloorToInt(Random.Range(0.0f, instance.powerUps.Length - 0.0001f));
            obj = Instantiate<GameObject>(instance.powerUps[rand]);
            AudioManager.PlayAudio("DropPower");
            obj.transform.position = worldSpace;
        }
        return obj;
    }

    public static GameObject SpawnRandomPickUps(Vector3 worldSpace) {
        GameObject obj = null;
        if (instance.pickUps.Length > 0) {
            int rand = Mathf.FloorToInt(Random.Range(0.0f, instance.pickUps.Length - 0.0001f));
            obj = Instantiate<GameObject>(instance.pickUps[rand]);
            obj.transform.position = worldSpace;
            if(obj.GetComponent<PickUps>().pickUpType == PickUpType.Coin) {
                AudioManager.PlayAudio("DropCoin");
            }else if(obj.GetComponent<PickUps>().pickUpType == PickUpType.Heart) {
                AudioManager.PlayAudio("DropHeart");
            }
        }
        return obj;
    }

    public static GameObject SpawnRandomEnemy(Vector3 worldSpace) {
        GameObject obj = null;
        if (instance.enemies.Length > 0) {
            int rand = Mathf.FloorToInt(Random.Range(0.0f, instance.enemies.Length - 0.0001f));
            obj = Instantiate<GameObject>(instance.enemies[rand]);
            obj.transform.position = worldSpace;
        }
        return obj;
    }
}
