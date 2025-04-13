public class CollectItems : Objective
{
    Item[] items;

    private void Start()
    {
        items = FindObjectsOfType<Item>();
        Current = 0;
    }

    public override void SetTotal(bool customTarget, int targetNumber)
    {
        Total = customTarget ? targetNumber : items.Length;
    }

    public override bool CheckCompletion()
    {
        Current = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Collected)
            {
                Current++;
            }
        }
        return Current >= Total;
    }
}