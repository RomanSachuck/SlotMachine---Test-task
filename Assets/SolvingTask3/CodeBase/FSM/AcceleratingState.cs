using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using SolvingTask3.CodeBase.Constants;
using UnityEngine;

namespace SolvingTask3.CodeBase.FSM
{
    [State("Accelerating")]
    public class AcceleratingState : FSMState
    {
        private float _accelerationTime = 3f;
        private float _maxSpeed = 1500f;
        private float _minSpeed = 50f;

        [Enter]
        public void Enter()
        {
            Log.Info("FSM: Вошли в Accelerating");

            Model.Set(SlotModelKeys.CanStartSpin, false);
            Model.Set(SlotModelKeys.CanStopSpin, false);
            Model.Set(SlotModelKeys.CurrentSpeed, _minSpeed);

            Settings.Invoke(EventNames.StartSpinning);
        }

        [One(3f)]
        public void EnableStop()
        {
            Log.Info("FSM: Теперь можно останавливать");
            
            Model.Set(SlotModelKeys.CanStopSpin, true);
        }

        [Loop(0.1f)]
        public void UpdateSpeed()
        {
            float currentSpeed = Model.GetFloat(SlotModelKeys.CurrentSpeed);
            
            float newSpeed = Mathf.Lerp(currentSpeed, _maxSpeed, 0.1f);
            Model.Set(SlotModelKeys.CurrentSpeed, newSpeed);
            
            if (Mathf.Abs(newSpeed - _maxSpeed) < 1f)
            {
                Model.Set(SlotModelKeys.CurrentSpeed, _maxSpeed);
                Parent.Change("Spinning");
            }
        }

        [Bind("OnBtn")]
        public void OnStopPressed(string btnName)
        {
            if (btnName != ButtonNames.StopSpinButton)
                return;
            
            bool canStop = Settings.Model.GetBool(SlotModelKeys.CanStopSpin);
            
            if (canStop)
            {
                Log.Info("FSM: Получен сигнал Stop, переходим в Stopping");
                
                Parent.Change("Stopping");
            }
            else
            {
                Log.Warn("FSM: Слишком рано для остановки!");
            }
        }
    }
}