using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public GameObject BasePrefab;
    public GameObject MinePrefab;
    public GameObject RessourcePrefab;
    public int NbRessource = 10;

    private GameObject[] m_bases;
    private GameObject[] m_mines;
    private GameObject[] m_ressources;
    private GameObject[] m_powerups;
    private GameObject[,] m_grid;



    // Start is called before the first frame update
    void Start()
    {

        m_grid = new GameObject[(int)GetComponent<Renderer>().bounds.size.x, (int)GetComponent<Renderer>().bounds.size.z];

        InitRessources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Create all the ressources of the map
    void InitRessources()
    {

        float l_startCoordX = 4.5f * gameObject.transform.localScale.x;
        float l_startCoordZ = -4.5f * gameObject.transform.localScale.z;

        for (int i = 0; i < NbRessource; i++)
        {

            int x = 0;
            int z = 0;

            do
            {

               x = Random.Range(0, 10);
               z = Random.Range(0, 10);

            } while (m_grid[x,z] != null); 

            Debug.Log("x : " + x + " z : " + z);

            Instantiate<GameObject>(RessourcePrefab, new Vector3(l_startCoordX - x, 1f, l_startCoordZ + z), Quaternion.identity, gameObject.transform); // test

        }

    }

    void SpawnPowerups()
    {

    }

}
