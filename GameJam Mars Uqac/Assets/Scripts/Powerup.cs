using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour, Pickup
{
    public abstract void Activate();
    public abstract void IsPick();
    public abstract void Respawn();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
