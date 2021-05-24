using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using AgToolkit.Core.Manager;

public class O2_Display : MonoBehaviour
{
    private int o2_P1;
    private int o2_P2;
    private string mine_P1, mine_P2; 
    public Text o2Text_P1, o2Text_P2, mineP1, mineP2, baseP1, baseP2;
    private Player[] m_players;

    private void Update()
    {
        if (GameManager.Instance.GetCurrentGameMode<SoloGameMode>()?.GameIsOver ?? true) return;

        //"catch" all spawned players
        m_players = MapManager.Instance.GetComponentsInChildren<Player>();

        if (m_players == null || m_players.Length <= 0) return;

        //link the resources number of each player to respective variable
        o2_P1 = m_players.Where<Player>(p => p.tag == "Player 0").ToArray<Player>()[0].GetNbOfRessources();
        o2_P2 = m_players.Where<Player>(p => p.tag == "Player 1").ToArray<Player>()[0].GetNbOfRessources();

        mine_P1 = m_players.Where<Player>(p => p.tag == "Player 0").ToArray<Player>()[0].HaveMine() ? "Mine disponible" : "Mine indisponible";
        mine_P2 = m_players.Where<Player>(p => p.tag == "Player 1").ToArray<Player>()[0].HaveMine() ? "Mine disponible" : "Mine indisponible";

        Base b1 = MapManager.Instance.Bases[0];
        Base b2 = MapManager.Instance.Bases[1];

        int l_Base1Life = 0, l_Base2Life = 0;

        if (b1)
            l_Base1Life = Mathf.FloorToInt(b1.GetCurrentLife());
        if(b2)
            l_Base2Life = Mathf.FloorToInt(b2.GetCurrentLife());

        //Update UI values
        baseP1.text = $"Vie restante : {l_Base1Life}%";
        baseP2.text = $"Vie restante : {l_Base2Life}%";
        o2Text_P1.text = $"Oxygen {o2_P1}/5";
        o2Text_P2.text = $"Oxygen {o2_P2}/5";
        mineP1.text = mine_P1;
        mineP2.text = mine_P2;
    }
}
