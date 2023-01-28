using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Networking.Types;
using Color = UnityEngine.Color;

public class Player4 : MonoBehaviour
{
    private Vector2 _inputVector;
    private bool jumpPress;
    private bool jumpKeyDown;
    private bool dashPress;
    private bool isDead;
    private Rigidbody2D rigid;
    private Animator animator;
    public GameObject hair;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true;
        groundMask = LayerMask.GetMask("Boundary");
        dashLeft = _dashCount;
        currentHealth = maxHealth;
        hurtTimer = 0.0f;
    }

    void Update()
    {
        if (!isDead)
        {
            GatherInput();
            HandleGrounding();
            HandleWalking();
            HandleJumping();
            HandleWallSlide();
            HandleDashing();
            HandleHair();
            // Hurt Timer
            if (hurtTimer > 0.0f)
            {
                hurtTimer = Mathf.Clamp(hurtTimer - Time.deltaTime, 0.0f, float.MaxValue);
            }
            // Make player not to fall too fast
            rigid.velocity = new Vector2(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y, -20f, float.MaxValue));
        }
    }

    private bool facingLeft;
    private void GatherInput()
    {
        _inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        jumpPress = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C);
        jumpKeyDown = Input.GetButton("Jump") || Input.GetKey(KeyCode.C);
        dashPress = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.J);
        facingLeft = (_inputVector.x < 0 || facingLeft) && _inputVector.x <= 0;
        animator.SetFloat("InputX", _inputVector.x);
        animator.SetBool("facingLeft", facingLeft);
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.flipX = facingLeft;
        }
    }

    private bool isGrounded;
    private bool onWall;
    private bool onRightWall;
    private bool onLeftWall;
    private int groundMask;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.125f;
    [SerializeField] private Vector2 bottomOffset, rightOffset, leftOffset;
    [SerializeField] private Color debugCollisionColor = Color.red;

    private void HandleGrounding()
    {
        // IGP feature point: grounding
        bool grounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundMask);
        if (!isGrounded && grounded)
        {
            AudioManager.PlayAudio("Land");
            isGrounded = true;
            animator.SetBool("isGrounded", isGrounded);
            _hasJumped = false;
            _currentMovementLerpSpeed = 100;
            dashLeft = _dashCount;
            _jumpLeft = _jumpCount;
        }
        else if (isGrounded && !grounded)
        {
            isGrounded = false;
            animator.SetBool("isGrounded", isGrounded);
            _timeLeftGrounded = Time.time;
            if (!_hasJumped)
            {
                _jumpLeft -= 1;
            }
        }
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundMask);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundMask);
        onWall = onRightWall || onLeftWall;
        _pushingLeftWall = onLeftWall && _inputVector.x < 0;
        _pushingRightWall = onRightWall && _inputVector.x > 0;
    }

    [Header("Walking")]
    [SerializeField] private float _walkSpeed = 8;
    [SerializeField] private float _acceleration = 4;
    [SerializeField] private float _currentMovementLerpSpeed = 100;

    private void HandleWalking()
    {
        _currentMovementLerpSpeed = Mathf.MoveTowards(_currentMovementLerpSpeed, 100, _wallJumpMovementLerp * Time.deltaTime);
        float acc = isGrounded ? _acceleration : _acceleration * 0.5f;
        if (_inputVector.x < 0)
        {
            if (rigid.velocity.x > 0)
            {
                _inputVector.x = 0;
            }
            _inputVector.x = Mathf.MoveTowards(_inputVector.x, -1, _acceleration * Time.deltaTime);
        } else if (_inputVector.x > 0)
        {
            if (rigid.velocity.x < 0)
            {
                _inputVector.x = 0;
            }
            _inputVector.x = Mathf.MoveTowards(_inputVector.x, 1, _acceleration * Time.deltaTime);
        }
        else
        {
            _inputVector.x = Mathf.MoveTowards(_inputVector.x, 0, 2 * _acceleration * Time.deltaTime);
        }
        Vector3 vel = new Vector3(_inputVector.x * _walkSpeed, rigid.velocity.y);
        rigid.velocity = Vector3.MoveTowards(rigid.velocity, vel, _currentMovementLerpSpeed * Time.deltaTime);
    }

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 15;
    [SerializeField] private float _fallMultiplier = 8;
    [SerializeField] private float _jumpVelocityFalloff = 12.5f;
    [SerializeField] private float _wallJumpMovementLerp = 25;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private int _jumpCount = 1;
    private int _jumpLeft;
    private bool _hasJumped;
    private float _timeLeftGrounded = -10;
    private float _timeLastWallJumped;

    private void HandleJumping()
    {
        // IGP feature point: jumping
        if (jumpPress) {
            // IGP feature point: implement “coyote time”
            bool freeJump = Time.time - _timeLeftGrounded < _coyoteTime;
            if (_jumpLeft > 0 || freeJump && !_hasJumped)
            {
                bool normalJump = true;
                if (freeJump)
                {
                    Debug.Log("Free Jump");
                }
                Vector2 jumpDirection = new Vector2(rigid.velocity.x, _jumpForce);
                if (!isGrounded && onWall)
                {
                    normalJump = false;
                    Debug.Log("Wall Jump");
                    AudioManager.PlayAudio("WallJump");
                    _timeLastWallJumped = Time.time;
                    _currentMovementLerpSpeed = _wallJumpMovementLerp;
                    jumpDirection.x = onLeftWall ? _jumpForce : -_jumpForce;
                    jumpDirection.x *= 0.5f;
                }
                else
                {
                    _currentMovementLerpSpeed = 100;
                }
                // IGP cool feature: Dash Jump Speed Boost (triggered by pressing jump while dashing, need to touch the ground in order to jump)
                if (_dashing && isGrounded || Time.time - _timeDashEnded < _coyoteTime)
                {
                    normalJump = false;
                    Debug.Log("Dash Jump");
                    StopDashing();
                    Vector2 dashBoost = _dashDirection * _dashSpeed;
                    if (onWall)
                    {
                        Debug.Log("Wall Dash Jump");
                        AudioManager.PlayAudio("WallDashJump");
                        dashBoost.x = onLeftWall ? _jumpForce : -_jumpForce;
                        dashBoost.x *= 0.25f;
                        dashBoost.y = _jumpForce * 0.1f;
                    }
                    else
                    {
                        if (dashBoost.y < 0)
                        {
                            Debug.Log("Super Dash Jump");
                            dashBoost.x *= 2;
                        }
                        AudioManager.PlayAudio("DashJump");
                    }
                    dashBoost.y = Mathf.Max(0.0f, dashBoost.y);
                    jumpDirection += dashBoost;
                }
                if (normalJump)
                {
                    AudioManager.PlayAudio("Jump");
                }
                OnJump(jumpDirection, freeJump);
            }
        }
        if (!isGrounded && rigid.velocity.y < _jumpVelocityFalloff || rigid.velocity.y > 0 && !(jumpKeyDown && _hasJumped))
        {
            rigid.velocity += _fallMultiplier * Physics.gravity.y * Vector2.up * Time.deltaTime;
        }
    }

    private void OnJump(Vector2 direction, bool noCostJump)
    {
        animator.SetTrigger("Jump");
        rigid.velocity = direction;
        _hasJumped = true;
        if (!noCostJump)
        {
            _jumpLeft -= 1;
        }
    }

    [Header("Dashing")]
    [SerializeField] private float _dashSpeed = 20;
    [SerializeField] private float _dashTime = 0.15f;
    [SerializeField] private int _dashCount = 1;
    private int _dashLeft;
    private int dashLeft
    {
        get
        {
            return _dashLeft;
        }

        set
        {
            _dashLeft = value;
            RefreshHair();
        }
    }
    private bool _dashing;
    private float _timeDashStarted;
    private float _timeDashEnded = -10;
    private Vector2 _dashDirection;
    private void HandleDashing()
    {
        // IGP feature point: dash
        if (dashPress && dashLeft > 0)
        {
            AudioManager.PlayAudio("Dash");
            animator.SetTrigger("Dash");
            _dashDirection = _inputVector.normalized;
            if(_dashDirection == Vector2.zero)
            {
                _dashDirection = facingLeft ? Vector2.left: Vector2.right;
            }
            _dashing = true;
            dashLeft -= 1;
            _timeDashStarted = Time.time;
            rigid.gravityScale = 0;
        }
        if (_dashing)
        {
            rigid.velocity = _dashDirection * _dashSpeed;
            if(_timeDashStarted + _dashTime <= Time.time)
            {
                StopDashing();
            }
        }
    }

    private void StopDashing() {
        _dashing = false;
        rigid.gravityScale = 1;
        _hasJumped = false;
        _timeDashEnded = Time.time;
        if (isGrounded)
        {
            dashLeft = _dashCount;
        }
        if (onWall)
        {
            _timeLeftGrounded = Time.time;
        }
        else
        {
            _timeLeftGrounded = Time.time - _coyoteTime;
        }
    }


    [Header("Sliding")]
    [SerializeField] private float slidingSpeed = 2;
    private bool _pushingLeftWall;
    private bool _pushingRightWall;
    private bool _wallSliding;
    private void HandleWallSlide()
    {
        // IGP cool feature: Wall Sliding
        bool isSliding = _pushingLeftWall || _pushingRightWall;
        if(isSliding && !_wallSliding && !isGrounded)
        {
            _wallSliding = true;
            animator.SetBool("sliding", _wallSliding);
            _jumpLeft = _jumpCount;
        }else if(!isSliding && _wallSliding)
        {
            _wallSliding = false;
            animator.SetBool("sliding", _wallSliding);
            _jumpLeft -= 1;
        }
        if (_wallSliding)
        {
            if (rigid.velocity.y < 0)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -slidingSpeed);
            }
        }
    }

    [Header("Setting")]
    public int maxHealth = 3;
    public float hurtCD;

    private float hurtTimer;
    private int _currentHealth;
    private int currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            UIManager4.RefreshHealth(_currentHealth);
        }
    }

    public void OnHurt()
    {
        if(hurtTimer == 0.0f)
        {
            if (currentHealth > 1)
            {
                AudioManager.PlayAudio("Hurt");
                hurtTimer = hurtCD;
                OnHurtStart();
            }
            else
            {
                AudioManager.PlayAudio("Death");
                hurtTimer = 999;
                OnDeathStart();
            }
            currentHealth -= 1;
        }
    }

    private void OnDeathStart()
    {
        isDead = true;
        Transform tempHair = hair.transform;
        int maxNest = 5;
        tempHair.gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        // IGP feature point: getChild
        while (tempHair.childCount != 0 && maxNest > 0)
        {
            tempHair = tempHair.transform.GetChild(0);
            tempHair.gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
            --maxNest;
        }
        animator.SetBool("Death", true);
    }

    // IGP feature point: animation events (called when death animation is finished)
    private void OnDeathEnd()
    {
        GameManager4.OnDead();
    }

    private void OnHurtStart()
    {
        CancelInvoke("OnBlink");
        Invoke("OnBlink", hurtCD * 0.2f);
    }

    private void OnBlink()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.0f);
        }
        Invoke("OnBlinkEnd", hurtCD * 0.1f);
    }

    private void OnBlinkEnd()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1.0f);
        }
        if (hurtTimer > 0.0f)
        {
            Invoke("OnBlink", hurtCD * 0.1f);
        }
    }

    private void HandleHair()
    {
        Transform tempHair = hair.transform;
        int maxNest = 5;
        while (tempHair.childCount != 0 && maxNest > 0)
        {
            tempHair = tempHair.transform.GetChild(0);
            tempHair.localPosition = -(rigid.velocity - Physics2D.gravity + (facingLeft ? Vector2.left : Vector2.right) * 10.0f).normalized * 0.2f;
            --maxNest;
        }
    }

    private void RefreshHair()
    {
        Color color = dashLeft > 0 ? Color.red : Color.cyan;
        Transform tempHair = hair.transform;
        int maxNest = 5;
        tempHair.gameObject.GetComponent<SpriteRenderer>().color = color;
        while (tempHair.childCount != 0 && maxNest > 0)
        {
            tempHair = tempHair.transform.GetChild(0);
            tempHair.gameObject.GetComponent<SpriteRenderer>().color = color;
            --maxNest;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isDead)
        {
            if (collision.gameObject.CompareTag("Item"))
            {
                Item4 item = collision.gameObject.GetComponent<Item4>();
                if (item.pickUpType == Item4.Item4Type.Coin)
                {
                    GameManager4.instance.score += 100;
                    AudioManager.PlayAudio("Coin");
                }
                else if (item.pickUpType == Item4.Item4Type.Heart)
                {
                    if (currentHealth < 5)
                    {
                        currentHealth += 1;
                    }
                    else
                    {
                        GameManager4.instance.score += 500;
                    }
                    AudioManager.PlayAudio("Heart");
                }
                Destroy(item.gameObject);
            }else if (collision.gameObject.CompareTag("TrapPlatform"))
            {
                if (!_dashing && !(Time.time - _timeDashEnded < _coyoteTime))
                {
                    OnHurt();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (!isDead)
        {
            if (collision.gameObject.CompareTag("TrapPlatform"))
            {
                if (!_dashing && !(Time.time - _timeDashEnded < _coyoteTime))
                {
                    OnHurt();
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = debugCollisionColor;
        Vector2[] position = new Vector2[] { bottomOffset, rightOffset, leftOffset };
        for(int i = 0; i < position.Length; ++i)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + position[i], collisionRadius);
        }
    }
}
