using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : Powerup
{
    public override void Activate()
    {
        RegisterManager.Instance.GetGameObjectInstance("LureSE")?.GetComponent<AudioSource>()?.Play();
        _player.PowerUpCooldown = true;
        MapManager.Instance.RemoveGameObjectOnTheGrid(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.z));
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }
}
