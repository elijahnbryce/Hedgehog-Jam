public class ShootObjective : GameObjective
{
    public ShootObjective(int amountOfTimes = 1)
    {
        Name = "Shoot Objective";
        Description = "Shoot by releasing space while aiming";

        this.ProgressCompletion = amountOfTimes;
    }

    public override void StartObjective()
    {
        PlayerAttack.OnAttack += PlayerAttack_OnAttack;
    }

    private void PlayerAttack_OnAttack()
    {
        this.Progress++;
        if (this.Progress >= this.ProgressCompletion)
        {
            PlayerAttack.OnAttackAim -= PlayerAttack_OnAttack;
            CompletedObjective();
        }
    }
}