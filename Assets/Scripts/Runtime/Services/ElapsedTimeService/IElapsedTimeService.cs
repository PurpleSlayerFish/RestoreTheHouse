namespace Runtime.Services.ElapsedTimeService
{
    public interface IElapsedTimeService
    {
        float GetElapsedTime();
        
        void SetElapsedTime(float value);
    }
}