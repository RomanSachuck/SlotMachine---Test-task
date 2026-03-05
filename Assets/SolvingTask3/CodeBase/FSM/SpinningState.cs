using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using SolvingTask3.CodeBase.Constants;

namespace SolvingTask3.CodeBase.FSM
{
    [State("Spinning")]
    public class SpinningState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Log.Info("FSM: Вошли в Spinning (постоянное вращение)");
        }
    
        [Bind("OnBtn")]
        private void OnStopPressed(string btnName)
        {
            if (btnName != ButtonNames.StopSpinButton)
                return;
            
            bool canStop = Model.GetBool(SlotModelKeys.CanStopSpin);
            
            if (canStop)
            {
                Log.Info("FSM: Получен сигнал Stop, переходим в Stopping");
                
                Parent.Change("Stopping");
            }
        }
    }
}