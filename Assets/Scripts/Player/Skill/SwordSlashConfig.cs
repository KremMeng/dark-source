using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[CreateAssetMenu(fileName = "SwordSlashConfig",menuName = "ScriptableObject/刀光配置")]
public class SwordSlashConfig : ScriptableObject {
    [Header("Prefab")] 
    public GameObject slashPrefab;
    [Header("位置(local)")] 
    public Vector3 localPosition;
    public Vector3 localEulerRotation = Vector3.Zero;
    public Vector3 localScale = Vector3.One;

}
