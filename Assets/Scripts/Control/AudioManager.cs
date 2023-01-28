using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [System.Serializable]
    public struct Audio {
        public string name;
        public AudioClip source;
    }

    public Audio[] audioMap;
    private Dictionary<string, AudioClip> map;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        // IGP feature point: dictionaries
        map = new Dictionary<string, AudioClip>();
        for(int i = 0; i < audioMap.Length; ++i) {
            if (!map.ContainsKey(audioMap[i].name)) {
                map.Add(audioMap[i].name, audioMap[i].source);
            }
        }
    }

    // IGP feature point: triggered sounds
    public static void PlayAudio(string name) {
        if (instance.map.ContainsKey(name)) {
            AudioSource.PlayClipAtPoint(instance.map[name], Vector3.zero, 1.0f);
        }
    }

}
