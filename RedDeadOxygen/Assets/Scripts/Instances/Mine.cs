using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public string m_PlayerTag;
    public GameObject m_ExplosionEffect;

    public void MakeExplosionEffect()
    {
        GameObject l_Explosion = Instantiate(m_ExplosionEffect, transform.position, Quaternion.identity );        
        ParticleSystem.EmissionModule em1 = l_Explosion.GetComponent<ParticleSystem>().emission;
        em1.enabled = true;
    }

}
