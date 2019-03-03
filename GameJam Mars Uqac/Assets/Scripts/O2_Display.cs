using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class O2_Display : MonoBehaviour
{
    private float o2_P1;
    private float o2_P2;

    private Text o2Text_P1;
    private Text o2Text_P2;
    private Player[] m_players;

    private void Start() {
        m_players = GameObject.Find("Map_Plane").GetComponentsInChildren<Player>();
        //TODO: getResources // o2_P1 = m_players.Where<Player>(p => p.tag == "Player 0").getResources();
        //TODO: getResources // o2_P2 = m_players.Where<Player>(p => p.tag == "Player 1").getResources();
        o2Text_P1 = GameObject.Find("CountBase_P1")?.GetComponent<Text>();
        o2Text_P2 = GameObject.Find("CountBase_P2")?.GetComponent<Text>();
    }

    private void Update() {
        //Update UI values
        o2Text_P1.text = o2_P1.ToString();
        o2Text_P2.text = o2_P2.ToString();
    }
}
