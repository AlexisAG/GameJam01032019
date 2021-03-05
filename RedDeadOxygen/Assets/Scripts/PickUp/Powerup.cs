using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour, Pickup
{
    protected Player _player;

    public abstract void Activate();
    public abstract void Respawn();
    public void IsPick(Player playerRef)
    {
        _player = playerRef;
        Activate();
    }

    protected void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponent<Player>();

        if (p != null && !p.PowerUpCooldown)
        {
            IsPick(p);
        }
    }
}
