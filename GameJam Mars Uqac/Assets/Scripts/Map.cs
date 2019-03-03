using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum TypeObject
    {
        e_None,
        e_Mine,
        e_Ressource,
        e_PowerUp
    }
    public GameObject BasePrefab;
    public GameObject MinePrefab;
    public GameObject RessourcePrefab;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject LurePrefab;
    public GameObject SlowPrefab;
    public GameObject SpeedBoostPrefab;
    public GameObject ShieldBreakPrefab;
    public int NbRessource = 10;
    public float StartEnemySpawnFrequency = 10; // Time in seconds between two spawns
    public float TimeOfFirstSpawnEnemy = 5; // Time in seconds when spawn the first enemy
    public float PowerUpSpawnFrequency = 10; //Time in seconds between powerup spawns
    public float PowerUpDeletionTime = 5; //Time to get the powerups before they disappear

    private float m_powerupSpawnTimer;
    private float m_powerupDeleteTimer;
    private List<GameObject> m_bases = new List<GameObject>();
    private List<GameObject> m_mines = new List<GameObject>();
    private List<GameObject> m_ressources = new List<GameObject>();
    private List<GameObject> m_powerups = new List<GameObject>();
    private GameObject m_player1, m_player2;
    private GameObject[,] m_grid;
    private int m_indexGridX, m_indexGridZ;
    private float m_nextTimeEnemySpawn;
    private int m_nbPlayers;
    private float m_enemySpawnFrequency;

    // Start is called before the first frame update
    void Start()
    {
        m_grid = new GameObject[(int)GetComponent<Renderer>().bounds.size.x, (int)GetComponent<Renderer>().bounds.size.z]; // init the capacity with scale of plane

        m_indexGridX = m_grid.Length / (int)GetComponent<Renderer>().bounds.size.x;
        m_indexGridZ = m_grid.Length / (int)GetComponent<Renderer>().bounds.size.z;

        m_nextTimeEnemySpawn = 0;
        m_nbPlayers = 2;
        m_enemySpawnFrequency = Mathf.Clamp(StartEnemySpawnFrequency,1,300);

        m_powerupSpawnTimer = PowerUpSpawnFrequency;
        m_powerupDeleteTimer = PowerUpDeletionTime;

        InitBase();
        InitRessources();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpawnEnemys();

        if (m_ressources.Count < NbRessource)
            SpawnARessource();

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

    void InitBase()
    {

        m_bases = new List<GameObject>(2);

        /* Instantiate Base 1 & Base 2 */

        // base 1
        GameObject l_base1 = Instantiate<GameObject>(BasePrefab, new Vector3(-3, 0, 3), Quaternion.identity, gameObject.transform);
        //l_base1.tag = "Player 0";
        m_bases.Add(l_base1);
        AddGameObjectOnTheGrid(3, 3, m_bases[0], TypeObject.e_None);
        
        
        

        //player 1
        m_player1 = Instantiate<GameObject>(PlayerPrefab, new Vector3(-3, PlayerPrefab.transform.localScale.y / 2, 3), Quaternion.identity, gameObject.transform);
        m_player1.GetComponent<Player>().m_joystickNumber = 0;
        m_player1.tag = "Player 0";

        //base 2
        m_bases.Add(Instantiate<GameObject>(BasePrefab, new Vector3(-(m_indexGridX - 3), 0, (m_indexGridZ - 3)), Quaternion.identity, gameObject.transform));
        AddGameObjectOnTheGrid((m_indexGridX - 3), (m_indexGridZ - 3), m_bases[1], TypeObject.e_None);
        //m_bases[1].tag = "Player 1";

        //player 2
        m_player2 = Instantiate<GameObject>(PlayerPrefab, new Vector3(-(m_indexGridX - 3), PlayerPrefab.transform.localScale.y / 2, (m_indexGridZ - 3)), Quaternion.identity, gameObject.transform);
        m_player2.GetComponent<Player>().m_joystickNumber = 1;
        m_player2.tag = "Player 1";

    }

    // Create all the ressources of the map
    void InitRessources()
    {

        m_ressources = new List<GameObject>(NbRessource); // init the capacity


        for (int i = 0; i < NbRessource; i++)
        {

            Vector2Int pos = GetRandomFreePosition();

            // add the ressource to the List m_ressources
            GameObject ressource = Instantiate<GameObject>(RessourcePrefab, new Vector3(-pos.x, 0f, pos.y), Quaternion.identity, gameObject.transform);
            AddGameObjectOnTheGrid(pos.x, pos.y, ressource, TypeObject.e_Ressource);
        }

    }

    void SpawnARessource()
    {
        Vector2Int pos = GetRandomFreePosition();

        // add the ressource to the List m_ressources
        GameObject ressource = Instantiate<GameObject>(RessourcePrefab, new Vector3(-pos.x, 0f, pos.y), Quaternion.identity, gameObject.transform);
        AddGameObjectOnTheGrid(pos.x, pos.y, ressource, TypeObject.e_Ressource);
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
            GameObject lurePowerUp = Instantiate<GameObject>(LurePrefab, l_leftSpawnPosition,Quaternion.identity, transform);
            AddGameObjectOnTheGrid((int)-l_leftSpawnPosition.x, m_indexGridZ / 2, lurePowerUp, TypeObject.e_PowerUp);
            l_remainingPosition = l_rightSpawnPosition;
            l_remainingIndex = (int)-l_remainingPosition.x;
        }
        else
        {
            GameObject lurePowerUp = Instantiate<GameObject>(LurePrefab, l_rightSpawnPosition, Quaternion.identity, transform);
            AddGameObjectOnTheGrid((int)-l_rightSpawnPosition.x, m_indexGridZ / 2, lurePowerUp, TypeObject.e_PowerUp);
            l_remainingPosition = l_leftSpawnPosition;
            l_remainingIndex = (int)-l_remainingPosition.x;
        }

        //int l_PowerUpType = Random.Range(0, 2);
        int l_PowerUpType = 2;
        switch (l_PowerUpType)
        {
            case 0:
                GameObject slowPowerUp = Instantiate<GameObject>(SlowPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, m_indexGridZ / 2, slowPowerUp, TypeObject.e_PowerUp);
                break;

            case 1:
                GameObject speedBoostPowerUp = Instantiate<GameObject>(SpeedBoostPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, m_indexGridZ / 2, speedBoostPowerUp, TypeObject.e_PowerUp);
                break;

            case 2:
                GameObject shieldBreakPowerUp = Instantiate<GameObject>(ShieldBreakPrefab, l_remainingPosition, Quaternion.identity, transform);
                AddGameObjectOnTheGrid(l_remainingIndex, m_indexGridZ / 2, shieldBreakPowerUp, TypeObject.e_PowerUp);
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
            RemoveGameObjectOnTheGrid(11, m_indexGridZ / 2, TypeObject.e_PowerUp);
            RemoveGameObjectOnTheGrid(9, m_indexGridZ / 2, TypeObject.e_PowerUp);

            m_powerupDeleteTimer = 2*PowerUpSpawnFrequency;
            m_powerupSpawnTimer = PowerUpSpawnFrequency;
        }
        
    }

    private void CheckSpawnEnemys()
    {
        if (m_nextTimeEnemySpawn == 0 && Time.fixedTime >= TimeOfFirstSpawnEnemy)
        {
            SpawnEnemy();
            m_nextTimeEnemySpawn = Time.fixedTime + m_enemySpawnFrequency;
            m_enemySpawnFrequency = Mathf.Clamp(m_enemySpawnFrequency*0.8f, 1, 300);
        } else if(m_nextTimeEnemySpawn != 0 && Time.fixedTime >= m_nextTimeEnemySpawn)
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
                GameObject FirstPlayerEnemy = (GameObject)Instantiate(EnemyPrefab, new Vector3(-3, 0, 17), Quaternion.identity);
                FirstPlayerEnemy.transform.Rotate(new Vector3(180, 0, 180));

                GameObject SecondPlayerEnemy = (GameObject)Instantiate(EnemyPrefab, new Vector3(-17, 0, 3), Quaternion.identity);
                break;
        }
    }

    // Public method for add an object into the grid
    public void AddGameObjectOnTheGrid(int x, int z, GameObject obj, TypeObject type)
    {

        if (m_grid[x, z] != null)
        {
            Debug.Log(m_grid[x, z].name);

            switch (m_grid[x, z].name)
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
        m_grid[x, z] = obj;

        switch (type)
        {
            case TypeObject.e_Mine:
                m_mines.Add(obj);
                break;
            case TypeObject.e_Ressource:
                m_ressources.Add(obj);
                break;
            default:
                break;
        }


    }

    // public method for destroy an object from the grid
    public void RemoveGameObjectOnTheGrid(int x, int z, TypeObject type)
    {

        switch (type)
        {
            case TypeObject.e_Mine:
                m_mines.Remove(m_grid[x, z]);
                break;
            case TypeObject.e_Ressource:
                m_ressources.Remove(m_grid[x, z]);
                break;
            default:
                break;
        }

        Destroy(m_grid[x, z]);
    }

    public int[] GetGridSize()
    {
        return new int[2] { m_indexGridX, m_indexGridZ };
    }

    public Vector2Int GetRandomFreePosition()
    {
        int x = 0;
        int z = 0;

        // random position on the grid
        do
        {

            x = Random.Range(0, m_indexGridX);
            z = Random.Range(0, m_indexGridZ);

        } while (m_grid[x, z] != null);

        return new Vector2Int(x,z);
    }

}
