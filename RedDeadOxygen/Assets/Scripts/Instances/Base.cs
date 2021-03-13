using AgToolkit.Core.GameModes;
using AgToolkit.Core.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    [SerializeField]
    private Transform _sphere;
    [SerializeField]
    private Vector3 _baseScale; // Basic sphere scale without timeLife 
    [SerializeField]
    private float _maxLifeInSeconds = 20f;
    [SerializeField]
    private float _maxLife = 150f;
    [SerializeField]
    private int _maxScale = 5;

    private bool _lifeChanged = false;
    private int _baseIndex;
    private int m_PreviousRayon;
    private float m_ScaleFactorByLifeTime; 
    private float m_LifeTime; // Current life of the base decrease with time
    private float m_LoseLifeMultiplicator; // Scale apply to time to decrease life time
    private List<Vector2Int> _posInRangeOfDome; // All pos in range of dome
    private GameObject m_EventManager;
    private SoloGameMode _gm = null;

    public List<Player> Players { get; private set; } = new List<Player>();
    public int BaseIndex => _baseIndex;
    public Color Color { get; private set; }

    private void Start() 
    {
        _gm = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();
    }

    private void Update() 
    {
        if (_gm.GameIsOver) return;

        UpdateSphereSize(); // Update the scale of the sphere with remaining life time 
        CheckLifetime(); // Check if base is dead    
    }

    private IEnumerator DecreaseLifeOverTime()
    {
        float counter = 0f;
        float lifeOnStart = m_LifeTime;

        while (m_LifeTime > 0f && Time.timeScale != 0 && counter < 1f && !_lifeChanged)
        {
            counter += Time.deltaTime;
            m_LifeTime -= m_LifeTime - Mathf.Lerp(lifeOnStart, lifeOnStart - m_LoseLifeMultiplicator, counter / 1f);

            yield return null;            
        }

        if (m_LifeTime > 0f && !_gm.GameIsOver) 
        {
            _lifeChanged = false;
            CoroutineManager.Instance.StartCoroutine(DecreaseLifeOverTime());
        }
    }

    private void CreatePlayer(int index, GameObject prefab)
    {
        Player p = Instantiate<GameObject>(prefab, 
            new Vector3(transform.position.x, prefab.transform.localScale.y / 2, transform.position.z), 
            Quaternion.identity, 
            MapManager.Instance.gameObject.transform).GetComponent<Player>();

        p.transform.GetChild(0).GetComponent<Renderer>().material.color = Color;
        p.Init(index, this);
        Players.Add(p);
    }

    private void UpdateSphereSize()
    {
        float newScale = (_baseScale.x * m_LifeTime * m_ScaleFactorByLifeTime);
        newScale = Mathf.Clamp(newScale, 0, _maxScale);
        _sphere.localScale = new Vector3(newScale, newScale, _baseScale.z);

        if (m_PreviousRayon != Mathf.CeilToInt(_sphere.localScale.x))
        {
            m_PreviousRayon = Mathf.CeilToInt(_sphere.localScale.x);
            UpdatePosInRange();
        }
    }

    private void UpdatePosInRange()
    {
        List<Vector2Int> l_CurrentPosInRangeOfDome = new List<Vector2Int>();
        int index = Mathf.FloorToInt(m_PreviousRayon/2);

        for (int i=-index; i<= index; i++)
        {
            for (int j=-index; j<= index; j++)
            {
                Vector2Int l_TestPositionR = new Vector2Int(Mathf.FloorToInt(transform.localPosition.x + i*.5f), 
                    Mathf.FloorToInt(transform.localPosition.z + j*.5f));

                l_CurrentPosInRangeOfDome.Add(l_TestPositionR);
            }
            
        }

        // add new positions
        foreach(Vector2Int l_vec in l_CurrentPosInRangeOfDome)
        {
            if (!_posInRangeOfDome.Contains(l_vec))
            {
                _posInRangeOfDome.Add(l_vec);
                MapManager.Instance.AddGameObjectOnTheGrid(l_vec.x, l_vec.y, transform.gameObject, MapManager.TypeObject.e_Base);
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
                MapManager.Instance.RemoveGameObjectOnTheGrid(l_vec.x, l_vec.y);
            }
        }
    }

    private void CheckLifetime() 
    {
        if (m_LifeTime <= 0) 
        {
            FinishGame();
        }
    }

    public void Init(int baseIndex, GameObject playerPrefab, int nbPlayer = 1) 
    {
        _baseIndex = baseIndex;
        Color = _baseIndex == 0 ? Color.magenta : Color.cyan;

        for (int i = 0; i < nbPlayer; i++)
        {
            CreatePlayer(i + baseIndex, playerPrefab);
        }

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
        _lifeChanged = true;
    }

    public void TakeOfLifeTime(float p_Value = 10)
    {
        m_LifeTime -= p_Value;
        _lifeChanged = true;
    }

    public void TakeOfPourcentOfLifeTime(float p_Pourcent = .25f)
    {
        if (p_Pourcent > 1f) return;

        m_LifeTime -= p_Pourcent * m_LifeTime;
        _lifeChanged = true;
    }

    private void FinishGame()
    {
        if (_gm.GameIsOver) return;

        _gm.GameIsOver = true;
        _gm.LooserTeamIndex = BaseIndex;
        _gm?.GetComponent<Animator>().SetTrigger(_gm?.EndTrigger);
    }

    public float GetCurrentLife()
    {
        return (m_LifeTime/_maxLife)*100f;
    }
}
