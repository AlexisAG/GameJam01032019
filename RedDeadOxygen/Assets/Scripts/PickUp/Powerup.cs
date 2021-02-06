using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour, Pickup
{
    public abstract void Activate();
    public abstract void IsPick(Player playerRef);
    public abstract void Respawn();
}
