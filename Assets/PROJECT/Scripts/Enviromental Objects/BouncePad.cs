
using System.Collections;
using UnityEngine;

public class BouncePad : EnviromentalAid, INeedPlayerRefs
{
    public float jumpHeight = 10f;
    public Animator anim;
    private Jump jump;
    [SerializeField] float duration;
    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.TryGetComponent(out GrindController GC)) return;

        if (playerRB != null)
        {
            if (grindController != null && PlayerMovement.moveState is MoveState.OnRail || PlayerMovement.jumpState == JumpState.Stomping)
            {

                GC.ExitRails();
                //Grinding+Jumppad == BEEG JUMP
                StartCoroutine(EnhancedBounce(duration));

            }
            else
            {
                StartCoroutine(RegularBounce(duration));
            }
        }
    }

    IEnumerator RegularBounce(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float force =( Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y) * playerRB.mass * 500)) * Time.deltaTime;
            playerRB.AddForce(Vector3.up * force, ForceMode.Impulse);
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator EnhancedBounce(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float force = (Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y) * playerRB.mass * 500))*Time.deltaTime;
            Vector3 forwardDirection = playerRB.transform.forward;
            playerRB.AddForce(Vector3.up * force + forwardDirection * force * 0.6f, ForceMode.Impulse);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
