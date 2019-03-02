using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum TypeObject
    {
        e_None,
        e_Mine,
        e_Ressource
    }
    public GameObject BasePrefab;
    public GameObject MinePrefab;
    public GameObject RessourcePrefab;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public int NbRessource = 10;
    public float StartEnemySpawnFrequency = 10; // Time in seconds between two spawns
    public float TimeOfFirstSpawnEnemy = 5; // Time in seconds when spawn the first enemy

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


        InitBase();
        InitRessources();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpawnEnemys();
    }

    void InitBase()
    {

        m_bases = new List<GameObject>(2);

        /* Instantiate Base 1 & Base 2 */

        // base 1
        m_bases.Add(Instantiate<GameObject>(BasePrefab, new Vector3(-3, 0, 3), Quaternion.identity, gameObject.transform));
        AddGameObjectOnTheGrid(3, 3, m_bases[0], TypeObject.e_None);

        //player 1
        m_player1 = Instantiate<GameObject>(PlayerPrefab, new Vector3(-3, 1, 3), Quaternion.identity, gameObject.transform);
        m_player1.GetComponent<Player>().m_joystickNumber = 0;

        //base 2
        m_bases.Add(Instantiate<GameObject>(BasePrefab, new Vector3(-(m_indexGridX - 3), 0, (m_indexGridZ - 3)), Quaternion.identity, gameObject.transform));
        AddGameObjectOnTheGrid((m_indexGridX - 3), (m_indexGridZ - 3), m_bases[1], TypeObject.e_None);

        //player 2
        m_player2 = Instantiate<GameObject>(PlayerPrefab, new Vector3(-(m_indexGridX - 3), 0, (m_indexGridZ - 3)), Quaternion.identity, gameObject.transform);
        m_player2.GetComponent<Player>().m_joystickNumber = 1;
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

    void SpawnPowerups()
    {

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
            RemoveGameObjectOnTheGrid(x, z, type);

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
