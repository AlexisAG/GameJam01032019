using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventInstance : MonoBehaviour
{
    public MapEvent Parent { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        Base b = other.GetComponentInParent<Base>();
        Player p = other.GetComponent<Player>();

        if (b != null)
        {
            Parent.BaseCollision(b);
        }

        if (p != null)
        {
            Parent.PlayerCollision(p, gameObject);
        }
    }
}
