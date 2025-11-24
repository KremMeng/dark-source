using UnityEngine;

//数值的调度员（如冰面上和普通地面的摩擦力等不一样/游戏难度不同，需要切换）
public abstract class EntityStatManager<T> : MonoBehaviour where T : EntityStat<T> {
    public T[] stat;
    public T current { get; protected set; }

    protected void Start(){
        //如果不为空，初始化为stat数组的第一个
        if(stat.Length > 0) {
            current = stat[0];
        }
    }

    public virtual void Change(int to){
        //确保索引合法
        if (to >= 0 && to < stat.Length) {
            //如果待切换的不等于当前的，就进行切换
            if (current != stat[to]) {
                current = stat[to];
            }
        }
    }
}
