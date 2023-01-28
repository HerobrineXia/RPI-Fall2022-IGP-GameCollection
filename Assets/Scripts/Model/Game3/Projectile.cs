using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class Projectile : MonoBehaviour
{
    public float speedMultiplier;

    internal Entity owner;
    internal float speed;
    internal float range;
    internal float damage;
    internal Vector2 direction;
    internal Vector2 startPos;

    internal Rigidbody2D rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void InitializeBullet(Entity owner, float speed, float range, float damage, Vector2 direction) {
        this.owner = owner;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        this.direction = direction;
        startPos = owner.transform.position;
        transform.position = owner.transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Entity")) {
            OnCollisionEntity(collision.gameObject.GetComponent<Entity>());
        }
        if (collision.gameObject.CompareTag("Boundary")) {
            OnCollisionBoundary();
        }
        if (collision.gameObject.CompareTag("Projectile")) {
            OnCollisionProjectile(collision.gameObject.GetComponent<Projectile>());
        }
    }

    internal virtual void OnCollisionProjectile(Projectile projectile) {
        
    }

    internal virtual void OnCollisionBoundary() {
        Destroy(this.gameObject);
    }

    internal void OnCollisionEntity(Entity entity) {
        if(entity != owner) {
            owner.OnEntityHit(entity, this);
            entity.OnEntityHurt(owner, this);
            Destroy(this.gameObject);
        }
    }
}

