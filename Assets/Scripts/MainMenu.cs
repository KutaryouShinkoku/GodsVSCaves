using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string selectHeros = "Select_Single";
    [SerializeField] private string combat = "Combat";

    public void SelectHeros()
    {
        SceneManager.LoadScene(selectHeros);
    }

}
