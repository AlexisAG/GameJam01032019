using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class O2_Display : MonoBehaviour
{
    private int o2_P1;
    private int o2_P2;
    private string mine_P1, mine_P2; 
    public Text o2Text_P1, o2Text_P2, mineP1, mineP2, baseP1, baseP2;
    private Player[] m_players;

    private void Awake() {
        
    }

    private void Update() {
        
        //"catch" all spawned players
        m_players = GameObject.Find("Map_Plane")?.GetComponentsInChildren<Player>();

        //link the resources number of each player to respective variable
        o2_P1 = m_players.Where<Player>(p => p.tag == "Player 0").ToArray<Player>()[0].GetNbOfRessources();
        o2_P2 = m_players.Where<Player>(p => p.tag == "Player 1").ToArray<Player>()[0].GetNbOfRessources();
        mine_P1 = m_players.Where<Player>(p => p.tag == "Player 0").ToArray<Player>()[0].HaveMine() ? "Mine disponible" : "Mine indisponible";
        mine_P2 = m_players.Where<Player>(p => p.tag == "Player 1").ToArray<Player>()[0].HaveMine() ? "Mine disponible" : "Mine indisponible";

        Base b1 = m_players.Where<Player>(p => p.tag == "Player 0").ToArray<Player>()[0].m_PlayerBase;
        Base b2 = m_players.Where<Player>(p => p.tag == "Player 1").ToArray<Player>()[0].m_PlayerBase;

        int l_Base1Life = 0, l_Base2Life = 0;

        if (b1)
            l_Base1Life = (int)Mathf.Clamp(b1.GetCurrentLife(),0,1000000);
        if(b2)
            l_Base2Life = (int)Mathf.Clamp(b2.GetCurrentLife(), 0, 1000000);


        baseP1.text = "Vie restante : " + l_Base1Life.ToString()+" %";
        baseP2.text = "Vie restante : " + l_Base2Life.ToString()+" %";

        //Update UI values
        o2Text_P1.text = "Oxygen " + o2_P1.ToString() + "/5";
        o2Text_P2.text = "Oxygen " + o2_P2.ToString() + "/5";


        mineP1.text = mine_P1;
        mineP2.text = mine_P2;
    }
}
