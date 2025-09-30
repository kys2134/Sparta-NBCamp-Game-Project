using Backend.Util.Data.StatusDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class EnemyStatus : Status
    {
        [field: SerializeField] public StatusBossData BossStatus { get; private set; }
    }
}
