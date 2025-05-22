/*
 * 점수를 관리하는 랭크매니저
 */
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using System.Security.Cryptography; //AES 암호화를 위해 사용

/*
 * 직렬화(Serialization)
 * 오브젝트를 저장하거나 전송할 수 있는 형태로 변환하는 과정을 의미함
 * 이 랭크매니저에서는 JSON 파일 형식으로 전환하기 위해 직렬화 개념을 사용함
 */

///<summary>개별 랭킹 정보 저장 클래스</summary>
[System.Serializable]
public class RankData
{
    [SerializeField] private string sPlayerName = null;
    [SerializeField] private int nScore = 0;

    //프로퍼티
    public string PlayerName { get { return sPlayerName; } }
    public int Score { get { return nScore; } }

    public RankData(string UserName, int UserScore)
    {
        sPlayerName = UserName;
        nScore = UserScore;
    }
}

///<summary>직렬화를 위한 랭크 리스트 래퍼 클래스</summary>
[System.Serializable]
public class RankListWrapper
{
    [SerializeField] private List<RankData> list;

    //프로퍼티
    public List<RankData> RankList { get { return list; } }

    public RankListWrapper(List<RankData> rankList)
    {
        list = rankList;
    }
}

///<summary>JSON 파일로 랭크를 관리하는 랭크 매니저 클래스</summary>
public class RankManager : MonoBehaviour
{
    [SerializeField] private List<RankData> rankList = new List<RankData>();

    private string SavePath => Path.Combine(Application.persistentDataPath, "Rank.json"); //저장 경로 Read-Only 필드

    public IReadOnlyList<RankData> RankList => rankList.AsReadOnly(); //외부에서는 Read-Only 랭크 리스트

    private static RankManager _instance = null;

    public static RankManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("RankManager is null.");
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.Log("RankManager has another instance.");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); //씬이 변경되어도 현재 게임 오브젝트를 유지시키는 메소드

        f_LoadRank(); //시작과 동시에 랭크를 로드
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>랭크 데이터를 JSON으로 저장하는 메소드</summary>
    private void f_SaveRank()
    {
        string json = JsonUtility.ToJson(new RankListWrapper(rankList), true);
        File.WriteAllText(SavePath, json);
    }

    /// <summary>저장된 JSON 파일로부터 랭크 데이터를 불러오는 메소드</summary>
    private void f_LoadRank()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            RankListWrapper wrapper = JsonUtility.FromJson<RankListWrapper>(json);
            if (wrapper?.RankList != null)
            {
                rankList = wrapper.RankList;
            }
        }
    }

    /// <summary>
    /// 새로운 랭크 데이터를 추가하고 리스트를 정렬 및 저장하는 메소드
    /// </summary>
    /// <param name="playerName">플레이어 이름</param>
    /// <param name="score">획득한 점수</param>
    public void f_AddRank(string playerName, int score)
    {
        rankList.Add(new RankData(playerName, score));
        rankList.Sort((a, b) => b.Score.CompareTo(a.Score)); //점수 내림차순

        if (rankList.Count > 10)
        {
            rankList.RemoveAt(rankList.Count - 1); //상위 10개만 유지
        }

        f_SaveRank();
    }
}

