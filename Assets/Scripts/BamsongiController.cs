/*
 * 마우스를 클릭하면 밤송이(Chestnut Bur)가 과녁으로 날아가는 동작 제어 스크립트
 */
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    //밤송이의 궤적을 시각적 표현을 위해 LineRenderer를 사용함
    LineRenderer lineRenderer = null;
    List<Vector3> trajectoryPoint = new List<Vector3>(); //밤송이의 궤적 지점 List

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        lineRenderer = GetComponent<LineRenderer>(); //LineRenderer 기능 가져오기
        lineRenderer.positionCount = 0;
    }

    private void FixedUpdate()
    {
        f_RenderTrajectory(); //화면상에 궤적 그리기
    }


    // Update is called once per frame
    void Update()
    {
        f_CheckFallAndDestroy(); //과녁에 맞지 않고 지면아래로 낙하시 오브젝트를 삭제함
    }

    ///<summary>밤송이에게 발사 방향으로 힘을 가하는 메소드</summary>
    public void f_TargetShoot(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction); //매개변수로 전달된 Vector값으로 힘을 가한다.
    }

    //Physics를 사용하므로 과녁과 밤송이가 충돌하면 OnCollisionEnter 메소드가 호출되어 실행됨
    //충돌 발생시 점수 계산 및 연출을 처리함
    private void OnCollisionEnter(Collision collision)
    {
        TargetController activeTarget = TargetManager.Instance.ActiveTarget; //활성화된 타겟 정보 불러오기

        //현재 활성화된 과녁이 없거나 충돌 대상이 다르면 Early return하여 다음 코드부분 실행을 막음
        if (activeTarget == null || collision.gameObject != activeTarget.gameObject)
        {
            Debug.Log("비활성된 과녁에 충돌하였습니다. 충돌을 무시합니다.");
            return;
        }

        //랜덤 과녁 루틴 일시정지 및 충돌관련 처리
        TargetManager.Instance.f_PauseTargetRoutine();  
        GetComponent<Rigidbody>().isKinematic = true;   
        GetComponent<ParticleSystem>().Play();
        SoundManager.Instance.f_PlaySFX(SoundName.SFX_Crash, 1.0f);
        
        //BoxCollider의 Center 좌표값을 월드 좌표기준으로 변환
        BoxCollider boxCollider = activeTarget.GetComponent<BoxCollider>();
        Vector3 vCenterWorld = activeTarget.transform.TransformPoint(boxCollider.center);

        //점수 계산 : 충돌 지점과 과녁의 중심점간의 거리 산출
        float fDistance = Vector3.Distance(collision.contacts[0].point, vCenterWorld);
        float fMaxRadius = boxCollider.size.x / 2.0f; //원 크기 계산

        GameManager.Instance.f_AddScoreByDistance(fDistance, fMaxRadius);
        UIManager.Instance.f_UpdateScore();
        UIManager.Instance.f_UpdateTotalScore();

        GameManager.Instance.CanShoot = false;
        CameraManager.Instance.f_MoveCameraRoutine();

        //카메라의 Blend가 종료된 경우 오브젝트 삭제
        CameraManager.Instance.OnCameraBlendComplete += f_DestroyBamsongiAfterBlend;
    }

    private void f_DestroyBamsongiAfterBlend()
    {
        Destroy(gameObject); //오브젝트 삭제
        CameraManager.Instance.OnCameraBlendComplete -= f_DestroyBamsongiAfterBlend; //메모리 누수 방지 "-=" 호출 후 이벤트에서 제거
    }

    /// <summary>RigidBody의 궤적을 실시간으로 기록하고 그리는 메소드</summary>
    void f_RenderTrajectory()
    {
        if (!GetComponent<Rigidbody>().isKinematic) //Rigidbody가 움직이는 동안의 위치를 궤적으로 그림
        {
            trajectoryPoint.Add(transform.position);                //현재 위치를 리스트에 기록함
            lineRenderer.positionCount = trajectoryPoint.Count;     //리스트 갯수만큼 lineRenderer가 몇 개의 점을 그릴지 할당
            lineRenderer.SetPositions(trajectoryPoint.ToArray());   //SetPositions 메소드를 사용해서 Vector3 배열을 LineRenderer가 화면상에 그린다.
        } 
    }

    /// <summary>밤송이가 화면 아래로 낙하시 삭제하는 메소드</summary>
    void f_CheckFallAndDestroy()
    {
        if (transform.position.y < -3.0f) //지면 아래로 낙하시 
        {
            GameManager.Instance.f_DecreaseShotCount(); //기회 1회 차감
            UIManager.Instance.f_UpdateTotalScore(); //UI 갱신
            Destroy(gameObject);
        }
    }
}
