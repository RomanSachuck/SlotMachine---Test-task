using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using SolvingTask3.CodeBase.Constants;

namespace SolvingTask3.CodeBase.FSM
{
    [State("Idle")]
    public class IdleState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Log.Info("FSM: Вошли в Idle");
            
            Model.Set(SlotModelKeys.CanStartSpin, true);
            Model.Set(SlotModelKeys.CurrentSpeed, 0);
            Model.Set(SlotModelKeys.CanStopSpin, false);
        }

        [Bind("OnBtn")]
        private void OnStartPressed(string btnName)
        {
            if (btnName != ButtonNames.StartSpinButton)
                return;
            
            Log.Info("FSM: Получен сигнал Start, переходим в Accelerating");
            
            Parent.Change("Accelerating");
        }
    }
}