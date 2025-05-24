using System.Collections;
using UnityEngine;

/// <summary> 과녁의 회전 및 Box Collider 상태를 제어하는 클래스 </summary>
public class TargetController : MonoBehaviour
{
    [SerializeField] private Transform targetVisual = null;
    [SerializeField] private Collider targetCollider = null;

    //과녁의 회전 기준 값(과녁이 기본적으로 90,0,-180 상태로 설정되어 있으므로 이 상태값을 고려해야함)
    private Vector3 vStandingRotation = new Vector3(90, 0, -180);   //서 있는 상태
    private Vector3 vLyingRotation = new Vector3(0, 0, -180);       //누워 있는 상태      

    bool isTargetStaning = false; //타겟이 누워있는지 확인하는 bool 필드
    float fRotateSpeed = 360.0f; //초당 회전 속도 (각도)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(targetVisual != null)
        {
            targetVisual.localEulerAngles = vLyingRotation; //게임 시작시 기본 상태인 90도 누워있는 상태로 변경
        }

        if(targetCollider != null)
        {
            targetCollider.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>과녁을 세우는 메소드</summary>
    public void f_StandUpTarget()
    {
        if(isTargetStaning == true)
        {
            return;
        }

        isTargetStaning = true;
        StartCoroutine(f_RotateTargetRoutine(vStandingRotation)); //vStandingRotation 벡터값으로 루틴 실행, 과녁이 일어선다.
    }

    /// <summary>과녁을 눕히는 메소드</summary>
    public void f_LieDownTarget()
    {
        if(isTargetStaning == false)
        {
            return;
        }

        isTargetStaning = false;

        if(targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        StartCoroutine(f_RotateTargetRoutine(vLyingRotation)); //vLyingRotation 벡터값으로 루틴 실행, 과녁이 눕는다.
    }

    private IEnumerator f_RotateTargetRoutine(Vector3 vTargetEuler)
    {
        Quaternion targetRotation = Quaternion.Euler(vTargetEuler);
        
        while (Quaternion.Angle(targetVisual.rotation, targetRotation) > 0.5f)
        {
            targetVisual.rotation = Quaternion.RotateTowards(targetVisual.rotation, targetRotation, fRotateSpeed * Time.deltaTime);

            yield return null;
        }

        targetVisual.rotation = targetRotation;

        /*
         * EulerAngles는 0도와 360도가 같음, 그러나 Distance 0도와 360도를 다르다고 인식함
         * localEulerAngles = (359.9, 0, -180) != targetRotation = (0, 0, -180)
         * 여기서 EulerAngles의 359.9값이 무한대로 0에 근접하는 결과가 발생하여 while문이 무한루프에 빠지는 버그가 발생함
         * 따라서 아래와 같은 방법으로는 구현이 불가능 하여 다른 방식을 채택함
         */
        /* 
        while(Vector3.Distance(targetVisual.localEulerAngles, vRotation) > 1.0f)
        {
            targetVisual.localEulerAngles = Vector3.Lerp(targetVisual.localEulerAngles, vRotation, Time.deltaTime * 5.0f);

            yield return null;
        }

        targetVisual.localEulerAngles = vRotation;
        */
    }

}
