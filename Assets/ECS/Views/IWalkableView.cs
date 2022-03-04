namespace ECS.Views
{
    public interface IWalkableView
    {
        bool IsCarrying();
        void SetIdleAnimation();
        void SetCarryAnimation();
        void SetWalkAnimation();
        void SetCarryingWalkAnimation();
    }
}