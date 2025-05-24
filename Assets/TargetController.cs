using System.Collections;
using UnityEngine;

/// <summary> ������ ȸ�� �� Box Collider ���¸� �����ϴ� Ŭ���� </summary>
public class TargetController : MonoBehaviour
{
    [SerializeField] private Transform targetVisual = null;
    [SerializeField] private Collider targetCollider = null;

    //������ ȸ�� ���� ��(������ �⺻������ 90,0,-180 ���·� �����Ǿ� �����Ƿ� �� ���°��� ����ؾ���)
    private Vector3 vStandingRotation = new Vector3(90, 0, -180);   //�� �ִ� ����
    private Vector3 vLyingRotation = new Vector3(0, 0, -180);       //���� �ִ� ����      

    bool isTargetStaning = false; //Ÿ���� �����ִ��� Ȯ���ϴ� bool �ʵ�
    float fRotateSpeed = 360.0f; //�ʴ� ȸ�� �ӵ� (����)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(targetVisual != null)
        {
            targetVisual.localEulerAngles = vLyingRotation; //���� ���۽� �⺻ ������ 90�� �����ִ� ���·� ����
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

    /// <summary>������ ����� �޼ҵ�</summary>
    public void f_StandUpTarget()
    {
        if(isTargetStaning == true)
        {
            return;
        }

        isTargetStaning = true;
        StartCoroutine(f_RotateTargetRoutine(vStandingRotation)); //vStandingRotation ���Ͱ����� ��ƾ ����, ������ �Ͼ��.
    }

    /// <summary>������ ������ �޼ҵ�</summary>
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

        StartCoroutine(f_RotateTargetRoutine(vLyingRotation)); //vLyingRotation ���Ͱ����� ��ƾ ����, ������ ���´�.
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
         * EulerAngles�� 0���� 360���� ����, �׷��� Distance 0���� 360���� �ٸ��ٰ� �ν���
         * localEulerAngles = (359.9, 0, -180) != targetRotation = (0, 0, -180)
         * ���⼭ EulerAngles�� 359.9���� ���Ѵ�� 0�� �����ϴ� ����� �߻��Ͽ� while���� ���ѷ����� ������ ���װ� �߻���
         * ���� �Ʒ��� ���� ������δ� ������ �Ұ��� �Ͽ� �ٸ� ����� ä����
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
