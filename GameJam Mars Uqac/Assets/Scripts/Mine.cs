using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public string m_PlayerTag;
    public GameObject m_ExplosionEffect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeExplosionEffect()
    {
        Debug.Log("Explosion effect");
        GameObject l_Explosion = Instantiate(m_ExplosionEffect, transform.position, Quaternion.identity );
        
        ParticleSystem.EmissionModule em1 = l_Explosion.GetComponent<ParticleSystem>().emission;
        em1.enabled = true;
    }

}
