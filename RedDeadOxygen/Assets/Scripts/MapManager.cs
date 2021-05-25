using AgToolkit.Core.DesignPattern;
using AgToolkit.Core.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using AgToolkit.Core.DataSystem;

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
    private GameObject _mapEventPrefab;
    [SerializeField]
    private List<GameObject> _powerUpPrefabs = new List<GameObject>();

    [Header("Positions")]
    [SerializeField]
    private List<Vector3> _basesPosition = new List<Vector3>(2);
    [SerializeField]
    private List<Vector3> _enemySpawnsPosition = new List<Vector3>(2);
    [SerializeField]
    private List<Vector3> _powerUpPosition = new List<Vector3>(2);

    [Header("Timers")]
    [SerializeField]
    private float _enemySpawnTime = 10f;
    [SerializeField]
    private float _powerUpSpawnTime = 10f;
    [SerializeField]
    private float _powerUpRemoveTime = 5f;
    [SerializeField]
    private float _mapEventTime = 20f;

    [Header("Properties")]
    [SerializeField]
    private MapProperties _mapProperty = MapProperties.e_Basic;
    [SerializeField]
    private float _wallElasticity = .5f;

    [Header("Divers")]
    [SerializeField]
    private int _maxRessource = 10;
    [SerializeField]
    private string _assetBundleMapEvent;

    private int _nbPlayers;
    private MapEventData _lastMapEventData;
    private Timer _enemySpawnTimer;
    private Timer _powerUpSpawnTimer;
    private Timer _powerUpDeleteTimer;
    private Timer _mapEventTimer;
    private SoloGameMode _gameMode;
    private GameObjectGrid[,] _grid = new GameObjectGrid[0,0];
    private List<MapEventData> _mapEventDatas = new List<MapEventData>();

    public List<Base> Bases { get; private set; } = new List<Base>();
    public Vector2Int GridSize { get; private set; } = Vector2Int.zero;

    public MapProperties MapProperty => _mapProperty;
    public float WallElasticity => _wallElasticity;

    public enum TypeObject
    {
        e_None,
        e_Base,
        e_Mine,
        e_Ressource,
        e_PowerUp
    }

    public enum MapProperties
    {
        e_Basic,
        e_Ice
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

        if (ressource == null) return;

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
        _nbPlayers = 2;
    }

    #region TimerCallback
    private void SpawnPowerups()
    {
        if (_gameMode.GameIsOver)
        {
            return;
        }

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
        if (_gameMode.GameIsOver)
        {
            return;
        }

        RemoveGameObjectOnTheGrid((int)_powerUpPosition[0].x, (int)_powerUpPosition[0].z);
        RemoveGameObjectOnTheGrid((int)_powerUpPosition[1].x, (int)_powerUpPosition[1].z);
        TimerManager.Instance.StartTimer(_powerUpSpawnTimer);
    }

    private void SpawnEnemy()
    {
        if (_gameMode.GameIsOver)
        {
            return;
        }

        for (int i = 0; i < _nbPlayers; i++)
        {
            GameObject enemy = PoolManager.Instance.GetPooledObject(_enemyPrefab.name);
            enemy.transform.SetParent(transform);
            enemy.transform.localPosition = _enemySpawnsPosition[i];
            enemy.transform.localScale = new Vector3(_enemyPrefab.transform.localScale.x /transform.localScale.x, 
                                                    _enemyPrefab.transform.localScale.y / transform.localScale.y, 
                                                    _enemyPrefab.transform.localScale.z / transform.localScale.z);

            enemy.GetComponent<Enemy>()?.Init(Bases[i % 2]);
            enemy.SetActive(true);
        }

        TimerManager.Instance.StartTimer(_enemySpawnTimer);
    }

    private void ActivateMapEvent()
    {
        if (PartyManager.Instance.IsClassic || _gameMode.GameIsOver) return;

        List<MapEventData> filter = _lastMapEventData != null ? _mapEventDatas.Where(data => data.Type != _lastMapEventData.Type).ToList() : _mapEventDatas;
        _lastMapEventData = filter[Random.Range(0, filter.Count)];
        MapEvent temp = GameObject.Instantiate(_mapEventPrefab, transform).GetComponent<MapEvent>();
        temp.Init(_lastMapEventData);
        TimerManager.Instance.StartTimer(_mapEventTimer);
    }
    #endregion

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

    public IEnumerator InitMap() 
    {
        // Get GameMode
        _gameMode = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();

        // Init pool
        if (!PoolManager.Instance.PoolExists(_ressourcePrefab.name)) 
        {
            yield return PoolManager.Instance.CreatePool(new PoolData(_ressourcePrefab.name, _ressourcePrefab, _maxRessource));
        }
        if (!PoolManager.Instance.PoolExists(_enemyPrefab.name)) 
        {
            yield return PoolManager.Instance.CreatePool(new PoolData(_enemyPrefab.name, _enemyPrefab, 5, true));
        }

        // Load AssetBundle TODO: Use BundleManager
        if (!PartyManager.Instance.IsClassic && _mapEventDatas.Count <= 0)
        {
            DataSystem.UnloadAssetBundle(_assetBundleMapEvent);
            yield return DataSystem.LoadLocalBundleAsync<MapEventData>(_assetBundleMapEvent, data => _mapEventDatas = data);
        }

        ClearGrid();
        yield return null;

        // Init grid size from plane renderer
        Vector3 planeSize = GetComponent<Renderer>()?.bounds.size / 2 ?? Vector3.zero;
        GridSize = new Vector2Int((int)planeSize.x, (int)planeSize.z);
        _grid = new GameObjectGrid[GridSize.x, GridSize.y];
        GridSize -= new Vector2Int(1, 1);

        // Register timers
        UnityEvent spawnEnemyEvent = new UnityEvent();
        UnityEvent spawnPowerUpEvent = new UnityEvent();
        UnityEvent deletePowerUpEvent = new UnityEvent();
        UnityEvent mapEventTimerEvent = new UnityEvent();
        spawnEnemyEvent.AddListener(SpawnEnemy);
        spawnPowerUpEvent.AddListener(SpawnPowerups);
        deletePowerUpEvent.AddListener(DeletePowerups);
        mapEventTimerEvent.AddListener(ActivateMapEvent);

        _enemySpawnTimer = new Timer("enemySpawn", _enemySpawnTime, spawnEnemyEvent);
        _powerUpSpawnTimer = new Timer("powerUpSpawn", _powerUpSpawnTime, spawnPowerUpEvent);
        _powerUpDeleteTimer = new Timer("powerUpDelete", _powerUpRemoveTime, deletePowerUpEvent);
        _mapEventTimer = new Timer("mapEvent", _mapEventTime, mapEventTimerEvent);

        // Start timer (not _powerUpDeleteTimer)
        TimerManager.Instance.StartTimer(_enemySpawnTimer);
        TimerManager.Instance.StartTimer(_powerUpSpawnTimer);
        TimerManager.Instance.StartTimer(_mapEventTimer);
            
        // Init bases & resources
        InitBase();
        InitRessources();
    }

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

            if (_grid[x, y].TypeRef != TypeObject.e_Base && _grid[x, y].TypeRef != TypeObject.e_PowerUp)
            {
                RemoveGameObjectOnTheGrid(_grid[x, y]);
            }

        }

        _grid[x, y] = new GameObjectGrid(obj, type, x, y);
        return true;
    }

    private void RemoveGameObjectOnTheGrid(GameObjectGrid go)
    {
        switch (go.TypeRef)
        {
            case TypeObject.e_Base:
                break;
            case TypeObject.e_Ressource:

                Ressource r = go.GameObjectRef.GetComponent<Ressource>();

                if (r != null && !r.IsUsed)
                {
                    r.Respawn();
                }

                break;

            case TypeObject.e_None:
            case TypeObject.e_Mine:
            case TypeObject.e_PowerUp:
            default:
                GameObject.Destroy(go.GameObjectRef);
                break;
        }

        _grid[go.PositionOnGrid.x, go.PositionOnGrid.y] = null;
    }

    public void RemoveGameObjectOnTheGrid(int x, int z)
    {
        ConvertUnityPositionToCordinate(ref x, ref z);

        if (_grid.Length <= 0 || x > GridSize.x  || z > GridSize.y|| _grid[x, z] == null) return;

        switch (_grid[x, z].TypeRef)
        {
            case TypeObject.e_Base:
                break;
            case TypeObject.e_Ressource:

                Ressource r = _grid[x, z].GameObjectRef.GetComponent<Ressource>();

                if (r != null && !r.IsUsed)
                {
                    r.Respawn();
                }

                break;
            case TypeObject.e_None:
            case TypeObject.e_Mine:
            case TypeObject.e_PowerUp:
            default:
                GameObject.Destroy(_grid[x, z].GameObjectRef);
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

        TimerManager.Instance.RemoveTimer(_enemySpawnTimer);
        TimerManager.Instance.RemoveTimer(_powerUpDeleteTimer);
        TimerManager.Instance.RemoveTimer(_powerUpSpawnTimer);
        TimerManager.Instance.RemoveTimer(_mapEventTimer);
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
