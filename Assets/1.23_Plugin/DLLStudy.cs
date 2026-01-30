using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class DLLStudy : MonoBehaviour
{
    public int[] arr;
    public int[] testArr;
    public int[] sorted;
    // Start is called before the first frame update
    [DllImport("CustomPlugin 1")]
    static extern void Sort(int[] arr, int size);
        
    void Start()
    {
        Stopwatch sw = new Stopwatch();

        testArr = new int[15];
        MakeArr(testArr, 50);
        Sort(testArr, testArr.Length);
        UnityEngine.Debug.Log($"C++ Merge Test : \n{string.Join(" ", testArr)}");

        MakeArr(testArr, 50);
        SortArr(testArr, testArr.Length);
        UnityEngine.Debug.Log($"C# Merge Test : \n{string.Join(" ", testArr)}");


        arr = new int[10_000_000];
        MakeArr(arr, 10_000_000);
        UnityEngine.Debug.Log("Array Size : 10,000,000 \n" +
            "Value Range : -10,000,000 ~ 10,000,000");
        sw.Start();
        Sort(arr, arr.Length);
        sw.Stop();
        UnityEngine.Debug.Log($"C++ Merge Sort : {sw.ElapsedMilliseconds}(ms)");

        MakeArr(arr, 10000000);
        sw.Start();
        SortArr(arr, arr.Length);
        sw.Stop();
        UnityEngine.Debug.Log($"C# Merge Sort : {sw.ElapsedMilliseconds}(ms)");
    }
    void MakeArr(int[] arr, int range)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = UnityEngine.Random.Range(-range, range);
        }
    }
    void SortArr(int [] arr, int size)
    {

        sorted = new int[size];

        DivideArr(arr, 0, size - 1); //마지막 인덱스는 배열크기-1
    }

    void DivideArr(int [] arr, int left, int right)
    {

        if (left < right)
        {
            int mid = (left + right) / 2;
            DivideArr(arr, left, mid);
            DivideArr(arr, mid + 1, right);

            Merge(arr, left, mid, right);
        }
    }

    void Merge(int [] arr, int left, int mid, int right)
    {
        int i = left, j = mid + 1, count = left;
        while (i <= mid && j <= right)
        {
            if (arr[i] <= arr[j])
            {
                sorted[count++] = arr[i++];
            }
            else
            {
                sorted[count++] = arr[j++];
            }
        }
        while (i <= mid)
        {
            sorted[count++] = arr[i++];
        }
        while (j <= right)
        {
            sorted[count++] = arr[j++];
        }

        for (int k = left; k <= right; k++)
        {
            //sorted배열에 저장한 정렬결과를 원본 arr에 재복사
            arr[k] = sorted[k];
        }
    }
}
