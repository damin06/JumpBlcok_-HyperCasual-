using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : MonoBehaviour
{
    [Header("Baizer Test")]
    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;

    [Space]

    [Header("Agent Setting")]
    [SerializeField] private float m_jumpSpeed = 2f;
    [SerializeField] private float m_maxJumpPower = 5f;
    [SerializeField] private AnimationCurve m_animationCurve;


    private bool isGround = false;
    private Rigidbody m_rb;
    private float m_jumpPower;

    public float JumpPower
    {
        get { return m_jumpPower; }
        set { m_jumpPower = Mathf.Clamp(value, 0.1f, m_maxJumpPower); }
    }



    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        AgentInput();
    }

    private void AgentInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            JumpPower += Time.deltaTime;

            if (touch.phase == TouchPhase.Ended)
            {
                Vector3 dir = BlockManager.Instance.m_curDir == BlockDir.Right ? Vector3.right : Vector3.forward;
                Jump(dir, JumpPower);
                JumpPower = 0;
            }
        }
    }

    [ContextMenu("Jump Right")]
    public void JumpeTest()
    {
        //m_rb.AddForce(new Vector3(1, 1f, 0) * 12, ForceMode.Impulse);
        StartCoroutine(Jump(Vector3.right, 10f));
    }

    private IEnumerator Jump(Vector3 dir, float distance)
    {
        float time = 0f;
        float value = 0f;
        float curve = 5f;

        P1 = transform.position;
        P2 = transform.position + (Vector3.up * curve);
        P3 = transform.position + (dir * distance) + (Vector3.up * curve);
        P4 = transform.position + (dir * distance);

        while (value < 1f)
        {
            transform.position = BazierCurve(P1, P2, P3, P4, value);
            //value = Mathf.Lerp(value, 1.1f, Time.deltaTime);
            time = Mathf.Clamp(time + Time.deltaTime, 0, 1);
            //value = m_animationCurve.Evaluate(time);
            value = easeOutCirc(time);
            Debug.Log(value);
            yield return null;
        }
        Debug.Log("break");
    }

    private float easeOutCirc(float value) => (Mathf.Sqrt(1 - Mathf.Pow(value-1, 2)));
    private float easeOutQuad(float value) => (1 - (1 - value) * (1 - value));

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
