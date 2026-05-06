namespace Mapper.Application.Features.Analytics;

public record MotionAnomalyInput(
    Guid CameraMarkId,
    string CameraTitle,
    int CurrentAlertCount,
    IReadOnlyCollection<int> BaselineBuckets);

public record MotionAnomalyResult(
    Guid CameraMarkId,
    string CameraTitle,
    int CurrentAlertCount,
    double BaselineAverageAlertCount,
    double ZScore,
    double ActivityRatio,
    bool IsAnomaly,
    string Reason);

public static class MotionAnomalyDetector
{
    private const int MinimumCurrentAlertCount = 3;
    private const double MinimumZScore = 2.0;
    private const double MinimumActivityRatio = 2.5;

    public static MotionAnomalyResult Analyze(MotionAnomalyInput input)
    {
        var baseline = input.BaselineBuckets.Count == 0 ? new[] { 0 } : input.BaselineBuckets;
        var average = baseline.Average();
        var variance = baseline.Average(value => Math.Pow(value - average, 2));
        var standardDeviation = Math.Sqrt(variance);
        var zScore = standardDeviation == 0
            ? input.CurrentAlertCount - average
            : (input.CurrentAlertCount - average) / standardDeviation;
        var activityRatio = input.CurrentAlertCount / Math.Max(average, 1.0);

        var isAnomaly = input.CurrentAlertCount >= MinimumCurrentAlertCount
            && zScore >= MinimumZScore
            && activityRatio >= MinimumActivityRatio;

        var reason = isAnomaly
            ? $"Current motion activity is {activityRatio:F1}x above the baseline."
            : "Motion activity is within the expected baseline.";

        return new MotionAnomalyResult(
            input.CameraMarkId,
            input.CameraTitle,
            input.CurrentAlertCount,
            Math.Round(average, 2),
            Math.Round(zScore, 2),
            Math.Round(activityRatio, 2),
            isAnomaly,
            reason);
    }
}
