using System.Collections.Generic;
using UnityEngine;

public class BrickPool
{
    private readonly GameObject[] brickPrefabs;
    private readonly List<GameObject> pool;
    private readonly Dictionary<GameObject, GameObject> sourceMap;

    public BrickPool(GameObject[] prefabs, int initialSize)
    {
        brickPrefabs = prefabs;
        pool = new List<GameObject>(initialSize);
        sourceMap = new Dictionary<GameObject, GameObject>();


        for (int i = 0; i < initialSize; i++)
        {
            var prefab = brickPrefabs[Random.Range(0, brickPrefabs.Length)];
            var go = Object.Instantiate(prefab);
            go.SetActive(false);
            pool.Add(go);
            sourceMap[go] = prefab;
        }
    }

   
    public GameObject GetBrick(GameObject prefab, Vector3 position)
    {
       
        pool.RemoveAll(b => b == null);

       
        foreach (var brick in pool)
        {
            if (!brick.activeInHierarchy && sourceMap.TryGetValue(brick, out var src) && src == prefab)
            {
                brick.transform.position = position;
                brick.SetActive(true);
                return brick;
            }
        }

        var inst = Object.Instantiate(prefab, position, Quaternion.identity);
        pool.Add(inst);
        sourceMap[inst] = prefab;
        return inst;
    }

    
    public GameObject GetBrick(Vector3 position)
    {
        var prefab = brickPrefabs[Random.Range(0, brickPrefabs.Length)];
        return GetBrick(prefab, position);
    }

    public void ReturnAllBricks()
    {
        foreach (var brick in pool)
            if (brick != null)
                brick.SetActive(false);
    }
}
