using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour {
    public string gameName;
    public eventType buttonEvent;
    public AudioSource clickSound;

    public enum eventType {
        Enter = 1,
        Exit = 2,
        Start = 3
    };

    // Start is called before the first frame update
    void Start() {
        // IGP feature point: UI buttons
        GetComponent<Button>().onClick.AddListener(OnClick);
        Debug.Log("Control.ChangeScene: Finish loading the script.");
    }

    private void OnClick() {
        if(buttonEvent == 0) {
            return;
        }
        // IGP feature point: trigger sounds
        if (clickSound != null) {
            clickSound.Play();
        }
        string scenePath = "";
        // IGP feature point: reuse a script between dissimilar objects
        switch (buttonEvent) {
            case eventType.Enter:
                scenePath = gameName + "/MainMenu";
                break;
            case eventType.Exit:
                scenePath = "Menu";
                break;
            case eventType.Start:
                scenePath = gameName + "/Game";
                break;
        }
        Debug.Log("Control.ChangeScene: Loading " + scenePath);
        // IGP feature point: coroutines
        StartCoroutine(LoadScene(scenePath));
    }

    public static IEnumerator LoadScene(string scenePath) {
        if (SceneUtility.GetBuildIndexByScenePath("Scenes/" + scenePath) == -1) {
            Debug.Log("Control.ChangeScene: Unable to find " + scenePath);
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/" + scenePath);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            Debug.Log("Control.ChangeScene: Finish loading " + scenePath);
            yield return null;
        }
    }
}
