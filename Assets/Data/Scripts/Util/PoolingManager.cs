using Item;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using static DalmoreSetting;

public static class PoolingManager
{
    public static List<ObjectPool<GameObject>> PoolingList;
    public static ObjectPool<GameObject> IcePool = CreatePool(1220020);
    public static ObjectPool<GameObject> bottlePool = CreatePool(9999999);

    public static ObjectPool<GameObject> SearchPool(uint id)
    {
        if (DB_Item.TryGetItemData(id, out ItemData data))
        {
            foreach (var i in PoolingList)
            {   
                var temp = i.Get();
                if (data.ID == temp.GetComponent<ItemDataComponent>().GetItemData.ID)
                {
                    i.Release(temp);
                    return i;
                }
                i.Release(temp);
            }
        }
        return null;
    }
    public static ObjectPool<GameObject> SearchPool(GameObject obj)
    {
        foreach (var i in PoolingList)
        {
            var temp = i.Get();
            if (obj == temp)
            {
                i.Release(temp);
                return i;
            }
            i.Release(temp);
        }
        return null;
    }
    static ObjectPool<GameObject> CreatePool(uint id)
    {
        if (DB_Item.TryGetItemData(id, out ItemData data))
        { 
            return new ObjectPool<GameObject>(() => GameObject.Instantiate(data.Prefab), (go) => go.SetActive(true), (go) => go.SetActive(false));
        }
        else
        {
            return new ObjectPool<GameObject>(() => null, (go) => go.SetActive(true), (go) => go.SetActive(false));
        }
    }
    static ObjectPool<GameObject> CreatePool(GameObject obj)
    {
        return new ObjectPool<GameObject>(() => GameObject.Instantiate(obj), (go) => go.SetActive(true), (go) => go.SetActive(false));
    }
    public static void CreatePoolInList(uint id)
    {
        if (PoolingList == null) PoolingList = new List<ObjectPool<GameObject>>();
        if (DB_Item.TryGetItemData(id, out ItemData data))
        {
            var temp = new ObjectPool<GameObject>(() => GameObject.Instantiate(data.Prefab), (go) => go.SetActive(true), (go) => go.SetActive(false));
            PoolingList.Add(temp);
        }
    }
}

