using UnityEngine;
using VInspector;

public class RankSystem : MonoBehaviour
{
    [field: SerializeField] public SerializedDictionary<FinalRank, float> LevelRanks { get; private set; } = new()
    {
        { FinalRank.S , 5 },
        { FinalRank.A , 10 },
        { FinalRank.B , 15 }
    };

    [field: SerializeField] public SerializedDictionary<FinalRank, Color> RankColors { get; private set; } = new()
    {
        { FinalRank.S , Color.blue },
        { FinalRank.A , Color.green },
        { FinalRank.B , Color.red },
        { FinalRank.Unranked , Color.gray }
    };

    public FinalRank GetRank(float completionTime)
    {
        foreach (var rank in LevelRanks)
        {
            if (rank.Value < completionTime)
            {
                continue;
            }
            else
            {
                return rank.Key;
            }
        }
        return FinalRank.Unranked;
    }
}

public enum FinalRank
{
    Unranked,
    S,
    A,
    B
}