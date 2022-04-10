using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private int score = 0;

    public float jumpPower = 250f;
    public float turnDuration = 0.01f;
    public float pathSpacing = 1f;
    private bool isTurning = false;

    public float backwardTime = 1f;
    private float currBackwardTime;

    public float idleTime;

    // =============== VARIABLE SWIPE METHOD DARI COACH ===============
    /*
    public int pathIndex = 0;
    public float turnSpeed = 2.5f;
    public float pathSpacing = 1f;
    float pathX;
    */

    public State state;
    public enum State
    {
        Running,
        Falling,
        Backward,
        Idle
    };

    public Lane lane;
    public enum Lane
    {
        Left,
        Middle,
        Right
    }

    private void Awake()
    {
        EventManager.StartListening("jump", Jump);
        EventManager.StartListening("right", TurnRight);
        EventManager.StartListening("left", TurnLeft);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("jump", Jump);
        EventManager.StopListening("right", TurnRight);
        EventManager.StopListening("left", TurnLeft);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        state = State.Running;
        lane = Lane.Middle;
    }

    private void Update()
    {
        // Backward time
        if (currBackwardTime > 0)
        {
            currBackwardTime -= Time.deltaTime;
            if (currBackwardTime <= 0)
            {
                if (state != State.Idle)
                {
                    state = State.Idle;
                    TriggerAnimation("idleTrigger");

                    idleTime = 4;
                    PlayerController.main.cdText.gameObject.SetActive(true);

                    StartCoroutine(SmoothTranlation(transform, transform.parent.position, turnDuration, () => { print("fix position"); }));
                }
            }
        }

        // Idle time
        if (idleTime > 1)
        {
            idleTime -= Time.deltaTime;
            PlayerController.main.cdText.text = (int)idleTime + "";
            if (idleTime <= 1)
            {
                if (state != State.Running)
                {
                    state = State.Running;
                    TriggerAnimation("runTrigger");
                    PlayerController.main.cdText.gameObject.SetActive(false);
                }
            }
        }
    }

    // =============== SWIPE METHOD DARI COACH ===============
    /*
    private void Update()
    {
        if (transform.position.x < pathX)
        {
            transform.position += turnSpeed * Vector3.right * Time.deltaTime;
            if (transform.position.x > pathX)
            {
                transform.position = new Vector3(pathX, transform.position.y, 0);
            }
        }
        else if (transform.position.x > pathX)
        {
            transform.position += turnSpeed * Vector3.left * Time.deltaTime;
            if (transform.position.x < pathX)
            {
                transform.position = new Vector3(pathX, transform.position.y, 0);
            }
        }
    }
    */

    public void SetAnimator()
    {
        animator = gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void TriggerAnimation(string animation)
    {
        animator.SetTrigger(animation);
    }

    public void Jump()
    {
        /*
        if (state != State.Jumping)
        {
            ChangeState(State.Jumping);
            GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpPower, 0));
            animator.SetTrigger("jumpTrigger");
        }
        */
        // =============== CARA COACH LIMIT JUMP ===============
        
        if (rb.velocity.y == 0 && state == State.Running)
        {
            rb.AddForce(new Vector3(0, jumpPower, 0));
            TriggerAnimation("jumpTrigger");
        }
        
    }

    public void TurnRight()
    {
        Turning(Lane.Right);
    }

    public void TurnLeft()
    {
        Turning(Lane.Left);
    }

    public void Turning(Lane lane)
    {        
        if (!isTurning && state == State.Running)
        {
            isTurning = true;
            Transform parent = transform.parent.transform;
            Vector3 target;
            SwitchLane(lane, () => {
                target = Switching();
                StartCoroutine(SmoothTranlation(parent, target * pathSpacing, turnDuration, () => {
                    if (state == State.Running)
                        ChangeState(State.Running);
                    isTurning = false;
                }));
            });
            /*
            StartCoroutine(SmoothTranlation(parent, parent.position + direction * pathSpacing, turnDuration, () => { 
                SwitchLane(lane, () => {
                    if (state == State.Running)
                        ChangeState(State.Running);
                }); 
            }));
            */
        }
    }

    private IEnumerator SmoothTranlation(Transform objectPos, Vector3 target, float duration, System.Action OnTranslationComplete)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration && objectPos.position != target)
        //while (objectPos.position != target)
        {
            objectPos.position = Vector3.Lerp(objectPos.position, target, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectPos.position = target;
        print("movement done");
        OnTranslationComplete();
    }

    private void SwitchLane(Lane toLane, System.Action OnSwitchComplete)
    {
        if (lane != Lane.Middle)
            lane = Lane.Middle;
        else
            lane = toLane;

        //print("Lane: " + lane);
        OnSwitchComplete();
    }

    private Vector3 Switching()
    {
        switch (lane)
        {
            case Lane.Left:
                return Vector3.left;
            case Lane.Middle:
                return Vector3.zero;
            case Lane.Right:
                return Vector3.right;
            default: return Vector3.zero;
        }
    }

    private void ChangeState(State toState)
    {
        state = toState;
        //print("State: " + state);
    }

    // =============== SWIPE METHOD DARI COACH ===============
    /*
    public void Turn(int direction)
    {
        pathIndex += direction;
        if (pathIndex < -1)
            pathIndex = -1;
        else if (pathIndex > 1)
            pathIndex = 1;
        pathX = pathSpacing * pathIndex;
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Obstacle") // Hit obstacle
        {
            if (transform.position.y < 1 && state == State.Running)
            {
                ChangeState(State.Falling);
                animator.SetTrigger("fallTrigger");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Coin") // Hit coin
        {
            other.gameObject.SetActive(false);
            score++;
            PlayerController.main.scoreTxt.text = score.ToString();
        }
    }

    public void RunBackward()
    {
        ChangeState(State.Backward);
        currBackwardTime = backwardTime;

        PlayerController.main.bgTile.ClearObjects(PlayerController.main.bgTile.transform.GetChild(0));
    }
}
