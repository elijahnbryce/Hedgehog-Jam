using UnityEngine;

public class PickupObjective : GameObjective
{
    public PickupObjective(int amountOfTimes = 1)
    {
        Name = "Pickup Objective";
        Description = "Pickup your rubber band after firing it";
        this.ProgressCompletion = amountOfTimes;
    }

    public override void StartObjective()
    {
        PlayerAttack.OnPickup += PlayerAttack_OnPickup;
    }

    private void PlayerAttack_OnPickup()
    {
        Progress++;
        Debug.Log($"{Progress}/{this.ProgressCompletion}");
        if (Progress >= this.ProgressCompletion)
        {
            PlayerAttack.OnPickup -= PlayerAttack_OnPickup;
            CompletedObjective();
        }
    }
}