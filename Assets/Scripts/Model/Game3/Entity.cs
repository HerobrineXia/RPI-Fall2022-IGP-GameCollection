using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    public float maxHealth;
    public float speed;
    public float attackDamage;
    public float attackMultiplier;
    public float bulletPerSecond;
    public float attackSpeed;
    public float attackRange;
    public float armor;
    public float luck;

    public float hitCD;
    private float currentHitCD;

    internal EntityStats stats;

    public delegate void BulletShoot(Entity entity, Projectile bullet, Vector2 direction);

    private event BulletShoot OnBulletShooted;

    internal Rigidbody2D rigid;
    internal Animator animator;


    // Use this for initialization
    void Start()
    {
        rigid.freezeRotation = true;
        currentHitCD = 0.0f;
       
        OnInitialize();
    }

    void Update() {
        currentHitCD = Mathf.Clamp(currentHitCD - Time.deltaTime, 0.0f, hitCD);
        OnUpdate();
    }

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = new EntityStats(maxHealth, speed, attackDamage, attackMultiplier,
                               bulletPerSecond, attackSpeed, attackRange, armor, luck);
    }

    public virtual void OnInitialize() { }

    public virtual void OnUpdate() { }

    public virtual void OnEntityHit(Entity targetEntity, Projectile damageSource) { }

    public virtual void OnEntityHurt(Entity sourceEntity, Projectile damageSource) { }

    // IGP feature point: Publisher Subscriber events
    public void OnBulletShoot(Projectile bullet, Vector2 direction) {
        OnBulletShooted?.Invoke(this, bullet, direction);
    }

    public void AddBulletShootListener(BulletShoot listener) {
        OnBulletShooted += listener;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (currentHitCD == 0.0f && collision.gameObject.CompareTag("Entity")) {
            if (this.GetType() == typeof(Player3) && collision.gameObject.GetComponent<Entity>().GetType() == typeof(Enemy)) {
                OnEntityHurt(collision.gameObject.GetComponent<Entity>(), null);
                collision.gameObject.GetComponent<Entity>().OnEntityHurt(this, null);
                currentHitCD = hitCD;
            }
        }
    }

    public void RemoveBulletShootListener(BulletShoot listener) {
        OnBulletShooted -= listener;
    }
}

