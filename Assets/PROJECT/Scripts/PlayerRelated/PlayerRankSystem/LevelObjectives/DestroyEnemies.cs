public class DestroyEnemies : Objective
{
    EnemyBase[] enemies;

    private void Start()
    {
        enemies = FindObjectsOfType<EnemyBase>();
        Current = 0;
    }

    public override void SetTotal(bool customTarget, int targetNumber)
    {
        Total = customTarget ? targetNumber : enemies.Length;
    }

    public override bool CheckCompletion()
    {
        Current = 0;
        for (int i = 0; i < enemies.Length; i++)
        {

            if (!enemies[i].gameObject.activeSelf)
            {
                Current++;
            }
        }
        return Current >= Total;
    }
}