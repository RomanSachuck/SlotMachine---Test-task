using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using SolvingTask3.CodeBase.Constants;
using UnityEngine;

namespace SolvingTask3.CodeBase.FSM
{
    [State("Stopping")]
    public class StoppingState : FSMState
    {
        private float _decelerationTime = 1.5f;
        private float _startSpeed;
        private float _startTime;
    
        [Enter]
        private void Enter()
        {
            Log.Info("FSM: Вошли в Stopping");
        
            _startSpeed = Model.GetFloat(SlotModelKeys.CurrentSpeed);
            _startTime = Time.time;
        
            Model.Set(SlotModelKeys.CanStopSpin, false);
        }
    
        [Loop(0.1f)]
        private void UpdateStopping()
        {
            float elapsed = Time.time - _startTime;
            float t = elapsed / _decelerationTime;
        
            if (t >= 1f)
            {
                Model.Set(SlotModelKeys.CurrentSpeed, 0f);
                
                Settings.Invoke(EventNames.FinalAlignmentStarted);
                
                return;
            }
            
            t = 1f - (1f - t) * (1f - t);
            float newSpeed = Mathf.Lerp(_startSpeed, 0f, t);
            Model.Set(SlotModelKeys.CurrentSpeed, newSpeed);
        }

        [Bind(EventNames.SlotMachineStopped)]
        private void OnSlotMachineStopped(ItemRarity rarity)
        {
            Log.Info($"FSM: Выпавшая редкость - {rarity}");
            
            Parent.Change("Idle");
        }
    }
}