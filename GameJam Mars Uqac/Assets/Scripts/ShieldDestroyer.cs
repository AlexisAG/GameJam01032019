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
        switch (m_picker.tag.Substring(m_picker.tag.Length, -1))
        {
            case "1":
                GameObject.FindWithTag("Base_2").GetComponent<Base>().TakeOfLifeTime(shieldDamage);
                break;

            case "2":
                GameObject.FindWithTag("Base_1").GetComponent<Base>().TakeOfLifeTime(shieldDamage);
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

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            m_picker = other.gameObject;
            IsPick();
        }
    }
}
