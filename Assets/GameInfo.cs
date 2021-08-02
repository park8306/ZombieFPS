using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    public static GameInfo instance;
    Text ScoreText;
    GameManager gameManager;
    public float score;
    private void Awake()
    {
        instance = this;
        ScoreText = transform.Find("ScoreText").GetComponent<Text>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void Update()
    {
        ScoreText.text = $"{score}/{gameManager.GoalScore}";
    }
}
