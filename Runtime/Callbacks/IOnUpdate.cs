namespace Baracuda.Utilities.Callbacks
{
    public interface IOnUpdate
    {
        public void OnUpdate(float deltaTime);
    }

    public interface IOnLateUpdate
    {
        public void OnLateUpdate(float deltaTime);
    }

    public interface IOnFixedUpdate
    {
        public void OnFixedUpdate(float fixedDeltaTime);
    }
}