using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public GameObject BasePrefab;
    public GameObject MinePrefab;
    public GameObject RessourcePrefab;
    public GameObject PlayerPrefab;
    public int NbRessource = 10;

    private List<GameObject> m_bases = new List<GameObject>();
    private List<GameObject> m_mines = new List<GameObject>();
    private List<GameObject> m_ressources = new List<GameObject>();
    private List<GameObject> m_powerups = new List<GameObject>();
    private GameObject m_player1, m_player2; 
    private GameObject[,] m_grid;
    private int m_indexGridX, m_indexGridZ;

    // Start is called before the first frame update
    void Start()
    {
        m_grid = new GameObject[(int)GetComponent<Renderer>().bounds.size.x, (int)GetComponent<Renderer>().bounds.size.z]; // init the capacity with scale of plane

        m_indexGridX = m_grid.Length / (int)GetComponent<Renderer>().bounds.size.x;
        m_indexGridZ = m_grid.Length / (int)GetComponent<Renderer>().bounds.size.z;
        
        InitBase();
        InitRessources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitBase()
    {

        m_bases = new List<GameObject>(2);

        /* Instantiate Base 1 & Base 2 */

        // base 1
        m_bases.Add(Instantiate<GameObject>(BasePrefab, new Vector3(-3, 0, 3), Quaternion.identity, gameObject.transform));
        AddGameObjectOnTheGrid(3, 3, m_bases[0]);

        //player 1
        m_player1 = Instantiate<GameObject>(PlayerPrefab, new Vector3(-3, 1, 3), Quaternion.identity, gameObject.transform);
        m_player1.GetComponent<PlayerController>().m_joystickNumber = 0;

        //base 2
        m_bases.Add(Instantiate<GameObject>(BasePrefab, new Vector3(-(m_indexGridX - 3), 0, (m_indexGridZ - 3)), Quaternion.identity, gameObject.transform));
        AddGameObjectOnTheGrid((m_indexGridX - 3), (m_indexGridZ - 3), m_bases[1]);

        //player 2
        m_player2 = Instantiate<GameObject>(PlayerPrefab, new Vector3(-(m_indexGridX - 3), 0, (m_indexGridZ - 3)), Quaternion.identity, gameObject.transform);
        m_player2.GetComponent<PlayerController>().m_joystickNumber = 1;


    }

    // Create all the ressources of the map
    void InitRessources()
    {

        m_ressources = new List<GameObject>(NbRessource); // init the capacity


        for (int i = 0; i < NbRessource; i++)
        {

            int x = 0;
            int z = 0;

            // random position on the grid
            do
            {

               x = Random.Range(0, m_indexGridX);
               z = Random.Range(0, m_indexGridZ);

            } while (m_grid[x,z] != null);

            // add the ressource to the List m_ressources
            GameObject ressource = Instantiate<GameObject>(RessourcePrefab, new Vector3(-x, 0f, z), Quaternion.identity, gameObject.transform);
            m_ressources.Add(ressource);
            AddGameObjectOnTheGrid(x, z, ressource);
        }

    }

    void SpawnPowerups()
    {

    }

    // Public method for add an object into the grid
    public void AddGameObjectOnTheGrid(int x, int z, GameObject obj)
    {

        if (m_grid[x, z] != null)
            RemoveGameObjectOnTheGrid(x, z);

        m_grid[x, z] = obj;

    }

    // public method for destroy an object from the grid
    public void RemoveGameObjectOnTheGrid(int x, int z)
    {
        Destroy(m_grid[x, z]);
    }

    public int[] GetGridSize()
    {
        return new int[2] { m_indexGridX, m_indexGridZ };
    }

}
