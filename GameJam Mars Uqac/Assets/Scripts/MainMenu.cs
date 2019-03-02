using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Canvas m_mainMenuCanvas;

    public Canvas m_instructionsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Map", LoadSceneMode.Additive);
    }

    public void GoToInstructions(bool value)
    {
        m_instructionsCanvas.enabled = value;
        //m_mainMenuCanvas.SetActive(value);
    }
}
