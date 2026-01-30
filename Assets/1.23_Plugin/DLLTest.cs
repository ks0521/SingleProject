using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DLLTest : MonoBehaviour
{
    [DllImport("CustomPlugin")]
    static extern int AddNumbers(int a, int b);
    void Start()
    {
        int result = AddNumbers(10, 30);
        Debug.Log(result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
