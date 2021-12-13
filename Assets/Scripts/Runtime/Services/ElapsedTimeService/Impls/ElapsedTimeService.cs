namespace Runtime.Services.ElapsedTimeService.Impls
{
    public class ElapsedTimeService : IElapsedTimeService
    {
        private float _elapsedTime;
        public ElapsedTimeService()
        {
            _elapsedTime = 0;
        }

        public float GetElapsedTime()
        {
            return _elapsedTime;
        }

        public void SetElapsedTime(float value)
        {
            _elapsedTime = value;
        }
    }
}