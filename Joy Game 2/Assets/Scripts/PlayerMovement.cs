using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Score")]
    [SerializeField] private int score = 0;
    public int Score => score;

    [Header("State")]
    public bool stop;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private Coroutine stopRoutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // PANGGIL DARI OBJEK: ngurangin poin
    public void ReduceScore(int amount)
    {
        if (amount <= 0) return;
        score = Mathf.Max(0, score - amount);
        print(score);
        // nanti bisa tambahin efek: sfx, anim, popup angka, dll
    }

    // PANGGIL DARI OBJEK: stop selama X detik, abis itu stop=false lagi
    public void StopForSeconds(float seconds)
    {
        if (seconds <= 0f) return;
        
        if (stopRoutine != null) StopCoroutine(stopRoutine);
        stopRoutine = StartCoroutine(StopRoutine(seconds));
    }

    private IEnumerator StopRoutine(float seconds)
    {
        StopPlayer();
        yield return new WaitForSeconds(seconds); // timer stop [web:604]
        ResumePlayer();
        stopRoutine = null;
    }

    public void StopPlayer()
    {
        stop = true;
        horizontalInput = 0f;
        body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
        anim.SetBool("run", false);
        anim.SetBool("stop", true);
    }

    public void ResumePlayer()
    {
        stop = false;
        anim.SetBool("stop", false);
    }

    private void Update()
    {
        if (stop) return;

        // INPUT kamu (keyboard + controller) tetap seperti sebelumnya
        float x = 0f;
        var gamepad = Gamepad.current;
        if (gamepad != null) x = gamepad.leftStick.ReadValue().x;

        if (Mathf.Abs(x) < 0.1f)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1f;
        }
        if (stop) {
            anim.SetBool("stop", true);
        }else {
            anim.SetBool("stop", false);
        }

        horizontalInput = x;

        if (horizontalInput > 0.01f) transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f) transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else body.gravityScale = 7;

            bool jump = Keyboard.current.spaceKey.isPressed ||
                        (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);

            if (jump) Jump();
        }
        else wallJumpCooldown += Time.deltaTime;
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            // anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            if (Mathf.Abs(horizontalInput) < 0.01f)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0,
            new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return hit.collider != null;
    }
}
