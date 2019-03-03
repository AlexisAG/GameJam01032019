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
        Instantiate<GameObject>(m_ExplosionEffect, transform);
    }

    void OnDestroy()
    {

    }

}
