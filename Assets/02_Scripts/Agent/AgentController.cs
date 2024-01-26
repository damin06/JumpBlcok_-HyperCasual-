using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : MonoBehaviour
{
    private Rigidbody m_rb;
    [SerializeField] private float m_jumpPower = 0f;
    [SerializeField] private float m_jumpSpeed = 2f;
    private float m_touch = 0f;

    [Space]

    [Header("Baizer Test")]
    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;


    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AgentInput()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            m_jumpPower += Time.deltaTime;

            if(touch.phase== TouchPhase.Ended)
            {

            }
        }
    }

    [ContextMenu("Jump Right")]
    public void JumpeTest()
    {
        StartCoroutine(Jump(Vector3.right, 10f));
    }

    private IEnumerator Jump(Vector3 dir, float distance)
    {
        float value = 0f;
        float curve = 5f;

        P1 = transform.position;
        P2 = transform.position + (Vector3.up * curve);
        P3 = transform.position + (dir * distance) + (Vector3.up * curve);
        P4 = transform.position + (dir * distance);

        while(value < 1f)
        {
            transform.position = BazierCurve(P1, P2, P3, P4, value);
            value = Mathf.Lerp(value, 1, Time.deltaTime);

            yield return null;
        }
        Debug.Log("Break!");
    }

    public Vector3 BazierCurve(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, float value)
    {
        Vector3 A = Vector3.Lerp(P1, P2, value);
        Vector3 B = Vector3.Lerp(P2, P3, value);
        Vector3 C = Vector3.Lerp(P3, P4, value);

        Vector3 D = Vector3.Lerp(A, B, value);
        Vector3 E = Vector3.Lerp(B, C, value);

        Vector3 F = Vector3.Lerp(D, E, value);

        return F;
    }
}

[CustomEditor(typeof(AgentController)), CanEditMultipleObjects]
public class Agent_Editor : Editor
{
    private void OnSceneGUI()
    {
        AgentController controller = (AgentController)target;

        controller.P1 = Handles.PositionHandle(controller.P1, Quaternion.identity);
        controller.P2 = Handles.PositionHandle(controller.P2, Quaternion.identity);
        controller.P3 = Handles.PositionHandle(controller.P3, Quaternion.identity);
        controller.P4 = Handles.PositionHandle(controller.P4, Quaternion.identity);

        Handles.DrawLine(controller.P1, controller.P2);
        Handles.DrawLine(controller.P3, controller.P4);

        for(float i = 0; i < 100; i++)
        {
            float _beforeValue = i / 10;
            Vector3 Before = controller.BazierCurve(controller.P1, controller.P2, controller.P3, controller.P4, _beforeValue);
            float _AfterValue = (i + 1) / 10;
            Vector3 After = controller.BazierCurve(controller.P1, controller.P2, controller.P3, controller.P4, _AfterValue);

            Handles.DrawLine(Before, After);
        }
    }
}
