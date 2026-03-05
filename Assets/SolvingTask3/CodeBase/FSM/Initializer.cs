using AxGrid;
using AxGrid.Base;
using UnityEngine;

namespace SolvingTask3.CodeBase.FSM
{
    public class Initializer : MonoBehaviourExt
    {
        [OnAwake]
        private void InitFsm()
        {
            Settings.Fsm = new AxGrid.FSM.FSM();
            
            Settings.Fsm.Add(new IdleState());
            Settings.Fsm.Add(new AcceleratingState());
            Settings.Fsm.Add(new SpinningState());
            Settings.Fsm.Add(new StoppingState());
        }
    
        [OnStart]
        private void StartFsm()
        {
            Settings.Fsm.Start("Idle");
        }
    
        [OnUpdate]
        public void UpdateFsm()
        {
            Settings.Fsm.Update(Time.deltaTime);
        }
    }
}