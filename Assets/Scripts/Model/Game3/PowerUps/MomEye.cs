using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MomEye : Powerups
{
    private void Start() {
        itemName = "Mom Eye";
        description = "Increase Chance to shoot at the back!";
    }

    internal override void OnPowerUps(Player3 player) {
        player.stats.luck += 1;
        Debug.Log("Mom Eye Pick!");
        if (!player.powerUp.Contains(name)) {
            player.powerUp.Add(name);
            player.AddBulletShootListener(ExtraBullet);
        }
    }

    private void ExtraBullet(Entity entity, Projectile bullet, Vector2 direction) {
        if(entity.stats.luck + Random.Range(0.0f,15.0f) > 10.0f) {
            GameObject newBullet = Instantiate<GameObject>(bullet.gameObject);
            newBullet.GetComponent<Projectile>().InitializeBullet(entity, entity.stats.attackSpeed, entity.stats.attackRange, entity.stats.attackReal, -direction);
        }
    }
}
