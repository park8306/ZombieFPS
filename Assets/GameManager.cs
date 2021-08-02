using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    public float GoalScore = 1500;
    public GameObject zombie;
    GameObject zombies;
    void Start()
    {
        zombies = GameObject.Find("Zombies").gameObject;
        // 좀비 15마리를 랜덤 지역으로 생성시켜 보자   왼쪽 아래부터 시계방향으로 (-70, -58) (-70,28) (36,28) (36, -58) 이 안에서 생성 시켜 보자
        // 생성은 되는데 높이가 안 맞는다 어떻게 해결할까?
        //Instantiate(zombie, new Vector3(Random.Range(-70f,36f), 11 ,Random.Range(-58f, 28f)), zombie.transform.rotation, zombies.transform);
    }
}
