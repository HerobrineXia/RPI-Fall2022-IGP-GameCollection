using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    private float liveTime;

    void Start() {
        gameObject.GetComponent<Rigidbody2D>().velocity = direction * speed * speedMultiplier;
        liveTime = 0.0f;
        rigid.freezeRotation = true;
    }

    void Update() {
        liveTime += Time.deltaTime;
        if (liveTime > range) {
            Destroy(this.gameObject);
        }
    }
}
