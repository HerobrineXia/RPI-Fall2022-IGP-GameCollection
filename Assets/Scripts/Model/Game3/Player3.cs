using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player3 : Entity
{
    private Vector2 _movementVector;
    private Vector2 _fireVector;
    private float hurtTimer;
    internal Vector2 _fireRaw;

    public float hurtCD;
    public float speedMultiplier;
    public GameObject playerBulletPrefab;
    internal int coin;

    public List<string> powerUp;

    private SpriteRenderer[] srs;

    // Use this for initialization
    public override void OnInitialize()
    {
        hurtTimer = 0.0f;
        srs = GetComponentsInChildren<SpriteRenderer>();
        powerUp = new List<string>();
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (!stats.IsDead()) {
            // Input
            // IGP feature point: input system
            _movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _fireVector = new Vector2(Input.GetAxis("HorizontalShoot"), Input.GetAxis("VerticalShoot"));
            _fireRaw = _fireVector;

            // Timer
            if (hurtTimer > 0.0f) {
                hurtTimer = Mathf.Clamp(hurtTimer - Time.deltaTime, 0.0f, float.MaxValue);
            }

            // Mouse Input Check
            if (Input.GetMouseButton(0)) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = transform.position.z;
                Vector3 direct = (mousePos - transform.position).normalized;
                _fireRaw = direct;
                _fireVector = _fireRaw.normalized;
            }

            // Shooting
            // IGP feature point: shooting
            if (stats.CanShoot()) {
                // Shooting Bullet
                if (_fireVector.magnitude > 0) {
                    if (stats.CanShoot()) {
                        stats.OnShoot();
                        AudioManager.PlayAudio("Fire");
                        animator.SetBool("OnShoot", true);
                        Invoke("ShootDone", 0.2f);
                        GameObject bulletObject = Instantiate<GameObject>(playerBulletPrefab);
                        Projectile bullet = bulletObject.GetComponent<Projectile>();
                        OnBulletShoot(bullet, _fireVector + GetComponent<Rigidbody2D>().velocity * 0.1f);
                    }
                }
            } else {
                stats.currentCD -= Time.deltaTime;
            }

            // Animation
            // IGP feature point: sprite (flip book) animations
            animator.SetFloat("Horizontal", _movementVector.x);
            animator.SetFloat("Vertical", _movementVector.y);
            animator.SetFloat("HorizontalShoot", Mathf.Abs(_fireRaw.x) > Mathf.Abs(_fireRaw.y) ? _fireRaw.x : 0.0f);
            animator.SetFloat("VerticalShoot", Mathf.Abs(_fireRaw.x) > Mathf.Abs(_fireRaw.y) ? 0.0f : _fireRaw.y);
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
                // Flip Image
                if (renderer.transform.name.Equals("Body")) {
                    renderer.flipX = _movementVector.x < 0;
                }
            }
        }
    }

    public override void OnEntityHurt(Entity sourceEntity, Projectile damageSource) {
        if(hurtTimer == 0.0f) {
            float damage = damageSource == null ? sourceEntity.stats.attackReal : damageSource.damage;
            stats.OnDamageReceive(damage);
            UIManager.UpdateHeart();
            if (stats.IsDead()) {
                AudioManager.PlayAudio("Die");
                animator.SetBool("OnDied", true);
                hurtTimer = 999.0f;
                Invoke(nameof(OnDied), 1.0f);
            } else {
                AudioManager.PlayAudio("Hurt");
                OnHurtStart();
            }
            hurtTimer = hurtCD;
        }
    }

    public override void OnEntityHit(Entity targetEntity, Projectile damageSource) {
        AudioManager.PlayAudio("FireHit");
    }

    public void ItemPickAnimation() {
        Invoke(nameof(OnItemPickEnd), 0.5f);
    }

    private void OnItemPickEnd() {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            if (sr.name.Equals("Item")) {
                sr.sprite = null;
            }
        }
        animator.SetBool("OnPickItem", false);
    }


    private void OnDied() {
        this.enabled = false;
        GameManager.OnDead();
    }

    private void OnHurtStart() {
        // IGP feature point: animate a parameter
        animator.SetBool("OnHurt", true);
        // IGP feature point: CancelInvoke
        CancelInvoke("OnHurtEnd");
        Invoke("OnHurtEnd", hurtCD * 0.2f);
        CancelInvoke("OnBlink");
        Invoke("OnBlink", hurtCD * 0.2f);
    }

    private void OnBlink() {
        foreach(SpriteRenderer sr in srs) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.0f);
        }
        Invoke("OnBlinkEnd", hurtCD * 0.1f);
    }

    private void OnBlinkEnd() {
        foreach (SpriteRenderer sr in srs) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1.0f);
        }
        if (hurtTimer > 0.0f) {
            Invoke("OnBlink", hurtCD * 0.1f);
        }
    }

    private void OnHurtEnd() {
        animator.SetBool("OnHurt", false);
    }

    private void ShootDone() {
        animator.SetBool("OnShoot", false);
    }

    private void FixedUpdate() {
        // Player Movement
        // IGP feature point: AddForce
        gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(_movementVector * stats.speed * speedMultiplier);
    }

    private void OnEnable() {
        AddBulletShootListener(BulletInitialize);
    }

    private void OnDisable() {
        RemoveBulletShootListener(BulletInitialize);
    }

    private void BulletInitialize(Entity entity, Projectile bullet, Vector2 direction) {
        bullet.InitializeBullet(entity, stats.attackSpeed, stats.attackRange, stats.attackReal, direction);
    }
}

