using UnityEngine;

public class ITimer
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

   public void Tick()
   {
      if (state == STATE.IDLE)
      {
         
      }else if (state == STATE.RUN) {
         eclipseTime += Time.deltaTime;
         if (eclipseTime >= duration) {
            state = STATE.FINISHED;
         }
      }else if (state == STATE.FINISHED) {
         
      }
      else {
         
         Debug.Log("timer error!");
      }
   }

   public void TimerGo(){
      eclipseTime = 0;
      state = STATE.RUN; //开启计时状态
   }

}
