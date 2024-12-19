public class MoveObjective : GameObjective
{
    public MoveObjective()
    {
        Name = "Move Objective";
        Description = "Walk around using WASD";
    }

    public override void UpdateObjective()
    {
        if (PlayerMovement.Instance.Moving)
            CompletedObjective();
    }
}