using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;

public class Player : InGameObject
{
    public AudioSource moveFail;
    public AudioSource hitWall;
    public AudioSource walk;
    public AudioSource hurt;
    public AudioSource reachGoal;
    public TextMeshProUGUI position;
    public TextMeshProUGUI information;

    private int playerStatus;

    public void OnMove(Vector2 direction) {
        if(playerStatus != 0) {
            return;
        }
        foreach (Stone stone in FindObjectsOfType(typeof(Stone)).Cast<Stone>()) {
            if (stone.Collision(new Vector2(transform.position.x + direction.x,
                                            transform.position.y + direction.y))) {
                PlaySound(hitWall);
                Debug.Log("Player hits the wall");
                information.text = string.Format("You hit the wall.");
                return;
            }
        }
        foreach (Trap trap in FindObjectsOfType(typeof(Trap)).Cast<Trap>()) {
            if (trap.Collision(new Vector2(transform.position.x + direction.x,
                                            transform.position.y + direction.y))) {
                PlaySound(hurt);
                playerStatus = -1;
                Debug.Log("Player steps on the trap");
                return;
            }
        }
        foreach (Goal goal in FindObjectsOfType(typeof(Goal)).Cast<Goal>()) {
            if (goal.Collision(new Vector2(transform.position.x + direction.x,
                                            transform.position.y + direction.y))) {
                PlaySound(reachGoal);
                playerStatus = 1;
                Debug.Log("Player hits the goal");
                return;
            }
        }
        if (Movement(direction)) {
            // Success
            Debug.Log(string.Format("Player moves to ({0},{1})",((int)transform.position.x),((int)transform.position.y)));
            PlaySound(walk);
        } else {
            // Failure
            if(moveFail != null) {
                PlaySound(moveFail);
            }
            Debug.Log("Player hits the boundary");
            information.text = string.Format("You hit the boundary.");
            return;
        }
        position.text = string.Format("Position: ({0}, {1})", transform.position.x, transform.position.y);
        information.text = string.Format("You move to ({0}, {1})", transform.position.x, transform.position.y);
    }

    public void NewGame() {
        transform.position = new Vector3(0.0f, 0.0f);
        playerStatus = 0;
        if(position != null) {
            position.text = "Position: (0, 0)";
        }
    }

    public int GetStatus() {
        return playerStatus;
    }

    private void PlaySound(AudioSource source) {
        if(source != null) {
            source.Play();
        }
    }
}

