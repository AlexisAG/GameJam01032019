using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Powerup
{
    [SerializeField]
    private float _speedMultiplier = 1.5f;

    public override void Activate()
    {
        foreach (Player p in _player.PlayerBase.Players)
        {
            _player.ApplySpeedEffect(_speedMultiplier);
        }

        RegisterManager.Instance.GetGameObjectInstance("SpeedBoostSE")?.GetComponent<AudioSource>()?.Play();
        _player.PowerUpCooldown = true;
        MapManager.Instance.RemoveGameObjectOnTheGrid(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.z));
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }
}
