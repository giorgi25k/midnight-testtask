using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider speedSlider;
    public Slider turningSpeedSlider;

    public TMP_Text fishCountText;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        speedSlider.onValueChanged.AddListener(ChangeSpeed);
        turningSpeedSlider.onValueChanged.AddListener(ChangeTurningSpeed);
    }

    public void ChangeSpeed(float newSpeed)
    {
        for (int i = 0; i < GameManager.Instance.fish.Count; i++)
        {
            GameManager.Instance.fish[i].SetSpeed(newSpeed);
        }
    }

    public void ChangeTurningSpeed(float newTurningSpeed)
    {
        for (int i = 0; i < GameManager.Instance.fish.Count; i++)
        {
            GameManager.Instance.fish[i].SetTurningSpeed(newTurningSpeed);
        }
    }

    public void UpdateFishCountText(int count)
    {
        fishCountText.text = "FISH COUNT : " + count;
    }
}
