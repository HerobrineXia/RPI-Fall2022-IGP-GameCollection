using UnityEngine;
using System.Collections;

public class InGameObject : MonoBehaviour
{
    private Vector2 BoundaryLeftCorner = new Vector2(-5.0f, -5.0f);
    private Vector2 BoundaryRightCorner = new Vector2(5.0f, 5.0f);
    private Vector2 HitboxLeftCorner = new Vector2(-0.5f, -0.5f);
    private Vector2 HitboxRightCorner = new Vector2(0.5f, 0.5f);
    public AudioSource noice;

    public bool Movement(Vector2 direction) {
        // IGP feature point: enforcing bounded movement
        if (transform.position.x + direction.x >= BoundaryLeftCorner.x &&
            transform.position.x + direction.x <= BoundaryRightCorner.x &&
            transform.position.y + direction.y >= BoundaryLeftCorner.y &&
            transform.position.y + direction.y <= BoundaryRightCorner.y) {
            transform.position += new Vector3(direction.x, direction.y);
            return true;
        } else {
            return false;
        }
    }

    public bool Collision(Vector2 position) {
        return position.x >= transform.position.x + HitboxLeftCorner.x &&
            position.x <= transform.position.x + HitboxRightCorner.x &&
            position.y >= transform.position.y + HitboxLeftCorner.y &&
            position.y <= transform.position.y + HitboxRightCorner.y;
    }


    public void Noice(Vector2 position) {
        if(noice == null) {
            return;
        }
        // IGP feature point: dynamic volume
        if (Vector2.Distance(transform.position, position) < 5) {
            noice.volume = 1 - Mathf.Clamp(Vector2.Distance(transform.position, position)/5f, 0f, .9f);
            noice.Play();
        }
    }
}

