﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDestroyer : Powerup
{
    [SerializeField]
    private float _damagePercent = .33f;

    private GameObject m_picker;
    private Player _player;

    public override void Activate()
    {
        MapManager.Instance.Bases.Find((Base b) => b != _player.PlayerBase)?.TakeOfPourcentOfLifeTime(_damagePercent);

        RegisterManager.Instance.GetGameObjectInstance("ShieldSE")?.GetComponent<AudioSource>()?.Play();
        _player.PowerUpCooldown = true;
        MapManager.Instance.RemoveGameObjectOnTheGrid(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.z), MapManager.TypeObject.e_PowerUp);
    }

    public override void IsPick(Player player)
    {
        m_picker = player.gameObject;
        _player = player;
        Activate();
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponent<Player>();

        if (p != null && !p.PowerUpCooldown)
        {
            IsPick(p);
        }
    }
}
