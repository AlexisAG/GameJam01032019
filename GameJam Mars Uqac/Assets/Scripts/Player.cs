using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string m_name;
    public double m_health;

    //ctor
    public Player(string p_name, double p_health)
    {
        this.m_name = p_name;
        this.m_health = p_health;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(this.m_name == "" || this.m_name == null) {
            this.m_name = "Default name";
            this.m_health = 42;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
