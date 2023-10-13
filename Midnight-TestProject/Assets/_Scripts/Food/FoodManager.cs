using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class FoodManager : MonoBehaviour
{
    [SerializeField] private GameObject _foodPrefab;
    [SerializeField] private int _numFood;
    [SerializeField] private int _tankSize;

    public static GameObject[] allFood;

    public int NumFood
    {
        get { return _numFood; }
    }

    private void Start()
    {
        allFood = new GameObject[NumFood];

        for (int i = 0; i < NumFood; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-_tankSize, _tankSize),
                Random.Range(-_tankSize, _tankSize),
                Random.Range(-_tankSize, _tankSize));
            allFood[i] = Instantiate(_foodPrefab, pos, Quaternion.identity);
        }
    }
}
