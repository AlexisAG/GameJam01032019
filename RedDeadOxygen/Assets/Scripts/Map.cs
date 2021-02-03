using AgToolkit.Core.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Map : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("BasePrefab")]
    private GameObject _basePrefab;
    [SerializeField]
    [FormerlySerializedAs("MinePrefab")]
    private GameObject _minePrefab;
    [SerializeField]
    [FormerlySerializedAs("RessourcePrefab")]
    private GameObject _ressourcePrefab;
    [SerializeField]
    [FormerlySerializedAs("PlayerPrefab")]
    private GameObject _playerPrefab;
    [SerializeField]
    [FormerlySerializedAs("EnemyPrefab")]
    private GameObject _enemyPrefab;
    [SerializeField]
    [FormerlySerializedAs("LurePrefab")]
    private GameObject _lurePrefab;
    [SerializeField]
    [FormerlySerializedAs("SlowPrefab")]
    private GameObject _slowPowerPrefab;
    [SerializeField]
    [FormerlySerializedAs("SpeedBoostPrefab")]
    private GameObject _boostPowerPrefab;
    [SerializeField]
    [FormerlySerializedAs("ShieldBreakPrefab")]
    private GameObject _shieldBreakPowerPrefab;
    [SerializeField]
    private int _maxRessource = 10;
    public float StartEnemySpawnFrequency = 10; // Time in seconds between two spawns
    public float TimeOfFirstSpawnEnemy = 5; // Time in seconds when spawn the first enemy
    public float PowerUpSpawnFrequency = 10; //Time in seconds between powerup spawns
    public float PowerUpDeletionTime = 5; //Time to get the powerups before they disappear


    private List<GameObject> m_bases = new List<GameObject>();
    private List<GameObject> m_mines = new List<GameObject>();
    private List<GameObject> m_ressources = new List<GameObject>();
    private List<GameObject> m_powerups = new List<GameObject>();
    private GameObject m_player1, m_player2;
    private GameObject[,] _grid;
    private Vector2Int _gridSize = Vector2Int.zero;
    private int m_nbPlayers;
    private float m_nextTimeEnemySpawn;
    private float m_powerupSpawnTimer;
    private float m_powerupDeleteTimer;
    private float m_enemySpawnFrequency;

    public enum TypeObject
    {
        e_Base,
        e_Mine,
        e_Ressource,
        e_PowerUp
    }

    // Use Start instead of Awake for IEnumerator
    private IEnumerator Start()
    {
        // Init pool
        yield return PoolManager.Instance.CreatePool(new PoolData(_ressourcePrefab.name, _ressourcePrefab, _maxRessource));

        // Init grid size from plane renderer
        Vector3 planeSize = GetComponent<Renderer>()?.bounds.size ?? Vector3.zero;
        _gridSize = new Vector2Int((int)planeSize.x, (int)planeSize.z);
        _grid = new GameObject[_gridSize.x, _gridSize.y];
    
        m_enemySpawnFrequency = Mathf.Clamp(StartEnemySpawnFrequency,1,300);
        m_nextTimeEnemySpawn = Time.fixedTime + TimeOfFirstSpawnEnemy;

        m_powerupSpawnTimer = PowerUpSpawnFrequency;
        m_powerupDeleteTimer = PowerUpDeletionTime;

        InitBase();
        InitRessources();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpawnEnemys();

        //powerUps Timer
        m_powerupSpawnTimer -= Time.deltaTime;
        if (m_powerupSpawnTimer <= 0)
        {
            m_powerupSpawnTimer = 2*PowerUpSpawnFrequency;
            SpawnPowerups();
            m_powerupDeleteTimer = PowerUpDeletionTime;
        }
        CheckIfDeletePowerups();
    }


    // Create all the ressources of the map
    private void InitRessources() 
    {
        for (int i = 0; i < _maxRessource; i++) 
        {
            SpawnARessource();
        }
    }

    private void SpawnARessource() 
    {
        Vector2Int pos = GetRandomFreePosition();
        GameObject ressource = PoolManager.Instance.GetPooledObject(_ressourcePrefab.name);
        ressource.transform.SetParent(transform);
        ressource.transform.localPosition = new Vector3(-pos.x, 0f, pos.y);
        ressource.transform.rotation = Quaternion.identity;
        AddGameObjectOnTheGrid(pos.x, pos.y, ressource, TypeObject.e_Ressource);
    }


    void InitBase()
    {

        m_bases = new List<GameObject>(2);

        /* Instantiate Base 1 & Base 2 */

        // base 1
        GameObject l_base1 = Instantiate<GameObject>(_basePrefab, new Vector3(-3, 0, 3), Quaternion.identity, gameObject.transform);
        //l_base1.tag = "Player 0";
        m_bases.Add(l_base1);
        AddGameObjectOnTheGrid(3, 3, m_bases[0], TypeObject.e_Base);
        
        //player 1
        m_player1 = Instantiate<GameObject>(_playerPrefab, new Vector3(-3, _playerPrefab.transform.localScale.y / 2, 3), Quaternion.identity, gameObject.transform);
        m_player1.GetComponent<Player>().m_joystickNumber = 0;
        m_player1.tag = "Player 0";
        m_player1.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.magenta;

        //base 2
        m_bases.Add(Instantiate<GameObject>(_basePrefab, new Vector3(-(_gridSize.x - 3), 0, (_gridSize.y - 3)), Quaternion.identity, gameObject.transform));
        AddGameObjectOnTheGrid((_gridSize.x - 3), (_gridSize.y - 3), m_bases[1], TypeObject.e_Base);
        //m_bases[1].tag = "Player 1";

        //player 2
        m_player2 = Instantiate<GameObject>(_playerPrefab, new Vector3(-(_gridSize.x - 3), _playerPrefab.transform.localScale.y / 2, (_gridSize.y - 3)), Quaternion.identity, gameObject.transform);
        m_player2.GetComponent<Player>().m_joystickNumber = 1;
        m_player2.tag = "Player 1";
        m_player2.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);

    }

    void SpawnPowerups()
    {
        Vector3 l_leftSpawnPosition = new Vector3(-11, 0, 10);
        Vector3 l_rightSpawnPosition = new Vector3(-9, 0, 10);
        Vector3 l_remainingPosition;
        int l_remainingIndex;

        bool l_lurePos = (Random.value > 0.5f);
        if (l_lurePos)
        {
            GameObject lurePowerUp = Instantiate<GameObject>(_lurePrefab, l_leftSpawnPosition,Quaternion.identity, transform);
            AddGameObjectOnTheGrid((int)-l_leftSpawnPosition.x, _gridSize.y / 2, lurePowerUp, TypeObject.e_PowerUp);
            l_remainingPosition = l_rightSpawnPosition;
            l_remainingIndex = (int)-l_remainingPosition.x;
        }
        else
        {
            GameObject lurePowerUp = Instantiate<GameObject>(_lurePrefab, l_rightSpawnPosition, Quaternion.identity, transform);
            AddGameObjectOnTheGrid((int)-l_rightSpawnPosition.x, _gridSize.y / 2, lurePowerUp, TypeObject.e_PowerUp);
            l_remainingPosition = l_leftSpawnPosition;
            l_remainingIndex = (int)-l_remainingPosition.x;
        }

        int l_PowerUpType = Random.Range(0, 2);

        switch (l_PowerUpType)
        {
            case 0:
                GameObject slowPowerUp = Instantiate<GameObject>(_slowPowerPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, _gridSize.y / 2, slowPowerUp, TypeObject.e_PowerUp);
                break;

            case 1:
                GameObject speedBoostPowerUp = Instantiate<GameObject>(_boostPowerPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, _gridSize.y / 2, speedBoostPowerUp, TypeObject.e_PowerUp);
                break;

            case 2:
                GameObject shieldBreakPowerUp = Instantiate<GameObject>(_shieldBreakPowerPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, _gridSize.y / 2, shieldBreakPowerUp, TypeObject.e_PowerUp);
                break;

            default:
                break;
        }

    }

    private void CheckIfDeletePowerups()
    {
        m_powerupDeleteTimer -= Time.deltaTime;
        if (m_powerupDeleteTimer <= 0)
        {
            RemoveGameObjectOnTheGrid(11, _gridSize.x / 2, TypeObject.e_PowerUp);
            RemoveGameObjectOnTheGrid(9, _gridSize.y / 2, TypeObject.e_PowerUp);

            m_powerupDeleteTimer = 2*PowerUpSpawnFrequency;
            m_powerupSpawnTimer = PowerUpSpawnFrequency;
        }
        
    }

    private void CheckSpawnEnemys()
    {
        if(Time.fixedTime >= m_nextTimeEnemySpawn)
        {
            SpawnEnemy();
            if(m_enemySpawnFrequency > 1)
            {
                m_enemySpawnFrequency = Mathf.Clamp(m_enemySpawnFrequency * .9f, 1, 300);
            }
            m_nextTimeEnemySpawn = Time.fixedTime + m_enemySpawnFrequency;
        }
    }

    private void SpawnEnemy()
    {
        switch(m_nbPlayers)
        {
            case 2:
                GameObject FirstPlayerEnemy = (GameObject)Instantiate(_enemyPrefab, new Vector3(-3, 0, 17), Quaternion.identity);
                FirstPlayerEnemy.transform.Rotate(new Vector3(180, 0, 180));

                GameObject SecondPlayerEnemy = (GameObject)Instantiate(_enemyPrefab, new Vector3(-17, 0, 3), Quaternion.identity);
                break;
        }
    }

    // Public method for add an object into the grid
    public void AddGameObjectOnTheGrid(int x, int z, GameObject obj, TypeObject type, bool replace = true)
    {
        if (_grid[x, z] != null && !replace) return;
        else if (_grid[x, z] != null && replace)
        {
            Debug.LogWarning(_grid[x, z].name);

            switch (_grid[x, z].name)
            {
                case "Ressource(Clone)":
                    RemoveGameObjectOnTheGrid(x, z, TypeObject.e_Ressource);
                    break;

                case "Mine(Clone)":
                    RemoveGameObjectOnTheGrid(x, z, TypeObject.e_Mine);
                    break;

                default:

                    break;
            }
        }

        _grid[x, z] = obj;
    }

    public void RemoveGameObjectOnTheGrid(int x, int z, TypeObject type)
    {
        _grid[x, z].SetActive(false); // send pooled object to his pool
        _grid[x, z] = null;
    }

    //todo: DAFUQ?????
    public int[] GetGridSize()
    {
        return new int[2] { _gridSize.x, _gridSize.y }; 
    }

    //todo: probably a better way for that
    public Vector2Int GetRandomFreePosition()
    {
        int x = 0;
        int y = 0;

        // random position on the grid
        do
        {
            x = Random.Range(0, _gridSize.x);
            y = Random.Range(0, _gridSize.y);

        } while (_grid[x, y] != null);

        return new Vector2Int(x,y);
    }

}
