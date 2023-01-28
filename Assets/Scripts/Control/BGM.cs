using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour {

    private static BGM _instance;

    private void Awake() {
        // IGP feature point: Background Music
        // IGP feature point: Singleton
        if (_instance != null) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            this.GetComponent<AudioSource>().Play();
        }
    }
}

