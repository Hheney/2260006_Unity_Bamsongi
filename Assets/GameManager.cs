using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private int nScore = 0;      //플레이어의 점수
    private int nTotalScore = 0; //플레이어의 총 점수

    //읽기 전용 프로퍼티, 외부에서는 읽기만 가능
    public int TotalScore { get { return nTotalScore; } }
    public int Score { get { return nScore; } }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("GameManager is null.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; //this : 현재 인스턴스를 가리키는 레퍼런스
        }
        else if (_instance != this)
        {
            Debug.Log("GameManager has another instance.");

            Destroy(gameObject); //현재 인스턴스 파괴(GameManger Object)
        }
        DontDestroyOnLoad(gameObject); //씬이 변경되어도 현재 게임 오브젝트를 유지시키는 메소드
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //디바이스 성능에 따른 실행결과 차이 없애기
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 거리 기반으로 점수 계산 후 총점에 가산하는 메소드
    /// </summary>
    /// <param name="fDistance"></param>
    /// <param name="fMaxRadius"></param>
    public void f_AddScoreByDistance(float fDistance, float fMaxRadius)
    {
        nScore = f_CalculateScoreByZone(fDistance, fMaxRadius);
        nTotalScore += nScore;

        Debug.Log($"거리: {fDistance:F3} 점수: {nScore} 총점: {nTotalScore}");
    }

    /// <summary>
    /// 거리 기반 점수 계산 메소드, 중심에 가까울수록 높은 점수 리턴(최대 10점, 최소 0점)
    /// </summary>
    private int f_CalculateScoreByZone(float fDistance, float fMaxRadius)
    {
        float fStep = fMaxRadius / 6f;

        if (fDistance <= fStep * 1f) return 10;
        else if (fDistance <= fStep * 2f) return 9;
        else if (fDistance <= fStep * 3f) return 8;
        else if (fDistance <= fStep * 4f) return 7;
        else if (fDistance <= fStep * 5f) return 6;
        else if (fDistance <= fStep * 6f) return 5;
        else return 0;
    }

}
