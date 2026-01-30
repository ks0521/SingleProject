using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class UnsafeTest : MonoBehaviour
{
    int count = 1000_000_000;

    int[] array;
    // Start is called before the first frame update
    void Start()
    {
        array = new int[count];
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }
    public void Test()
    {
        Normal();
        UnSafe();
    }

    public void Normal()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }
        sw.Stop();
        UnityEngine.Debug.Log("일반 C#의 경우 걸리는 시간: " + sw.ElapsedMilliseconds);
    }
    public unsafe void UnSafe()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        fixed (int* arrayPtr = array)
        {

            for (int i = 0; i < count; i++)
            {
                arrayPtr[i] = i;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log("unsafe C#의 경우 걸리는 시간: " + sw.ElapsedMilliseconds);
    }
    unsafe void Func()
    {
        int num = 10;
        int* nPtr = &num;
        int* nPtrNext = nPtr + 1;
        int* allocPtr = (int*)UnsafeUtility.Malloc(sizeof(int), sizeof(int), Allocator.Persistent);
        *allocPtr = 50;
        //Debug.Log("GC와 무관한 heap에서 할당받은 값 : " + *allocPtr);
        UnsafeUtility.Free(allocPtr, Allocator.Persistent);
    }
}
