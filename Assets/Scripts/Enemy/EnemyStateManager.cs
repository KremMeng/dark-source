using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Enemy))]
public class EnemyStateManager : EntityStateManager<Enemy> {
    //自定义Attribute -> PropertyDrawer获取string存到states里 -> 根据string创建状态类型实例 ->加入到状态字典里
    //把这个 Type 信息"挂"在 states 数组字段上，数组的每个元素会分别调用对应的 PropertyDrawer
    //此时StateManager的编辑器页面就绘制出了一排具体的状态名列表
    [ClassTypeName(typeof(EnemyState))] 
    public string[] states; 

    //后续调用GetStateList的时候再加入到状态字典里
    protected override List<EntityState<Enemy>> GetStateList(){
        var list = EnemyState.CreateStatesListAfterReflection(states);
        return list;
    }
}
