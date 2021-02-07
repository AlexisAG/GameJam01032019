using AgToolkit.Core.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    [SerializeField]
    private Vector3 _baseScale; // Basic sphere scale without timeLife 
    [SerializeField]
    private float _maxLifeInSeconds = 20f;
    [SerializeField]
    private float _maxLife = 150f;
    [SerializeField]
    private int _maxScale = 5;

    private int m_PreviousRayon;
    private float m_ScaleFactorByLifeTime; 
    private float m_LifeTime; // Current life of the base decrease with time
    private float m_LoseLifeMultiplicator; // Scale apply to time to decrease life time
    private List<Vector2Int> _posInRangeOfDome; // All pos in range of dome
    private GameObject m_EventManager;

    private void Update() 
    {
        UpdateSphereSize(); // Update the scale of the sphere with remaining life time 
        CheckLifetime(); // Check if base is dead    
    }

    private IEnumerator DecreaseLifeOverTime()
    {
        float counter = 0f;
        float lifeOnStart = m_LifeTime;

        while (m_LifeTime > 0f && Time.timeScale != 0 && counter < 1f && m_LifeTime <= lifeOnStart)
        {
            counter += Time.deltaTime;
            TakeOfLifeTime(m_LifeTime - Mathf.Lerp(lifeOnStart, lifeOnStart - m_LoseLifeMultiplicator, counter / 1f));

            yield return null;            
        }

        if (m_LifeTime > 0f && Time.timeScale != 0) 
        {
            CoroutineManager.Instance.StartCoroutine(DecreaseLifeOverTime());
        }
    }

    private void UpdateSphereSize()
    {
        float newScale = (_baseScale.x * m_LifeTime * m_ScaleFactorByLifeTime);
        newScale = Mathf.Clamp(newScale, 0, _maxScale);
        transform.localScale = new Vector3(newScale, newScale, _baseScale.z);

        if (m_PreviousRayon != Mathf.CeilToInt(transform.localScale.x))
        {
            m_PreviousRayon = Mathf.CeilToInt(transform.localScale.x);
            UpdatePosInRange();
        }
    }

    private void UpdatePosInRange()
    {
        List<Vector2Int> l_CurrentPosInRangeOfDome = new List<Vector2Int>();

        for (int i=-Mathf.FloorToInt(m_PreviousRayon/2); i<= Mathf.FloorToInt(m_PreviousRayon/2); i++)
        {
            for (int j=-Mathf.FloorToInt(m_PreviousRayon/2); j<= Mathf.FloorToInt(m_PreviousRayon/2); j++)
            {
                Vector2Int l_TestPositionR = new Vector2Int(Mathf.FloorToInt(transform.parent.localPosition.x + i*.5f), 
                    Mathf.FloorToInt(transform.parent.localPosition.z + j*.5f));

                l_CurrentPosInRangeOfDome.Add(l_TestPositionR);
            }
            
        }

        // add new positions
        foreach(Vector2Int l_vec in l_CurrentPosInRangeOfDome)
        {
            if (!_posInRangeOfDome.Contains(l_vec))
            {
                _posInRangeOfDome.Add(l_vec);
                MapManager.Instance.AddGameObjectOnTheGrid(l_vec.x, l_vec.y, transform.parent.gameObject, MapManager.TypeObject.e_Base);
            }
        }

        // remove old positions
        List<Vector2Int> temp = new List<Vector2Int>();

        for (int i = _posInRangeOfDome.Count - 1; i >= 0; i--)
        {
            Vector2Int l_vec = _posInRangeOfDome[i];

            if (!l_CurrentPosInRangeOfDome.Contains(l_vec))
            {
                _posInRangeOfDome.RemoveAt(i);
                MapManager.Instance.RemoveGameObjectOnTheGrid(l_vec.x, l_vec.y, MapManager.TypeObject.e_Base);
            }
        }
    }

    private void CheckLifetime() 
    {
        if (m_LifeTime <= 0) 
        {
            Debug.Log("Fin de Game");
            FinishGame();
        }
    }

    public void Init() 
    {
        m_PreviousRayon = -1;
        m_LifeTime = _maxLife;
        m_LoseLifeMultiplicator = _maxLife / _maxLifeInSeconds;
        m_ScaleFactorByLifeTime = 1f / (_maxLife / _maxScale);
        _posInRangeOfDome = new List<Vector2Int>();
        UpdateSphereSize();
        UpdatePosInRange();
        CoroutineManager.Instance.StartCoroutine(DecreaseLifeOverTime());
    }

    public void AddRessourceToBase(float amount)
    {
        AddLifeTime(amount);
    }

    public void AddLifeTime(float p_Value = 10)
    {
        m_LifeTime += p_Value;
        m_LifeTime = Mathf.Clamp(m_LifeTime, 0, _maxLife);
    }

    public void TakeOfLifeTime(float p_Value = 10)
    {
        m_LifeTime -= p_Value;
    }

    public void TakeOfPourcentOfLifeTime(float p_Pourcent = .25f)
    {
        if (p_Pourcent > 1f) return;

        m_LifeTime -= p_Pourcent * m_LifeTime;
    }

    private void FinishGame()
    {

        //TODO GAMEMODE CREATE A GAMESTATE FOR ENDMENU


        /*if(!GetEventManager().GetComponent<EndGameMenu>().m_IsGameFinish)
        {
            GetEventManager().GetComponent<EndGameMenu>().m_IsGameFinish = true;
            string l_WinnerName = "Le vainqueur est : \n";
            Color col = new Color();

            switch (m_PlayerTag)
            {
                case "Player 0":
                    l_WinnerName += "Player 2";
                    col = Color.cyan;
                    break;
                case "Player 1":
                    l_WinnerName += "Player 1";
                    col = Color.magenta;
                    break;
            }


            GameObject.Find("EndScreen").GetComponentInChildren<Text>().text = l_WinnerName;
            GameObject.Find("EndScreen").GetComponentInChildren<Text>().color = col;
            GameObject.Find("EndScreen").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("EndScreen").transform.GetChild(2).gameObject.SetActive(true);
            GameObject.Find("EndScreen").transform.GetChild(2).gameObject.GetComponent<Button>().Select();*/
            Time.timeScale = 0;
    }

    public float GetCurrentLife()
    {
        return (m_LifeTime/_maxLife)*100f;
    }
}
