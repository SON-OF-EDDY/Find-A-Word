using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopUpManager : MonoBehaviour
{

    //public Button closeButton;
    public GameObject popUpPanel;
    public GameObject screenBlocker;
    

    // Start is called before the first frame update
    public void closePopUp ()
    {

        Debug.Log("Close button pressed");
        //popUpPanel.SetActive(false);
        //screenBlocker.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
