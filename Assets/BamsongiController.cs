/*
 * 마우스를 클릭하면 밤송이(Chestnut Bur)가 과녁으로 날아가는 동작 제어 스크립트
 */
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    GameObject gTarget = null; //과녁 오브젝트 오브젝트 변수
    BoxCollider boxCollider = null; //과녁의 Box Collider 값을 가져오기 위함

    Vector2 vHitXY = Vector2.zero;      //거리 계산을 위한 밤송이의 타격점 X, Y좌표값 벡터
    Vector2 vCenterXY = Vector2.zero;   //거리 계산을 위한 중심점 X, Y좌표값 벡터

    float fDistance = 0.0f;     //밤송이 타격지점과, 원의 중심까지의 거리
    float fMaxRadius = 0.0f;    //과녁의 크기
    float fKillObjTime = 3.0f;  //오브젝트 삭제 시간

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gTarget = GameObject.Find("Target");                //과녁 오브젝트 불러오기
        boxCollider = gTarget.GetComponent<BoxCollider>();  //과녁 오브젝트의 BoxCollider 기능 불러오기
    }

    // Update is called once per frame
    void Update()
    {
        //밤송이가 화면 아래로 낙하시 삭제
        if (transform.position.y < -3.0f)
        {
            Destroy(gameObject); 
        }
    }

    /*
     * 밤송이가 화면 안쪽으로 날아가도록 +Z축 방향의 벡터를 매개변수로 전달하고 f_TargetShoot 메소드 호출
     * Y축 방향으로 힘을 200.0f 가하는 이유는 밤송이가 과녁에 닫기 전에 중력의 영향을 받아
     * 지면으로 낙하하는 것을 막기 위함
     * Start 메소드를 호출하는 시작과 동시에 밤송이가 과녁으로 날아감
     */

    ///<summary>밤송이에게 힘을 가하는 메소드</summary>
    public void f_TargetShoot(Vector3 argDir)
    {
        //매개변수로 전달된 Vector값으로 힘을 가한다.
        GetComponent<Rigidbody>().AddForce(argDir);
    }

    //Physics를 사용하므로 과녁과 밤송이가 충돌하면 OnCollisionEnter 메소드가 호출되어 실행됨
    private void OnCollisionEnter(Collision collision)
    {
        /*
         * 밤송이가 과녁에 닿는 순간 밤송이 움직임이 멈추므로, Rigidbody 컴포넌트의 isKinematic 메소드를 true로 설정
         * isKinematic 메소드를 true로 설정 하면, 오브젝트에 작용하는 힘을 무시하고 밤송이를 정지시킴
         * isKinematic 메소드 : 외부에서 가해지는 물리적 힘에 반응하지 않는 오브젝트라는 의미. 중력과 충돍에 반응하지 않도록 함
         */
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play(); //파티클 재생

        if (boxCollider == null) //BoxCollider가 없는 경우
        {
            Debug.LogWarning($"{gTarget.gameObject.name}에 BoxCollider가 없습니다.");
            return;
        }

        //boxCollider.size를 사용하여 Inspector에서 값을 변경해도 자동으로 타켓 원의 크기 계산
        fMaxRadius = (boxCollider.size.x / 2.0f);

        /*
         * 밤송이가 Sphere Collider 형태라 타격지점이 모호할 수 있는 문제가 발생함
         * collision.contacts[0].point를 사용하여 정확한 충돌 접점 위치값을 사용함
         * 그러나, collision.contacts[0].point 값을 그대로 사용하면 z축값이 계산에 포함됨
         * z축값은 원의 중심에 타격해도 거리는 크게 계산되는 문제가 발생함
         * 따라서 Vector2를 사용하여 x,y값만 반영함
         */
        vHitXY = collision.contacts[0].point; //타격지점 저장
        vCenterXY = new Vector2(boxCollider.center.x, boxCollider.center.y); //중심점 저장
        fDistance = Vector2.Distance(vHitXY, vCenterXY); //타격지점과 중심점간의 거리 계산

        GameManager.Instance.f_AddScoreByDistance(fDistance, fMaxRadius); //점수 계산 및 누적 처리

        UIManager.Instance.f_UpdateScore(); //점수 UI 갱신
        UIManager.Instance.f_UpdateTotalScore(); //총점 UI 갱신

        Destroy(gameObject, fKillObjTime); //3초뒤 삭제
    }
}
