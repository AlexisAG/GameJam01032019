using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDestroyer : Powerup
{
    private BoxCollider m_boxCollider;
    private GameObject m_picker;
    public float shieldDamage = 15.0f;

    public override void Activate()
    {
        switch (m_picker.tag.Substring(m_picker.tag.Length - 1, 1))
        {
            case "0":
                GameObject.FindWithTag("Player 1").GetComponent<Player>()?.m_PlayerBase.TakeOfLifeTime(7);
                break;

            case "1":
                GameObject.FindWithTag("Player 0").GetComponent<Player>()?.m_PlayerBase.TakeOfLifeTime(7);
                break;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null && !other.gameObject.GetComponent<Player>().m_powerUpCooldown)
        {
            m_picker = other.gameObject;
            m_picker.GetComponent<Player>().m_powerUpCooldown = true;
            IsPick();
            RegisterManager.Instance.GetGameObjectInstance("ShieldSE")?.GetComponent<AudioSource>()?.Play();
            MapManager.Instance.RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), MapManager.TypeObject.e_Ressource);
        }
    }
}
