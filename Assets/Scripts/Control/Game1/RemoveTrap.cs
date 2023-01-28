using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RemoveTrap : MonoBehaviour {
    public int remainTrapRemover;
    public Player player;
    public TextMeshProUGUI information;
    public TextMeshProUGUI trapCount;

    // Use this for initialization
    void Start() {
        GetComponent<Button>().onClick.AddListener(OnClick);
        Debug.Log("Control.RemoveTrap: Finish loading the script.");
    }

    public void UpdateTrapCount(int count) {
        remainTrapRemover = count;
        trapCount.text = "Trap Remover * " + count;
    }

    private void OnClick() {
        if(remainTrapRemover > 0) {
            int count = 0;
            UpdateTrapCount(remainTrapRemover - 1);
            foreach(Trap trap in FindObjectsOfType(typeof(Trap)).Cast<Trap>()) {
                if (Vector2.Distance(trap.transform.position, player.transform.position) < 2) {
                    Destroy(trap.gameObject);
                    ++count;
                }
            }
            information.text = "You use a trap remover and remove " + count + " traps.";
        } else {
            information.text = "You don't have any trap remover left.";
        }
    }
}

