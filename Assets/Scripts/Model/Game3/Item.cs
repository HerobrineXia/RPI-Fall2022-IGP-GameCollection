using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void Awake() {
        GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Entity") && collision.gameObject.GetComponent<Entity>().GetType() == typeof(Player3)) {
            OnPlayerCollsion(collision.gameObject.GetComponent<Entity>());
        }
    }

    internal virtual void OnPlayerCollsion(Entity entity) {
        
    }
}
