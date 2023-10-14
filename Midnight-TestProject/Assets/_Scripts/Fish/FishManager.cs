using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [SerializeField] private GameObject _fishPrefab;

    public static int _numFish = 10;
    public static int tankSize = 8;
    public static GameObject[] allFish = new GameObject[_numFish];
    public static Vector3 goalPos = Vector3.zero;

    private void Start()
    {
        for (int i = 0; i < _numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize),
                Random.Range(-tankSize, tankSize),
                Random.Range(-tankSize, tankSize));
            allFish[i] = Instantiate(_fishPrefab, pos, Quaternion.identity);
        }
    }

    private void Update()
    {
        if(Random.Range(0,10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-tankSize, tankSize),
                Random.Range(-tankSize, tankSize),
                Random.Range(-tankSize, tankSize));
        }
    }
}
