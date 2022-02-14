using GameAnalyticsSDK;

namespace Runtime.Services.AnalyticsService.Impls
{
    public class AnalyticsService : IAnalyticsService
    {
        public void SendRequest(string message)
        {
            Amplitude.Instance.logEvent(message);
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, message);
        }
    }
}