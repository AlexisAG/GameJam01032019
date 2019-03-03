using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class O2_Display : MonoBehaviour
{
    private int o2_P1;
    private int o2_P2;

    public Text o2Text_P1;
    public Text o2Text_P2;
    private Player[] m_players;

    private void Awake() {
        

        //"catch" the Text component of each UI element who will be updated
        o2Text_P1 = GameObject.Find("CountBase_P1")?.GetComponent<Text>();
        o2Text_P2 = GameObject.Find("CountBase_P2")?.GetComponent<Text>();

        Debug.Log(o2Text_P2.text);
    }

    private void Update() {
        
        //"catch" all spawned players
        m_players = GameObject.Find("Map_Plane")?.GetComponentsInChildren<Player>();

        //link the resources number of each player to respective variable
        o2_P1 = m_players.Where<Player>(p => p.tag == "Player 0").ToArray<Player>()[0].GetNbOfRessources();

        o2_P2 = m_players.Where<Player>(p => p.tag == "Player 1").ToArray<Player>()[0].GetNbOfRessources();

        //Update UI values
        o2Text_P1.text = o2_P1.ToString() + "/5";
        o2Text_P2.text = o2_P2.ToString() + "/5";
    }
}
