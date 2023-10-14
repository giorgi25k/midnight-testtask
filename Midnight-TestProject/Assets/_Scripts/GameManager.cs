using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Fish_Flock> fish;

    private void Awake()
    {
        Instance = this;
    }
}
