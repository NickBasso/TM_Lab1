using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private string myString = "ch1234567890";
    
    // Start is called before the first frame update
    void Start()
    {
    
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myString = myString.Substring(2, myString.Length-2);
            print(myString);
        }
    }
}
