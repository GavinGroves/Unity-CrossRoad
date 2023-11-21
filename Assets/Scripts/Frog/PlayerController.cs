using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private enum Direction
    {
        Up,
        Right,
        Left
    }

    private PlayerInput _playerInput;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private BoxCollider2D coll;

    [Header("得分")] [SerializeField] private int stepPoint;
    private int pointResult;

    [Header("跳跃")] [SerializeField] private float jumpDistance;
    private float moveDistance;

    private bool isButtonHeld;
    private bool isJump; //bool 类型的默认值为 false。
    private bool canJump;
    private bool isDead;

    private Vector2 destination;
    private Vector2 touchPosition;
    private Direction dir;

    [Header("方向指示")] public SpriteRenderer signRenderer;
    public Sprite upSign;
    public Sprite leftSign;
    public Sprite rightSign;

    //判断碰撞检测返回的物体
    private RaycastHit2D[] result = new RaycastHit2D[2];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (isDead)
        {
            DisableInput();
            return;
        }

        if (canJump)
        {
            TriggerJump();
        }
    }

    private void FixedUpdate()
    {
        if (isJump)
            rb.position = Vector2.Lerp(transform.position, destination, 0.134f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water") && !isJump)
        {
            Physics2D.RaycastNonAlloc(transform.position + Vector3.up * 0.1f, Vector2.zero, result);
            bool isWater = true;
            foreach (var hit in result)
            {
                if (hit.collider == null) continue;
                if (hit.collider.CompareTag("Wood"))
                {
                    //在木板上跟随移动;成为木板的子物体
                    transform.parent = hit.collider.transform;
                    isWater = false;
                }
            }

            //没有模板游戏结束
            if (isWater && !isJump)
            {
                Debug.Log("GG!");
                isDead = true;
            }
        }

        if (other.CompareTag("Border") || other.CompareTag("Car"))
        {
            Debug.Log("Game Over!");
            isDead = true;
        }

        if (!isJump && other.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over!!!!");
            isDead = true;
        }

        if (isDead)
        {
            //广播通知游戏结束
            EventHandler.CallGameOverEvent();
            coll.enabled = false;
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Wood"))
    //     {
    //         transform.parent = null;
    //     }
    // }

    #region INPUT 输入回调函数

    //TODO:执行跳跃，跳跃的距离，记录分数，播放跳跃的音效
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !isJump)
        {
            moveDistance = jumpDistance;
            //执行跳跃
            canJump = true;
            AudioManager.instance.SetJumpClip(0);
        }

        //得分
        if (dir == Direction.Up && context.performed && !isJump)
        {
            pointResult += stepPoint;
        }
    }

    public void LongJump(InputAction.CallbackContext context)
    {
        if (context.performed && !isJump)
        {
            moveDistance = jumpDistance * 2;
            isButtonHeld = true;
            AudioManager.instance.SetJumpClip(1);
            signRenderer.gameObject.SetActive(true);
        }

        if (context.canceled && isButtonHeld && !isJump)
        {
            if (dir == Direction.Up)
                pointResult += stepPoint * 2;

            //执行跳跃
            isButtonHeld = false;
            canJump = true;
            signRenderer.gameObject.SetActive(false);
        }
    }

    public void GetTouchPosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Debug.Log(context.ReadValue<Vector2>());
            touchPosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            // Debug.Log(touchPosition);
            var offset = ((Vector3)touchPosition - transform.position).normalized;
            if (Mathf.Abs(offset.x) <= 0.7f)
            {
                dir = Direction.Up;
                signRenderer.sprite = upSign;
            }
            else if (offset.x < 0)
            {
                dir = Direction.Left;
                if (transform.localScale.x == -1)
                {
                    signRenderer.sprite = rightSign;
                }
                else
                {
                    signRenderer.sprite = leftSign;
                }
            }
            else if (offset.x > 0)
            {
                dir = Direction.Right;
                if (transform.localScale.x == -1)
                {
                    signRenderer.sprite = leftSign;
                }
                else
                {
                    signRenderer.sprite = rightSign;
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// 触发执行跳跃动画
    /// </summary>
    private void TriggerJump()
    {
        canJump = false;
        switch (dir)
        {
            case Direction.Up:
                anim.SetBool("isSide", false);
                destination = new Vector2(transform.position.x, transform.position.y + moveDistance);
                transform.localScale = Vector3.one;
                // sr.flipX = true;
                break;
            case Direction.Right:
                anim.SetBool("isSide", true);
                destination = new Vector2(transform.position.x + moveDistance, transform.position.y);
                //翻转
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case Direction.Left:
                anim.SetBool("isSide", true);
                destination = new Vector2(transform.position.x - moveDistance, transform.position.y);
                transform.localScale = Vector3.one;
                break;
        }

        anim.SetTrigger("Jump");
    }

    #region Animation Event

    public void JumpAnimationEvent()
    {
        //播放跳跃音效
        AudioManager.instance.PlayJumpFX();
        //改变状态
        isJump = true;
        //修改排序图层
        sr.sortingLayerName = "Front";
        //修改Parent
        transform.parent = null;
    }

    public void FinishJumpAnimationEvent()
    {
        isJump = false;
        sr.sortingLayerName = "Middle";
        if (dir == Direction.Up && !isDead)
        {
            //TODO: 得分、触发地面检测
            // _terrainManager.CheckPosition();

            EventHandler.CallGetPointEvent(pointResult);

            Debug.Log("总得分:" + pointResult);
        }
    }

    #endregion

    private void DisableInput()
    {
        _playerInput.enabled = false;
    }
}