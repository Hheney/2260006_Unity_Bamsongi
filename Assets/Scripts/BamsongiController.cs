/*
 * 마우스를 클릭하면 밤송이(Chestnut Bur)가 과녁으로 날아가는 동작 제어 스크립트
 */
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    Transform targetCenter = null;  //과녁의 중심점 투명 오브젝트의 좌표값 
    BoxCollider boxCollider = null; //과녁의 Box Collider 값을 가져오기 위함

    Vector3 vHitPoint = Vector3.zero;   //밤송이의 충돌위치 좌표값 벡터
    Vector2 vHitXY = Vector2.zero;      //거리 계산을 위한 밤송이의 X, Y좌표값 벡터
    Vector2 vCenterXY = Vector2.zero;   //거리 계산을 위한 중심점 X, Y좌표값 벡터

    float fDistance = 0.0f;     //밤송이 타격지점과, 원의 중심까지의 거리
    float fMaxRadius = 0.0f;    //과녁의 크기

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TargetCenter 위치를 기준으로 점수 계산하므로 TargetCenter의 transform 값을 불러온다
        targetCenter = GameObject.Find("TargetCenter").transform;

        //TargetCenter의 부모(Target)에서 BoxCollider를 가져온다.
        //Target 오브젝트를 Find하여도 되지만 Target과 TargetCenter는 상속관계이므로 아래와 같이 작성함
        boxCollider = targetCenter.transform.parent.GetComponent<BoxCollider>();

        /*
         * 밤송이가 화면 안쪽으로 날아가도록 +Z축 방향의 벡터를 매개변수로 전달하고 f_TargetShoot 메소드 호출
         * Y축 방향으로 힘을 200.0f 가하는 이유는 밤송이가 과녁에 닫기 전에 중력의 영향을 받아
         * 지면으로 낙하하는 것을 막기 위함
         * Start 메소드를 호출하는 시작과 동시에 밤송이가 과녁으로 날아감
         */
        f_TargetShoot(new Vector3(0.0f, 200.0f, 2000.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 매개변수 방향으로 밤송이에게 힘을 가하는 메소드
    /// </summary>
    /// <param name="argDir">방향</param>
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

        
        if (targetCenter == null)
        {
            Debug.LogWarning("TargetCenter를 찾을 수 없습니다.");
            return;
        }
        
        if (boxCollider == null)
        {
            Debug.LogWarning("Target에 BoxCollider가 없습니다.");
            return;
        }

        //boxCollider.size를 사용하여 Inspector에서 값을 변경해도 자동으로 타켓 원의 크기 계산
        fMaxRadius = (boxCollider.size.x / 2.0f);

        /*
         * 밤송이가 Sphere Collider 형태라 타격지점이 모호할 수 있는 문제가 발생함
         * collision.contacts[0].point를 사용하여 정확한 충돌 접점 위치값을 사용함
         */
        vHitPoint = collision.contacts[0].point;

        /*
         * collision.contacts[0].point 값을 그대로 사용하면
         * TargetCenter가 Target의 내부에 존재하여 z값이 계산에 포함됨
         * 즉, Vector3.Distance 사용시 z값이 포함되기 때문에 실제 원의 중심에 타격해도 거리는 크게 계산되는 문제가 발생함
         * 이 문제를 해결하기위해, z축 값은 제외시키로 결정함
         */
        vHitXY = new Vector2(vHitPoint.x, vHitPoint.y);
        vCenterXY = new Vector2(targetCenter.position.x, targetCenter.position.y);

        fDistance = Vector2.Distance(vHitXY, vCenterXY);

        GameManager.Instance.f_AddScoreByDistance(fDistance, fMaxRadius); //점수 계산 및 누적 처리
    }
}
