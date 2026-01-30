using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Result <T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error {  get; }
    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }
    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }
}

public struct Item
{
    public string Name { get; private set; }
    public int Id {  get; private set; }
    public Item(string name, int id)
    {
        Name = name;
        Id = id;
    }
}
public class Test : MonoBehaviour
{
    List<Item> inventory;
    Result<Item> FindAt(List<Item> inventory, int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            return Result<Item>.Failure("인벤토리 접근 인덱스가 유효하지 않습니다");
        }
        if (inventory == null)
        {
            return Result<Item>.Failure("인벤토리가 없습니다. ");
        }
        return Result<Item>.Success(inventory[index]);
    }
    Result<int> Multi(int a, int b)
    {
        if(b == 0)
        {
            return Result<int>.Success(0);
        }
        if(a > int.MaxValue / b)
        {
            return Result<int>.Failure("오버플로 발생");
        }
        return Result<int>.Success(a * b);
    }
    Result<int> Divide(int a, int b)
    {
        if (b == 0)
        {
            return Result<int>.Failure("0으로 나눌 수 없습니다");
        }
        return Result<int>.Success(a / b);
    }
    void Start()
    {
        /*inventory = new List<Item>();
        inventory.Add(new Item("뼈",3));
        inventory.Add(new Item("나무", 2));
        inventory.Add(new Item("오브", 5));
        //inventory.Add(new Item("골동품", 4));

        //인벤토리에서 아이템을 찾는 기능을 Result패턴으로 예외값 처리
        var result = FindAt(inventory, 3);
        if (result.IsSuccess)
        {
            Debug.Log(result.Value.Name);
        }
        else
        {
            Debug.LogWarning(result.Error);
        }
        */
        var result = Multi(12311531, 15648965);
        if (result.IsSuccess)
        {
            Debug.Log(result.Value);
        }
        else
        {
            Debug.LogError(result.Error);
        }
    }
}

