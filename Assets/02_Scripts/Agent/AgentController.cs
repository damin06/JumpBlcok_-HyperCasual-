using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using DG.Tweening;

[RequireComponent(typeof(AudioPlayer))]
[RequireComponent(typeof(Rigidbody))]
public class AgentController : MonoBehaviour
{
    [Header("Baizer Pivot")]
    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;

    [Space]

    [Header("Agent Setting")]
    [SerializeField] private float m_jumpInputAccel = 3f;
    [SerializeField] private float m_jumpSpeed = 2f;
    [SerializeField] private float m_minJumpPower = 3f;
    [SerializeField] private float m_maxJumpPower = 5f;
    [SerializeField] private AnimationCurve m_animationCurve;
    [SerializeField] private bool m_DevMode = false;


    private bool isGround;
    private float m_jumpPower;

    private Rigidbody m_rb;
    private AudioPlayer _audio;

    public float JumpPower
    {
        get { return m_jumpPower; }
        set { m_jumpPower = Mathf.Clamp(value, 0.1f, m_maxJumpPower); }
    }

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioPlayer>();
    }

    private void Start()
    {
        JumpPower = m_minJumpPower;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.localPosition, -transform.up, out RaycastHit hit, 1.2f))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
                isGround = true;
            else
                isGround = false;
        }
        else
        {
            isGround = false;
        }
       
        Debug.DrawRay(transform.localPosition, -transform.up * 1.2f, Color.red);
        Debug.Log($"CurVelocity : {m_rb.velocity.magnitude}");
        AgentInput();
    }

    private void AgentInput()
    {
        if (!isGround || transform.rotation != new Quaternion(0,0,0,1))
            return;

        //!isGround ||
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;

                if(touch.phase == TouchPhase.Began)
                {
                    _audio.PlayerClipWithVariablePitch("Increase", (m_maxJumpPower - m_minJumpPower) / m_jumpInputAccel);
                }

                JumpPower += Time.deltaTime * m_jumpInputAccel;


                if (touch.phase == TouchPhase.Ended)
                {
                    Vector3 dir = BlockManager.Instance.m_curDir == BlockDir.Right ? Vector3.right : Vector3.forward;
                    StartCoroutine(JumpBazier(dir, JumpPower));
                    //JumpRb(dir, JumpPower);
                    _audio.StopAudio();
                    JumpPower = m_minJumpPower;
                }
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _audio.PlayerClipWithVariablePitch("Increase", (m_maxJumpPower - m_minJumpPower) / m_jumpInputAccel);
            }
            else if(Input.GetMouseButton(0)) 
            {
                JumpPower += Time.deltaTime * m_jumpInputAccel;
                transform.localScale = new Vector3(1, Mathf.Clamp( 1 - (JumpPower / (m_maxJumpPower - m_minJumpPower) / 2), 0.5f, 1), 1);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //transform.localScale = Vector3.one;
                transform.DOScaleY(1, 0.2f).SetEase(Ease.InOutBack);
                Debug.Log($"jumpePower : {JumpPower}");
                Vector3 dir = BlockManager.Instance.m_curDir == BlockDir.Right ? Vector3.right : Vector3.forward;
                _audio.StopAudio();
                //JumpRb(dir, JumpPower);
                if (m_DevMode)
                {
                    StartCoroutine(JumpBazier(dir, PlayerPrefs.GetFloat("distance")));
                    return;
                }

                StartCoroutine(JumpBazier(dir, JumpPower));
                JumpPower = m_minJumpPower;
            }
        }
    }

    [ContextMenu("Jump Right")]
    public void JumpeTest()
    {
        JumpRb(Vector3.right, 10);
        //StartCoroutine(Jump(Vector3.right, 10f));
    }

    private void JumpRb(Vector3 dir, float power)
    {
        dir.y = 1.5f;
        //m_rb.velocity = dir * power;
        m_rb.AddForce(dir * power, ForceMode.Impulse);
    }

    private IEnumerator JumpBazier(Vector3 dir, float distance)
    {
        //isGround = false;

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
            //Debug.Log(value);
            yield return null;
        }
        //Debug.Log("break");
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

    public void ReSpawnPlayer()
    {
        transform.position = new Vector3(0, 20, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        m_rb.constraints = RigidbodyConstraints.FreezeRotation;

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            //isGround = true;
            _audio.PlayerClipWithVariablePitch("landing");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Fog"))
        {
            _audio.SimplePlay("Fog");
            UIManager.Instance.OnGameOverSeq();
            GameManager.Instance.GameState = GameState.End;
        }
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
