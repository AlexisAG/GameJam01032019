using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Powerup
{
    [SerializeField]
    private float _slowMultiplier = .75f;

    private GameObject m_picker;

    public override void Activate()
    {
        foreach(Player p in MapManager.Instance.Bases.Find((Base b) => b != _player.PlayerBase).Players)
        {
            p.ApplySpeedEffect(_slowMultiplier);
        }

        GameObject.Find("SpeedDebuffSE").GetComponent<AudioSource>().Play();
        _player.PowerUpCooldown = true;
        MapManager.Instance.RemoveGameObjectOnTheGrid(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.z), MapManager.TypeObject.e_PowerUp);
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }
}
