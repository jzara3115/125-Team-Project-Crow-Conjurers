using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    

    public void OnBackButton()
    {
        SceneManager.LoadScene(0);
    }
}
