using UnityEngine;


public abstract class EntityBase : MonoBehaviour {
    
    
}
//泛型抽象类，给T增加一个泛型约束
public abstract class Entity<T> : EntityBase where T : Entity<T> {
    
}
