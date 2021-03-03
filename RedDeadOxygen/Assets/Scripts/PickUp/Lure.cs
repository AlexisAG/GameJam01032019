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

    public override void IsPick(Player player)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null && !other.gameObject.GetComponent<Player>().PowerUpCooldown)
        {
            IsPick(other.gameObject.GetComponent<Player>());
            other.gameObject.GetComponent<Player>().PowerUpCooldown = true;
            RegisterManager.Instance.GetGameObjectInstance("LureSE")?.GetComponent<AudioSource>()?.Play();
            MapManager.Instance.RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), MapManager.TypeObject.e_Ressource);
        }
    }
}
