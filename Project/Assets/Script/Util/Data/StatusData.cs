using UnityEngine;

namespace Backend.Util.Data
{
    public class StatusData : ScriptableObject
    {
        [field: Header("Default Settings")]
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float HealthPoint { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
    }
}
