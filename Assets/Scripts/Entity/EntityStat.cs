using UnityEngine;
/// <summary>
/// 泛型抽象类，定义实体的数据属性；继承自ScriptableObject，便于在编辑器内创建和管理数据资产
/// </summary>
/// <typeparam name="T">继承自ScriptableObject的具体属性类型(player和enemy是分开配置的)</typeparam>
public abstract class EntityStat<T> : ScriptableObject where T : ScriptableObject {

}

