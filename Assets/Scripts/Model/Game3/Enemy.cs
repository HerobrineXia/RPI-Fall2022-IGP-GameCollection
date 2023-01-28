using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IGP feature point: C# inheritance
public class Enemy : Entity
{
    private GameObject target;
    public bool chasingPlayer;
    public bool shootBullet;
    public bool randomWalk;
    public float speedModifier;

    public GameObject bloodPrefab;
    public GameObject enemyBulletPrefab;

    private Vector3[] path;
    private int targetIndex;
    private Color originColor;
    private Vector2 direction;

    private SpriteRenderer sr;

    // Use this for initialization
    public override void OnInitialize() {
        target = GameObject.Find("Player");
        sr = GetComponentInChildren<SpriteRenderer>();
        originColor = sr.color;
        // A* Algorithm
        if (chasingPlayer) {
            FindPath();
        }
        if (randomWalk) {
            ChangeDirection();
        }
    }

    public override void OnEntityHurt(Entity sourceEntity, Projectile damageSource) {
        float damage = damageSource == null ? sourceEntity.stats.attackReal : damageSource.damage;
        stats.OnDamageReceive(damage);
        if (stats.IsDead()) {
            OnDead();
        }
        if (bloodPrefab != null) {
            GameObject blood = Instantiate<GameObject>(bloodPrefab);
            blood.transform.position = gameObject.transform.position;
        }
        HurtBlink(0.2f);
    }

    private void OnDead() {
        if(Random.Range(0.0f, 1.0f) > 0.9f) {
            ObjectGenerator.SpawnRandomPickUps(transform.position);
        }
        CancelInvoke();
        AudioManager.PlayAudio("KillEnemy");
        GameManager.instance.score += 10 + GameManager.instance.waves;
        Destroy(this.gameObject);
    }

    private void HurtBlink(float time) {
        sr.color = new Color(1.0f, 0.5f, 0.5f);
        // IGP feature point: invoke/invokerepeating
        CancelInvoke(nameof(RestoreColor));
        Invoke(nameof(RestoreColor), time);
    }

    private void RestoreColor() {
        sr.color = originColor;
    }

    private void ChangeDirection() {
        float rand = Random.Range(0.0f, 4.0f);
        if(rand < 1.0f) {
            direction = new Vector2(1.0f, 0.0f);
        }else if (rand < 2.0f) {
            direction = new Vector2(-1.0f, 0.0f);
        }else if (rand < 3.0f) {
            direction = new Vector2(0.0f, 1.0f);
        } else {
            direction = new Vector2(0.0f, -1.0f);
        }
        Invoke(nameof(ChangeDirection), Random.Range(2.0f, 5.0f));
    }

    // Update is called once per frame
    public override void OnUpdate() {
        // Movement
        if (chasingPlayer && Mathf.Abs((transform.position - target.transform.position).magnitude) < 1.5f) {
            Vector2 targetVector = target.transform.position - transform.position;
            GetComponent<Rigidbody2D>().velocity = targetVector.normalized * stats.speed * speedModifier;
        }

        if (randomWalk) {
            GetComponent<Rigidbody2D>().velocity = direction.normalized * stats.speed * speedModifier;
        }

        // Shooting
        if (shootBullet) {
            if (stats.CanShoot()) {
                // IGP feature point: raycast
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, target.transform.position - transform.position, Mathf.Infinity);
                foreach (RaycastHit2D hit in hits) {
                    if (hit.collider?.GetComponent<Entity>()?.GetType() == typeof(Player3)) {
                        TryShootBullet(hit.collider.GetComponent<Entity>());
                    }
                }
            } else {
                stats.currentCD -= Time.deltaTime;
            }
            
        }
    }

    private void TryShootBullet(Entity entity) {
        stats.OnShoot();
        AudioManager.PlayAudio("EnemyAttack");
        animator.SetBool("OnShoot", true);
        Invoke("ShootDone", 0.2f);
        GameObject bulletObject = Instantiate<GameObject>(enemyBulletPrefab);
        Projectile bullet = bulletObject.GetComponent<Projectile>();
        OnBulletShoot(bullet, (target.transform.position - transform.position).normalized);
    }

    private void ShootDone() {
        animator.SetBool("OnShoot", false);
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


    // Path Finding
    public void FindPath() {
        if(Mathf.Abs((transform.position - target.transform.position).magnitude) > 1.0f) {
            PathFindingManager.RequestPath(transform.position, target.transform.position, OnPathFound);
        } else {
            StopCoroutine("FollowPath");
        }
        Invoke("FindPath", 0.5f);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (this != null && pathSuccessful) {
            path = newPath;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    IEnumerator FollowPath() {
        if(path.Length > 0) {
            Vector3 currentWaypoint = path[0];
            targetIndex = 0;
            while (true) {
                if (transform.position == currentWaypoint) {
                    ++targetIndex;
                    if (targetIndex >= path.Length) {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, stats.speed * Time.deltaTime * speedModifier);
                yield return null;
            }
        }
        yield return null;
    }

    // Debug
    public void OnDrawGizmos() {
        if(path != null) {
            for(int i = targetIndex; i < path.Length; ++i) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);
                if(i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                } else {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
}
