using UnityEngine;

public class ITimer0
{
   public float duration; //时间间隔
   public float eclipseTime; //流逝的时间

   //枚举状态，表示默认状态、计时器打开、计时结束
   public enum STATE {
      IDLE,
      RUN,
      FINISHED
   };

   public STATE state;

   //推进时间，在一定时间间隔后结束状态
   public void Tick()
   {
      switch (state) {
         case STATE.IDLE:
            break;
         case STATE.RUN:
            eclipseTime += Time.deltaTime;
            if (eclipseTime >= duration) {
               state = STATE.FINISHED;
               break;
            }
            else {
               break;}
         case STATE.FINISHED:
            state = STATE.IDLE;
            break;
         default:
            Debug.Log("timer error!");
            break;
      }
   }
   
}

