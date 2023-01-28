using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : Item
{
    public PickUpType pickUpType;

    public enum PickUpType {
        Heart,
        Coin
    }

    internal override void OnPlayerCollsion(Entity entity) {
        Player3 player = entity as Player3;
        if (pickUpType == PickUpType.Heart) {
            if(!Mathf.Approximately(player.stats.currentHealth, player.stats.maxHealth)) {
                player.stats.currentHealth += 1.0f;
                AudioManager.PlayAudio("PickHeart");
                UIManager.UpdateHeart();
                Destroy(this.gameObject);
            }
        }else if(pickUpType == PickUpType.Coin) {
            player.coin += 1;
            AudioManager.PlayAudio("PickCoin");
            GameManager.instance.score += 100;
            Destroy(this.gameObject);
            Debug.Log("Current Coin: " + player.coin);
        }
    }
}
