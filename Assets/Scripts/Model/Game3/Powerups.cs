using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PickUps;

public class Powerups : Item
{
    internal string description;
    internal string itemName;

    internal override void OnPlayerCollsion(Entity entity) {
        Player3 player = entity as Player3;
        OnPowerUps(player);
        UIManager.Announce(itemName, description);
        AudioManager.PlayAudio("PickPower");
        player.animator.SetBool("OnPickItem", true);
        foreach(SpriteRenderer sr in player.GetComponentsInChildren<SpriteRenderer>()) {
            if (sr.name.Equals("Item")) {
                foreach (SpriteRenderer sr2 in GetComponentsInChildren<SpriteRenderer>()) {
                    if (sr2.name.Equals("ItemSprite")) {
                        sr.sprite = sr2.sprite;
                    }
                }
            }
        }
        player.ItemPickAnimation();
        Destroy(this.gameObject);
    }

    internal virtual void OnPowerUps(Player3 player) { }

    public string GetName() {
        return name;
    }

    public string GetDescription() {
        return description;
    }
}
