using System;
using System.Collections.Generic;

//一个抽象的状态基类
public abstract class EntityState<T> where T : Entity<T> {

    //反射机制实例化状态，不具体区分实例，返回笼统的EntityState<T>类型
    public static EntityState<T> CreateFromStatesNameString(string typeName){
        var type = System.Type.GetType(typeName);//typeName来自于扫描的各个状态 ？
        return (EntityState<T>)Activator.CreateInstance(type);//object强制转换
    }

    //把获取的实例加入list
    public static List<EntityState<T>> CreateListFromStatesArray(string[] arr){
        List<EntityState<T>> list = new List<EntityState<T>>();
        
        foreach (var typeName in arr) {
            list.Add(CreateFromStatesNameString(typeName));
        }
        return list;

    }
}
