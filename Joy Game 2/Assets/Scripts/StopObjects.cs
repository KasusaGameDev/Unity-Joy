using UnityEngine;

public class StopperObject2D : MonoBehaviour
{
    [SerializeField] private bool stop = true;
    [SerializeField] private float timer = 1.0f;      // lama stop (detik)
    [SerializeField] private int pointReduce = 5;     // pengurang poin

    [Header("Animation")]
    [SerializeField] private Animator anim;           // drag Animator object ini di Inspector
    [SerializeField] private string triggerName = "Hit"; // nama Trigger di Animator
    [SerializeField] private float destroyDelay = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerMovement>();
        if (!player) return;

        if (pointReduce > 0)
            player.ReduceScore(pointReduce);

        if (stop)
            player.StopForSeconds(timer);

        if (anim != null && !string.IsNullOrEmpty(triggerName))
            anim.SetTrigger(triggerName); // nembak trigger animasi [web:615]
    }

    public void DestroyObj()
    {
        if (destroyDelay > 0f) Destroy(gameObject, destroyDelay);
        else Destroy(gameObject);
    }
}
