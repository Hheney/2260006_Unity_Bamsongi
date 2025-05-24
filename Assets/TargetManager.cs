using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections; //List 사용을 위해 using


/// <summary>여러 개의 과녁을 관리하고 하나만 랜덤으로 활성화시키는 과녁 매니저 클래스</summary>
public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; } //싱글톤 접근을 위한 인스턴스
    /*
     * 사용할 타겟이 여러개이므로 List에 넣어서 관리하도록 함
     * [SerializeField] private 접근제어를 사용하여 Inspector에서 추가하되 캡슐화 유지
     */
    [SerializeField] private List<TargetController> listTargets = new List<TargetController>();
    [SerializeField] private float fInterval = 3.0f;    //과녁을 세우고 눕히는 간격

    //활성화된 과녁의 Box Collider 활성화 상태를 변경하기 위함
    private TargetController currentActiveTarget = null;
    public TargetController ActiveTarget {  get { return currentActiveTarget; } } //활성화된 과녁 프로퍼티
    bool isPaused = false; //카메라가 Blend중일때 과녁 Coroutine을 일시중단하기 위한 bool 필드

    /*
     * 랜덤값 추출중 이전과 같은 Index값이 추출되면 재시도 하기위한 필드
     * -1인 이유는 사용할 위치가 List 자료형을 사용하므로 Index는 0부터 시작하기에 초기화는 0이 아니라 -1이여야 함
     */
    int nPreviousIndex = -1;

    private void Awake()
    {
        //싱글톤 패턴 구현
        if(Instance == null)
        {
            Instance = this; //this : 현재 인스턴스를 가리키는 레퍼런스
        }
        else
        {
            Debug.LogWarning("TargetManager has another instance.");
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(f_ActiveRandomTargetRoutine()); //랜덤 타겟 루틴 활성화
    }

    //캡슐화 유지를 위한 일시정지 외부 간접 접근 메소드

    /// <summary>랜덤으로 과녁을 일으키는 루틴 일시정지 메소드</summary>
    public void f_PauseTargetRoutine() //일시정지
    {
        isPaused = true;
    }

    /// <summary>랜덤으로 과녁을 일으키는 루틴 재개 메소드</summary>
    public void f_ResumeTargetRoutine() //루틴 재개
    {
        isPaused = false;
    }

    /// <summary>일정 시간마다 과녁 하나만 랜덤으로 일으키는 루틴</summary>
    private IEnumerator f_ActiveRandomTargetRoutine()
    {
        int nNewIndex = 0; //새 추첨 Index번호 지역 변수
        
        while (true)
        {
            while(isPaused) //일시정지가 true값일 경우 루틴 대기
            {
                yield return null;
            }

            if(currentActiveTarget != null)
            {
                currentActiveTarget.f_LieDownTarget(); //과녁 눕히기
            }

            //--------------------------------[중복추첨 방지 기능]--------------------------------
            nNewIndex = nPreviousIndex; //이전 값 저장

            while(nNewIndex == nPreviousIndex && listTargets.Count > 1) //새 인덱스 값이 이전 값과 같으면 while 반복 
            {
                nNewIndex = Random.Range(0, listTargets.Count); //0부터 리스트 갯수(과녁 수)만큼의 범위에서 랜덤값 추출
            }

            nPreviousIndex = nNewIndex; //새로운 값 저장
            //--------------------------------[중복추첨 방지 기능]--------------------------------


            currentActiveTarget = listTargets[nNewIndex];
            currentActiveTarget.f_StandUpTarget(); //추출된 인덱스값을 가진 타겟 세우기

            yield return new WaitForSeconds(fInterval); //세우고 눕히는 간격(Interval)만큼 대기
        }
    }

    /// <summary>
    /// 현재 활성화된 과녁을 반환하는 메소드
    /// </summary>
    /// <returns>현재 세워진 과녁 오브젝트</returns>
    public GameObject f_GetCurrentTarget()
    {
        //삼항연산자 : null이 아닐경우 currentTarget.gameObject 반환 null일 경우 null 반환
        return currentActiveTarget != null ? currentActiveTarget.gameObject : null;
    }
}
