using UnityEngine;

public class Ejector : EnviromentalAid
{
    [SerializeField] int damage = 1;
    [SerializeField] float ejectSpeed = 140f;
    [SerializeField] float upwardForce = 60f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IDamageable damageable) && playerRB != null)
        {
            damageable.TakeDamage(damage);
            grindController.ExitRails();
            BounceOff();
        }
    }

    void BounceOff()
    {
        int direction = Random.Range(0, 2);
        float force = Mathf.Sqrt(ejectSpeed * Mathf.Abs(Physics.gravity.y) * playerRB.mass);
        Vector3 chosenDirection;

        if (direction == 0)
        {
            chosenDirection = (playerRB.transform.right * force) + (Vector3.up * upwardForce);
        }
        else
        {
            chosenDirection = (-playerRB.transform.right * force) + (Vector3.up * upwardForce);
        }

        playerRB.AddForce(chosenDirection, ForceMode.Impulse);
    }
}