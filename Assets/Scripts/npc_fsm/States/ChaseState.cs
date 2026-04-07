using UnityEngine;
    public class ChaseState : IState {
        private Transform npc;
        private Transform player;
        
        /// <summary>
        /// 构造函数初始化，创建状态时就提供玩家和npc的引用
        /// </summary>
        public ChaseState(Transform npc, Transform player){
            this.npc = npc;
            this.player = player;
        }
        
        public override void OnEnter(){
            //Debug.Log("npc enter chase");
        }

        public override void OnStep(){
            //Debug.Log("npc chase on step");
        }

        public override void OnExit(){
            //Debug.Log("npc exit chase");
        }
    }