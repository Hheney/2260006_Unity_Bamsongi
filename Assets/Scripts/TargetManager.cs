using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/*
 * [주의사항]
 * TargetManager 사용시, GameManager 또는 InitScene 에서 초기화 필수
 * GameManager → TargetManager.Instance.f_Init();
 */

/// <summary> 여러 개의 과녁을 관리하고 하나만 랜덤으로 활성화시키는 과녁 매니저 클래스 </summary>
public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //과녁 매니저 유지
        }
        else if (Instance != this)
        {
            Debug.Log("TargetManager has another instance.");
            Destroy(gameObject);
        }
    }

    //사용하는 과녁이 다수이므로 List를 사용하여 관리
    private List<TargetController> listTargets = new List<TargetController>();

    //활성화된 과녁의 Box Collider 활성화 상태를 변경하기 위함(충돌시 점수 연산 무효화)
    private TargetController currentActiveTarget = null; 

    private bool isPaused = false;          //카메라가 Blend중일때 과녁 Coroutine을 일시중단하기 위한 bool 필드
    private bool isRoutineRunning = false;  //루틴 중복 실행 방지 플래그
    private Coroutine routineHandle = null; //루틴 핸들 저장

    /*
     * 랜덤값 추출중 이전과 같은 Index값이 추출되면 재시도 하기위한 필드
     * -1인 이유는 사용할 위치가 List 자료형을 사용하므로 Index는 0부터 시작하기에 초기화는 0이 아니라 -1이여야 함
     */
    private int nPreviousIndex = -1;
    private const float fInterval = 3.0f; //과녁을 세우고 눕히는 간격
    public TargetController ActiveTarget {  get { return currentActiveTarget; } } //활성화된 과녁 Read-Only 프로퍼티


    /// <summary> TargetController(TargetPrefab의 컴포넌트)를 런타임에서 수집하고 루틴을 시작하는 초기화 메서드 </summary>
    public void f_Init()
    {
        listTargets.Clear(); //중복 방지 초기화
        TargetController[] foundTargets = Object.FindObjectsByType<TargetController>(FindObjectsSortMode.None); //TargetController 찾기
        //#pragma warning disable CS0618
        //TargetController[] foundTargets = Object.FindObjectsOfType<TargetController>(true);
        //#pragma warning restore CS0618

        listTargets.AddRange(foundTargets); //리스트에 찾은 과녁 추가
        listTargets = listTargets.OrderBy(Target => Target.name).ToList(); //오브젝트 이름을 기준으로 정렬

        if (listTargets.Count == 0)
        {
            Debug.LogWarning("TargetManager: 활성화된 TargetController가 없습니다.");
            return;
        }

        if (!isRoutineRunning)
        {
            routineHandle = StartCoroutine(f_ActiveRandomTargetRoutine()); //랜덤 타겟 루틴 활성화 및 루틴 핸들 저장
        }
    }

    /*
     * 게임 오버등의 Retry 상황에서 GameScene을 다시 로드할 경우 TargetManager는 DontDestroyOnLoad를 통하여 파괴되지 않았으나,
     * 관리중인 TargetController 오브젝트는 씬 전환으로 파괴되고 TargetManager의 f_ActiveRandomTargetRoutine() 루틴은 계속 실행중인 상태로
     * 구조적 문제가 발생하여 MissingReferenceException 에러가 발생함
     * 따라서, 씬 전환 전에 f_ActiveRandomTargetRoutine() 루틴을 종료하는 아래 메소드를 추가하여 해결함
     */
    public void f_Reset()
    {
        if (isRoutineRunning && routineHandle != null)
        {
            StopCoroutine(routineHandle);
        }
        routineHandle = null;
        isRoutineRunning = false;
        currentActiveTarget = null;
        listTargets.Clear();
    }

    public void f_StopTargetRoutine()
    {
        if (isRoutineRunning && routineHandle != null)
        {
            StopCoroutine(routineHandle);
            routineHandle = null;
            isRoutineRunning = false;
        }
    }

    public void f_PauseTargetRoutine() => isPaused = true;
    public void f_ResumeTargetRoutine() => isPaused = false;

    /// <summary>일정 시간마다 과녁 하나만 랜덤으로 일으키는 루틴</summary>
    private IEnumerator f_ActiveRandomTargetRoutine()
    {
        isRoutineRunning = true; //루틴이 실행중임을 알리는 플래그 활성화
        int nNewIndex = 0;       //새 추첨 Index번호 지역 변수

        while (true)
        {
            while (isPaused) //일시정지가 true값일 경우 루틴 대기
            {
                yield return null;
            }

            if (currentActiveTarget != null)
            {
                currentActiveTarget.f_LieDownTarget(); //과녁 눕히기
            }

            nNewIndex = nPreviousIndex; //이전 값 저장

            //새 인덱스 값이 이전 값과 같으면 while 반복
            while (nNewIndex == nPreviousIndex && listTargets.Count > 1) //listTargets.Count > 1 : 과녁이 1개일 경우 무한루프 발생 방지
            {
                nNewIndex = Random.Range(0, listTargets.Count); //0부터 리스트 갯수(과녁 수)만큼의 범위에서 랜덤값 추출
            }

            nPreviousIndex = nNewIndex; //새로운 값 저장
            currentActiveTarget = listTargets[nNewIndex];
            //아이템등의 요소가 등장한다면 Range 기반 랜덤방식보다 가중치 기반 방식으로 변경하여, 특정 조건에서 특정 과녁이 등장 확률을 조절할 수 있도록 확장 가능

            if (currentActiveTarget != null)
            {
                currentActiveTarget.f_StandUpTarget(); //추출된 인덱스값을 가진 타겟 세우기
            }

            yield return new WaitForSeconds(fInterval); //세우고 눕히는 간격(Interval)만큼 대기
        }
    }

    /// <summary> 현재 활성화된 과녁의 GameObject 반환하는 메소드 </summary>
    public GameObject f_GetCurrentTarget()
    {
        //삼항연산자 : null이 아닐경우 currentTarget.gameObject 반환 null일 경우 null 반환
        return currentActiveTarget != null ? currentActiveTarget.gameObject : null;
    }

    /// <summary> 현재 활성화된 과녁의 인덱스 반환하는 메소드 </summary>
    public int f_GetActiveTargetIndex()
    {
        return listTargets.IndexOf(currentActiveTarget);
    }
}
