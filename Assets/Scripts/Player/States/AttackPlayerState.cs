using UnityEngine;
    public class AttackPlayerState : PlayerState {
        
        
        protected override void OnEnter(Player player){
            player.comboCount = 0;//初始化为0
            
        }
       
        protected override void OnStep(Player player){
            
        }
        protected override void OnExit(Player player){
            
        }
        public override void OnContact(Player entity, Collider other){
            
        }
    }
