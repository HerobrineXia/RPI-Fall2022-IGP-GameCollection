using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroom : Powerups
{
    private void Start() {
        itemName = "Magic Mushroom";
        description = "All Stats Up!";
    }

    internal override void OnPowerUps(Player3 player) {
        player.stats.attackDamage += 1;
        player.stats.attackRange += 1;
        player.stats.attackSpeed += 0.2f;
        player.stats.bulletPerSecond += 1;
        player.stats.maxHealth += 1;
        player.stats.currentHealth += 2;
        UIManager.AddHeart();
        player.stats.speed += 0.2f;
        Debug.Log("Magic Mushroom Pick!");
        if (!player.powerUp.Contains(name)) {
            player.powerUp.Add(name);
        }
    }
}
