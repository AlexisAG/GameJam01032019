using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Powerup
{
    public float SpeedMultiplier = 4f;

    private BoxCollider m_boxCollider;
    private GameObject m_picker;

    public override void Activate()
    {
        m_picker.GetComponent<Player>()?.ApplySpeedEffect(SpeedMultiplier);
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
            m_picker = other.gameObject;
            m_picker.GetComponent<Player>().PowerUpCooldown = true;
            IsPick(other.gameObject.GetComponent<Player>());
            RegisterManager.Instance.GetGameObjectInstance("SpeedBoostSE")?.GetComponent<AudioSource>()?.Play();
            MapManager.Instance.RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), MapManager.TypeObject.e_Ressource);
        }
    }
}
