using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.AgToolkit.Core.Timer;
using AgToolkit.Core.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MapManager : Singleton<MapManager>
{
    [Header("Prefabs")]
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
    private List<GameObject> _powerUpPrefabs = new List<GameObject>();

    [Header("Positions")]
    [SerializeField]
    private List<Vector3> _basesPosition = new List<Vector3>(2);
    [SerializeField]
    private List<Vector3> _powerUpPosition = new List<Vector3>(2);

    [Header("Timers")]
    [SerializeField]
    private float _powerUpSpawnTime = 10f;
    [SerializeField]
    private float _powerUpRemoveTime = 5f;

    [Header("Divers")]
    [SerializeField]
    private int _maxRessource = 10;

    private GameObjectGrid[,] _grid = new GameObjectGrid[0,0];
    private Timer _powerUpSpawnTimer;
    private Timer _powerUpDeleteTimer;
    private int m_nbPlayers;
    private float m_nextTimeEnemySpawn;
    private float m_enemySpawnFrequency;

    public float StartEnemySpawnFrequency = 10; // Time in seconds between two spawns
    public float TimeOfFirstSpawnEnemy = 5; // Time in seconds when spawn the first enemy
    public List<Base> Bases { get; private set; } = new List<Base>();

    public Vector2Int GridSize { get; private set; } = Vector2Int.zero;

    public enum TypeObject
    {
        e_None,
        e_Base,
        e_Mine,
        e_Ressource,
        e_PowerUp
    }
    private class GameObjectGrid
    {
        public GameObject GameObjectRef { get; private set; }
        public TypeObject TypeRef { get; private set; }
        public Vector2Int PositionOnGrid { get; private set; }

        public GameObjectGrid(GameObject go, TypeObject type, int x, int y)
        {
            GameObjectRef = go;
            TypeRef = type;
            PositionOnGrid = new Vector2Int(x, y);
        }
    }


    // Update is called once per frame
    void Update()
    {
        return;

        CheckSpawnEnemys();
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
    
    private void SpawnPowerups()
    {
        bool leureIsFirstPos = (Random.value > 0.5f);
        int powerUpIndex = Random.Range(0, _powerUpPrefabs.Count);
        int index = leureIsFirstPos ? 0 : 1;
        
        //lure
        GameObject lure = GameObject.Instantiate(_lurePrefab, transform, false);
        lure.transform.rotation = Quaternion.identity;
        lure.transform.localPosition = _powerUpPosition[index];
        AddGameObjectOnTheGrid((int)_powerUpPosition[index].x, (int)_powerUpPosition[index].z, lure, TypeObject.e_PowerUp);

        //powerUp
        GameObject powerUp = GameObject.Instantiate(_powerUpPrefabs[powerUpIndex], transform, false);
        powerUp.transform.rotation = Quaternion.identity;
        powerUp.transform.localPosition = _powerUpPosition[index == 0 ? 1 : 0];
        AddGameObjectOnTheGrid((int)powerUp.transform.localPosition.x, (int)powerUp.transform.localPosition.z, powerUp, TypeObject.e_PowerUp);

        TimerManager.Instance.StartTimer(_powerUpDeleteTimer);
    }

    private void DeletePowerups()
    {
        RemoveGameObjectOnTheGrid((int)_powerUpPosition[0].x, (int)_powerUpPosition[0].z, TypeObject.e_PowerUp);
        RemoveGameObjectOnTheGrid((int)_powerUpPosition[1].x, (int)_powerUpPosition[1].z, TypeObject.e_PowerUp);
        TimerManager.Instance.StartTimer(_powerUpSpawnTimer);
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
            Debug.LogWarning(_grid[x, y].GameObjectRef.name);

            switch (_grid[x, y].TypeRef) {
                case TypeObject.e_Ressource:
                    RemoveGameObjectOnTheGrid(x, y, TypeObject.e_Ressource);
                    break;

                case TypeObject.e_Mine:
                    RemoveGameObjectOnTheGrid(x, y, TypeObject.e_Mine);
                    break;

                default:

                    break;
            }
        }

        _grid[x, y] = new GameObjectGrid(obj, type, x, y);
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
        _grid = new GameObjectGrid[GridSize.x, GridSize.y];
        GridSize -= new Vector2Int(1, 1);

        m_enemySpawnFrequency = Mathf.Clamp(StartEnemySpawnFrequency, 1, 300);
        m_nextTimeEnemySpawn = Time.fixedTime + TimeOfFirstSpawnEnemy;

        // Register timers
        UnityEvent spawnPowerUpEvent = new UnityEvent();
        UnityEvent deletePowerUpEvent = new UnityEvent();
        spawnPowerUpEvent.AddListener(SpawnPowerups);
        deletePowerUpEvent.AddListener(DeletePowerups);

        _powerUpSpawnTimer = new Timer("powerUpSpawn", _powerUpSpawnTime, spawnPowerUpEvent);
        _powerUpDeleteTimer = new Timer("powerUpDelete", _powerUpRemoveTime, deletePowerUpEvent);

        TimerManager.Instance.Register(_powerUpSpawnTimer);
        TimerManager.Instance.Register(_powerUpDeleteTimer);

        InitBase();
        InitRessources();

        // Start timer (not _powerUpDeleteTimer)
        TimerManager.Instance.StartTimer(_powerUpSpawnTimer);
    }

    public void RemoveGameObjectOnTheGrid(int x, int z, TypeObject type)
    {
        ConvertUnityPositionToCordinate(ref x, ref z);

        if (_grid.Length <= 0 || _grid[x, z] == null) return;

        switch (_grid[x, z].TypeRef)
        {
            case TypeObject.e_Base:
                return;
            case TypeObject.e_Ressource:
                _grid[x, z].GameObjectRef.GetComponent<Ressource>()?.Respawn();
                break;
            case TypeObject.e_None:
            case TypeObject.e_Mine:
            case TypeObject.e_PowerUp:
            default:
                _grid[x, z].GameObjectRef.SetActive(false);
                break;
        }


        _grid[x, z] = null;
    }

    public void ClearGrid() 
    {
        if (_grid.Length <= 0) return;

        // Destroy players (not referenced in the grid)
        foreach (Base item in Bases) 
        {
            foreach (Player p in item.Players)
            {
                p.DropResources();
                GameObject.Destroy(p.gameObject);
            }
        }

        // clean the grid
        for (int i = 0; i <= GridSize.x; i++) 
        {
            for (int j = 0; j <= GridSize.y; j++) 
            {
                if (_grid[i, j] == null) continue;

                if (_grid[i, j].TypeRef == TypeObject.e_Ressource)
                {
                    _grid[i, j].GameObjectRef.SetActive(false);
                }
                else
                {
                    Destroy(_grid[i, j].GameObjectRef);
                }

                _grid[i, j] = null;
            }
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
