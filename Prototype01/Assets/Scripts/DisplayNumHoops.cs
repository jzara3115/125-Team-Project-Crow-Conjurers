using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplayNumHoops : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hoopNum;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        hoopNum.text = "x " + GameManager.Instance.NumHoops.ToString();
    }
}
