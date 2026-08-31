using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyStateManager : EntityStateManager<Enemy> {
    //反射创建状态列表
    [ClassTypeName(typeof(EnemyState))] public string[] states;

    protected override List<EntityState<Enemy>> GetStateList(){
        var list = EntityState<Enemy>.CreateStatesListAfterReflection(states);
        return list;
    }
    
}
