public class AimObjective : GameObjective
{
    private int _amountOfTimes;
    private int _aimCount = 0;

    public AimObjective(int amountOfTimes = 1)
    {
        Name = "Aim Objective";
        Description = "Aim your rubber band by holding space and using WASD";

        this._amountOfTimes = amountOfTimes;
    }

    public override void StartObjective()
    {
        PlayerAttack.OnAttackAim += PlayerAttack_OnAttackAim;
    }

    private void PlayerAttack_OnAttackAim()
    {
        _aimCount++;
        if (_aimCount >= _amountOfTimes)
        {
            PlayerAttack.OnAttackAim -= PlayerAttack_OnAttackAim;
            CompletedObjective();
        }
    }
} 