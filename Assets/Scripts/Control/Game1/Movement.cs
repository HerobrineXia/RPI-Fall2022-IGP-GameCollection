using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public AudioSource clickSound;

    // IGP feature point: enumerations
    public enum Direction {
        North = 1,
        South = 2,
        West = 3,
        East = 4
    };

    public Player player;
    public Direction movingDirection;

    // Use this for initialization
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        Debug.Log("Control.Movement: Finish loading the script.");
    }

    private void OnClick() {
        if(movingDirection == 0) {
            return;
        }
        //if (clickSound != null) {
        //    clickSound.Play();
        //}
        // IGP feature point: switch/case
        switch (movingDirection) {
            case Direction.North:
                player.OnMove(new Vector2(1.0f, 0.0f));
                break;
            case Direction.South:
                player.OnMove(new Vector2(-1.0f, 0.0f));
                break;
            case Direction.West:
                player.OnMove(new Vector2(0.0f, -1.0f));
                break;
            case Direction.East:
                player.OnMove(new Vector2(0.0f, 1.0f));
                break;
        }
    }
}

