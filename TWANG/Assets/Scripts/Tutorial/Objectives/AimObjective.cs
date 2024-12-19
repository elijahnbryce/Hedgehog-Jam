public class AimObjective : GameObjective
{
    public AimObjective(int amountOfTimes = 1)
    {
        Name = "Aim Objective";
        Description = "Aim your rubber band by holding space and using WASD";
        this.ProgressCompletion = amountOfTimes;
    }

    public override void StartObjective()
    {
        PlayerAttack.OnAttackAim += PlayerAttack_OnAttackAim;
    }

    private void PlayerAttack_OnAttackAim()
    {
        Progress++;
        if (Progress >= this.ProgressCompletion)
        {
            PlayerAttack.OnAttackAim -= PlayerAttack_OnAttackAim;
            CompletedObjective();
        }
    }
} 