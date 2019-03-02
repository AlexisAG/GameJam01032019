using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : Powerup
{
    private BoxCollider m_boxCollider;

    public override void Activate()
    {
        Debug.Log("Leurre");
    }

    public override void IsPick()
    {
        Activate();
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            IsPick();
        }
    }
}
