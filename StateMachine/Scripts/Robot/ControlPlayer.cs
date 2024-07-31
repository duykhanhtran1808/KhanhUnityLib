using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayerMask;
    public float overlapCircleRadius = 0.4328545f;
    public Transform groundCheck;


    private Animator anim;
    private Rigidbody2D rg;
    private BoxCollider2D boxCollider2D;
    public GameObject Player;
    public GameObject Bullet;

    //Có di chuyển và chạy không
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isSuperRun = false;
    private Coroutine superRunPending;

    //Chiều của nhân vật
    public int isForward = 1;
    //Có chạm đất không
    public bool isOnGround;

    //Số lần nhảy còn lại
    public int jumpNumber = 0;

    //Tốc độ & Tốc độ nhảy
    public float speed;
    public float basicSpeed = 3;
    public float normalRun = 3;
    public float superRun = 3;
    public float speedJump = 6;

    // Scale của Player
    Vector3 scale;

    PlayerAttack playerAttack;

    // Start is called before the first frame update
    void Start()
    {
        anim = Player.GetComponent<Animator>();
        boxCollider2D = Player.GetComponent<BoxCollider2D>();
        rg = Player.GetComponent<Rigidbody2D>();
        scale = Player.transform.localScale; 
        playerAttack = GetComponent<PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        checkIsOnGround();
        //checkGroundOverlapCircle();
        if (isMoving)
        {
            AddForce(speed);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { Jump(); }

        if (Input.GetKeyDown(KeyCode.RightArrow)) { isMoving = true; isForward = 1; StartMove(); }
        if (Input.GetKeyUp(KeyCode.RightArrow)) { isMoving = false; isSuperRun = false; StopMove(); }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) { isMoving = true; isForward = -1; StartMove(); }
        if (Input.GetKeyUp(KeyCode.LeftArrow)) { isMoving = false; isSuperRun = false; StopMove(); }

        if (Input.GetKeyDown(KeyCode.Space)) { isRunning = true; StartMove(); }
        if (Input.GetKeyUp(KeyCode.Space)) { isRunning = false; isSuperRun = false; StopMove(); }

        if (Input.GetKeyDown(KeyCode.X)) { playerAttack.FireBullet(isForward); }
        updateAnim();
    }
    //=========================Cấu hình phím điều khiển=========================================================
    public void PreviousUp_Click() //Input.GetKeyUp(KeyCode.LeftArrow)
    {
        isMoving = false; isSuperRun = false; StopMove();
    }

    public void PreviousDown_Click() //Input.GetKeyDown(KeyCode.LeftArrow)
    {
        isMoving = true; isForward = -1; StartMove();

    }

    public void NextUp_Click() //Input.GetKeyUp(KeyCode.RightArrow)
    {
        isMoving = false; isSuperRun = false; StopMove();
    }

    public void NextDown_Click() //Input.GetKeyDown(KeyCode.RightArrow)
    {
        isMoving = true; isForward = 1; StartMove();
    }

    public void AUp_Click()
    {
    }

    public void ADown_Click() //Input.GetKeyDown(KeyCode.UpArrow)
    {
        Jump();
    }

    public void BUp_Click() //Input.GetKeyUp(KeyCode.Space)
    {
        isRunning = false; isSuperRun = false; StopMove();
    }

    public void BDown_Click() //Input.GetKeyDown(KeyCode.Space)
    {
        isRunning = true; StartMove();
    }

    public void XDown_Click() //Input.GetKeyDown(KeyCode.X)
    {
        playerAttack.FireBullet(isForward);
    }

    //=========================Lực của Rigid Body=========================================================
    private void AddForce(float speed)
    {
        rg.velocity = new Vector3(speed, rg.velocity.y, 0);

    }

    private void AddForceUp()
    {
        rg.velocity = new Vector3(rg.velocity.x, speedJump, 0);

    }
    //=========================Xử lý nhảy=========================================================
    private void Jump()
    {
        
        if (!isOnGround && jumpNumber > 0)
        {
            Invoke("DoubleJump", 0);
            jumpNumber--;
        }

        if (isOnGround)
        {
            AddForceUp();
        }

    }

    private void DoubleJump()
    {
        speed = Mathf.Abs(rg.velocity.x) >= basicSpeed ? basicSpeed * isForward : rg.velocity.x;
        AddForceUp();
        anim.Play("robotBoy_jump", -1, 0f);
    }

    //=========================Xử lý di chuyển + chạy=========================================================
    private void StopMove()
    {
        if(isOnGround && !isMoving)
        {
            speed = 0;
            rg.velocity = Vector3.zero;
        }
        if (isOnGround && isMoving)
        {
            StartMove();
        }

    }

    private void StartMove()
    {
        if (!isMoving) return;

        scale.x = Mathf.Abs(scale.x) * isForward;
        Player.transform.localScale = scale;

        if (!isRunning && isOnGround)
        {
            speed = basicSpeed * isForward;
            
        }
        else if(isRunning && isOnGround)
        {
            speed = basicSpeed * normalRun * isForward;
            if (superRunPending != null) StopCoroutine(superRunPending);
            superRunPending = StartCoroutine(activeSuperRun(2f));
            
        }
        else if(!isOnGround)
        {
            speed = basicSpeed * isForward;
        }
        
    }

    IEnumerator activeSuperRun(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if(isMoving && isRunning && isOnGround)
        {
            speed = superRun * isForward + speed;
            isSuperRun = true;
        }
    }

    //=========================Xử lý chạm đất/không chạm đất=========================================================
    private void handleTouchGround()
    {
        speed = 0;
        rg.velocity = Vector3.zero;
        if (isMoving)
        {
            StartMove();
        }
    }

    private void checkIsOnGround()
    {
        float extraHeight = 0.05f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, extraHeight, groundLayerMask);
        Color rayColor;
        if(raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        //========================Debug========================
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + extraHeight), Vector2.right * (boxCollider2D.bounds.extents.x * 2), rayColor);
        
        //========================Debug========================
        if(raycastHit.collider != null)
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
        //Debug.Log("Va chạm BoxCast: " + isOnGround);
    }

    //private void checkGroundOverlapCircle()
    //{
    //    bool touchGround = Physics2D.OverlapCircle(groundCheck.position, overlapCircleRadius, groundLayerMask);
    //    //Debug.Log("Va chạm OverlapCiecle: " + touchGround);
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isOnGround)
        {
            handleTouchGround();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (!isOnGround)
        {
            jumpNumber = 2;
        }
    }
    //=========================Cập nhật anim=========================================================
    private void updateAnim()
    {
        anim.SetBool("isOnGround", isOnGround);
        anim.SetFloat("yVelocity", rg.velocity.y);
        anim.SetFloat("speed", Mathf.Abs(speed));
    }

    //=========================Hàm khác: dùng để test=========================================================

}
