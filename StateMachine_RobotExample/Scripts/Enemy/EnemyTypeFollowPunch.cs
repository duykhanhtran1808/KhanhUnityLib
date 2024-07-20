using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTypeFollowPunch : MonoBehaviour, IDamageable, IObserver
{
    [SerializeField]
    float moveSpeed = 2f; //Tốc độ di chuyển
    [SerializeField]
    float distanceStartAttack = 2f; //Khoảng cách dừng lại tấn công
    [SerializeField]
    float speedJump = 6f;

    //Health==
    [SerializeField]
    private float maxHealth = 30f;
    [SerializeField]
    float currentHealth;
    [SerializeField]
    Slider healthSlide;
    //Health===

    //[SerializeField]
    //float chaseRange = 5f;

    public GameObject targetPlayer;
    protected Rigidbody2D rigidbody2d;
    protected Animator anim;
    float rightSide; // Chiều hướng mặt sang
    Vector3 startPosition; // Vị trí bắt đầu
    bool isOnGround; //Check chạm đất
    protected bool isAttacking; //Trạng thái có đang tấn công
    bool allowAttack = true; //Cho phép tấn công

    void Start()
    {
        ObserverManager.Instance.AddObserver(this);
        currentHealth = maxHealth;
        healthSlide.value = this.currentHealth / this.maxHealth;

        rigidbody2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveDecision(targetPlayer);
    }
    //=============Quản lý Damage và Chết===============
    public void ReduceHealth(float amount)
    {
        this.currentHealth -= amount;
        healthSlide.value = this.currentHealth / this.maxHealth;
        if (this.currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    protected virtual void HandleDeath()
    {
        ObserverManager.Instance.RemoveObserver(this);
        Destroy(this.gameObject);
    }
    //=============Quản lý Damage và Chết===============

    //=============Quản lý Nhận biết Player===============
    private void FoundPlayer(GameObject playerFound) { targetPlayer = playerFound; }
    private void NotFoundPlayer() { targetPlayer = null; }
    //=============Quản lý Nhận biết Player===============

    //=============Quản lý Động tác di chuyển & Tấn Công===============
    private void MoveDecision(GameObject playerFound)
    {
        if (playerFound != null)
        {
            TurnToPosition(playerFound.transform.position);
            isAttacking = true;
            SetDestination(playerFound.transform.position, distanceStartAttack);
        }
        else
        {
            TurnToPosition(startPosition);
            isAttacking = false;
            MoveToPlayer();
            SetDestination(startPosition, distanceStartAttack);
        }
    }

    private void SetDestination(Vector3 goal, float stopDistance)
    {
        float distanceToGoal = Mathf.Abs(this.transform.position.x - goal.x);
        if (distanceToGoal > stopDistance)
        {
            MoveToPlayer();
        }
        else if (isAttacking && 
                 isOnGround && 
                 allowAttack)
        {
            Attacking();
        }
        else if (isOnGround)
        {
            rigidbody2d.velocity = Vector3.zero;
            anim.SetFloat("xSpeed", 0);
        }
    }
    
    public virtual void MoveToPlayer()
    {
        rigidbody2d.velocity = new Vector3(moveSpeed * rightSide, rigidbody2d.velocity.y, 0);
        anim.SetFloat("xSpeed", moveSpeed);
        anim.SetBool("punch", false);
    }

    public virtual void Attacking()
    {
        rigidbody2d.velocity = Vector3.zero;
        anim.SetFloat("xSpeed", 0);
        anim.SetBool("punch", true);
    }

    void TurnToPosition(Vector3 goal)
    {
        rightSide = (goal.x - this.transform.position.x) < 0 ? -1.6f : 1.6f;
        this.transform.localScale = new Vector2(rightSide, this.transform.localScale.y);
    }

    public void TriggerJump()
    {
        anim.SetBool("jump", true);
        anim.Play("Jump");
        rigidbody2d.velocity = new Vector3(rigidbody2d.velocity.x, speedJump, 0);
        isOnGround = false;
    }
    //=============Quản lý Động tác di chuyển & Tấn Công===============
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerJump();
            TurnToPosition(startPosition);
            isAttacking = false;
            SetDestination(startPosition, distanceStartAttack);
        }
        else
        {
            anim.SetBool("jump", false);
            isOnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOnGround = false;
    }

    public void OnPlayerDeath()
    {
        allowAttack = false;
        MoveToPlayer();
    }
}
