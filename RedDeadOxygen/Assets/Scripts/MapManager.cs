using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.Core.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MapManager : Singleton<MapManager>
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
    private List<Vector3> _basesPosition = new List<Vector3>(2);
    [SerializeField]
    private int _maxRessource = 10;

    private List<GameObject> m_mines = new List<GameObject>();
    private List<GameObject> m_ressources = new List<GameObject>();
    private List<GameObject> m_powerups = new List<GameObject>();
    private GameObject[,] _grid = new GameObject[0,0];
    private int m_nbPlayers;
    private float m_nextTimeEnemySpawn;
    private float m_powerupSpawnTimer;
    private float m_powerupDeleteTimer;
    private float m_enemySpawnFrequency;

    public float StartEnemySpawnFrequency = 10; // Time in seconds between two spawns
    public float TimeOfFirstSpawnEnemy = 5; // Time in seconds when spawn the first enemy
    public float PowerUpSpawnFrequency = 10; //Time in seconds between powerup spawns
    public float PowerUpDeletionTime = 5; //Time to get the powerups before they disappear
    public List<Base> Bases { get; private set; } = new List<Base>();

    public Vector2Int GridSize { get; private set; } = Vector2Int.zero;

    //todo USELESS ??
    public enum TypeObject
    {
        e_None,
        e_Base,
        e_Mine,
        e_Ressource,
        e_PowerUp
    }

    // Update is called once per frame
    void Update()
    {
        return;

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
        GameObject ressource = PoolManager.Instance.GetPooledObject(_ressourcePrefab.name);
        ressource.SetActive(true);
        ressource.GetComponent<Ressource>()?.Respawn();
    }

    private void InitBase()
    {
        // base 1
        Base base1 = Instantiate<GameObject>(_basePrefab, transform).GetComponentInChildren<Base>();
        base1.transform.localPosition = _basesPosition[0];
        AddGameObjectOnTheGrid((int)_basesPosition[0].x, (int)_basesPosition[0].z, base1.gameObject, TypeObject.e_Base);
        base1.Init(0, _playerPrefab);

        //base 2
        Base base2 = Instantiate<GameObject>(_basePrefab, transform).GetComponentInChildren<Base>();
        base2.transform.localPosition = _basesPosition[1];
        AddGameObjectOnTheGrid((int)_basesPosition[1].x, (int)_basesPosition[1].z, base2.gameObject, TypeObject.e_Base);
        base2.Init(1, _playerPrefab);
        
        Bases.Add(base1);
        Bases.Add(base2);
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
            AddGameObjectOnTheGrid((int)-l_leftSpawnPosition.x, GridSize.y / 2, lurePowerUp, TypeObject.e_PowerUp);
            l_remainingPosition = l_rightSpawnPosition;
            l_remainingIndex = (int)-l_remainingPosition.x;
        }
        else
        {
            GameObject lurePowerUp = Instantiate<GameObject>(_lurePrefab, l_rightSpawnPosition, Quaternion.identity, transform);
            AddGameObjectOnTheGrid((int)-l_rightSpawnPosition.x, GridSize.y / 2, lurePowerUp, TypeObject.e_PowerUp);
            l_remainingPosition = l_leftSpawnPosition;
            l_remainingIndex = (int)-l_remainingPosition.x;
        }

        int l_PowerUpType = Random.Range(0, 2);

        switch (l_PowerUpType)
        {
            case 0:
                GameObject slowPowerUp = Instantiate<GameObject>(_slowPowerPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, GridSize.y / 2, slowPowerUp, TypeObject.e_PowerUp);
                break;

            case 1:
                GameObject speedBoostPowerUp = Instantiate<GameObject>(_boostPowerPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, GridSize.y / 2, speedBoostPowerUp, TypeObject.e_PowerUp);
                break;

            case 2:
                GameObject shieldBreakPowerUp = Instantiate<GameObject>(_shieldBreakPowerPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, GridSize.y / 2, shieldBreakPowerUp, TypeObject.e_PowerUp);
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
            RemoveGameObjectOnTheGrid(11, GridSize.x / 2, TypeObject.e_PowerUp);
            RemoveGameObjectOnTheGrid(9, GridSize.y / 2, TypeObject.e_PowerUp);

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

    #region CoordinationConverter
    private void ConvertUnityPositionToCordinate(ref int x, ref int y) 
    {
        //adjust Unity Local Position with grid position
        if (x < 0) {
            x = -x;
        }
        else if (x > 0) {
            x *= 2;
        }

        if (y < 0) {
            y = -y;
        }
        else if (y > 0) {
            y *= 2;
        }
    }

    private void ConvertUnityPositionToCordinate(ref Vector3 v) 
    {
        //adjust Unity Local Position with grid position
        if (v.x < 0) {
            v.x = -v.x;
        }
        else if (v.x > 0) {
            v.x *= 2;
        }

        if (v.z < 0) {
            v.z = -v.z;
        }
        else if (v.z > 0) {
            v.z *= 2;
        }
    }

    private void ConvertCoordinateToUnityPosition(ref int x, ref int y) 
    {
        //adjust coordination with Unity Local Position
        if (x < GridSize.x / 2) {
            x = -x;
        }
        else if (x > GridSize.x / 2) {
            x /= 2;
        }

        if (y < GridSize.y / 2) {
            y = -y;
        }
        else if (y > GridSize.y / 2) {
            y /= 2;
        }
    }

    private void ConvertCoordinateToUnityPosition(ref Vector3 v) 
    {
        //adjust coordination with Unity Local Position
        if (v.x < GridSize.x / 2) {
            v.x = -v.x;
        }
        else if (v.x > GridSize.x / 2) {
            v.x /= 2;
        }

        if (v.z < GridSize.y / 2) {
            v.z = -v.z;
        }
        else if (v.z > GridSize.y / 2) {
            v.z /= 2;
        }
    }
    #endregion

    // Public method for add an object into the grid
    public bool AddGameObjectOnTheGrid(int x, int y, GameObject obj, TypeObject type, bool replace = true)
    {
        ConvertUnityPositionToCordinate(ref x, ref y);

        if (_grid.Length <= 0) return false;
        else if (x > GridSize.x || x < 0) return false;
        else if (y > GridSize.y || y < 0) return false;
        else if (_grid[x, y] != null && !replace) return false;
        else if (_grid[x, y] != null && replace) 
        {
            Debug.LogWarning(_grid[x, y].name);

            switch (_grid[x, y].name) {
                case "Ressource(Clone)":
                    RemoveGameObjectOnTheGrid(x, y, TypeObject.e_Ressource);
                    break;

                case "Mine(Clone)":
                    RemoveGameObjectOnTheGrid(x, y, TypeObject.e_Mine);
                    break;

                default:

                    break;
            }
        }

        _grid[x, y] = obj;
        return true;
    }

    public IEnumerator InitMap() 
    {
        // Init pool
        if (!PoolManager.Instance.PoolExists(_ressourcePrefab.name)) 
        {
            yield return PoolManager.Instance.CreatePool(new PoolData(_ressourcePrefab.name, _ressourcePrefab, _maxRessource));
        }

        // Init grid size from plane renderer
        Vector3 planeSize = GetComponent<Renderer>()?.bounds.size / 2 ?? Vector3.zero;
        GridSize = new Vector2Int((int)planeSize.x, (int)planeSize.z);
        _grid = new GameObject[GridSize.x, GridSize.y];
        GridSize -= new Vector2Int(1, 1);

        m_enemySpawnFrequency = Mathf.Clamp(StartEnemySpawnFrequency, 1, 300);
        m_nextTimeEnemySpawn = Time.fixedTime + TimeOfFirstSpawnEnemy;

        m_powerupSpawnTimer = PowerUpSpawnFrequency;
        m_powerupDeleteTimer = PowerUpDeletionTime;

        InitBase();
        InitRessources();
    }

    public void RemoveGameObjectOnTheGrid(int x, int z, TypeObject type)
    {
        ConvertUnityPositionToCordinate(ref x, ref z);

        if (_grid.Length <= 0 || _grid[x, z] == null) return;
        if(_grid[x, z].GetComponentInChildren<Base>() != null) return;

        _grid[x, z].SetActive(false);
        _grid[x, z] = null;
    }

    public void ClearGrid() 
    {
        if (_grid.Length <= 0) return;

        for (int i = 0; i <= GridSize.x; i++) 
        {
            for (int j = 0; j <= GridSize.y; j++) 
            {
                if (_grid[i, j] == null) continue;

                _grid[i, j].SetActive(false);
                _grid[i, j] = null;
            }
        }

        foreach (Base item in Bases) 
        {
            foreach (Player p in item.Players)
            {
                p.DropResources();
                GameObject.Destroy(p.gameObject);
            }

            GameObject.Destroy(item.gameObject);
        }

        Bases.Clear();
    }
    
    //todo: probably a better way for that
    public Vector2Int GetRandomFreePosition()
    {
        int x = 0;
        int y = 0;

        // random position on the grid
        do
        {
            x = Random.Range(0, GridSize.x);
            y = Random.Range(0, GridSize.y);

        } while (_grid[x, y] != null);

        ConvertCoordinateToUnityPosition(ref x, ref y);

        return new Vector2Int(x,y);
    }

}
