namespace health_app_backend.Helpers;

public class RecommendationHelper
{
    public static float CalculateRecommendedValue(int minAge, int maxAge, string gender, string healthDataType, float averageValue)
    {
        float recommendedValue = averageValue;

        // Set boundaries based on health guidelines
        float minBound = 0;
        float maxBound = float.MaxValue;

        switch (healthDataType.ToLower())
        {
            case "steps":
                minBound = 5000;   // Minimum recommended steps
                maxBound = 15000;  // Maximum recommended steps
                recommendedValue = GetAdjustedSteps(minAge, maxAge, gender, averageValue);
                break;
            case "calories":
                minBound = 200;    // Minimum calories to burn additionally
                maxBound = 800;    // Maximum additional calories for a typical day
                recommendedValue = GetAdjustedCalories(minAge, maxAge, gender, averageValue);
                break;
            case "resting_heart_rate":
                minBound = 45;     // Healthy minimum heart rate
                maxBound = 80;     // Healthy maximum heart rate target
                recommendedValue = GetAdjustedHeartRate(minAge, maxAge, gender, averageValue);
                break;
            default:
                recommendedValue = averageValue; // Default fallback
                break;
        }

        // Clamp the recommended value within min and max bounds
        return Math.Clamp(recommendedValue, minBound, maxBound);
    }

    // Helper methods for specific health data types
    private static float GetAdjustedSteps(int minAge, int maxAge, string gender, float averageValue)
    {
        float adjustmentFactor = 1.0f;

        if (minAge <= 30)
            adjustmentFactor = 1.1f; // Increase for younger adults
        else if (minAge > 50)
            adjustmentFactor = 0.9f; // Decrease for older adults

        // Further adjustments by gender, if needed
        if (gender == "Female")
            adjustmentFactor *= 1.05f;

        return averageValue * adjustmentFactor;
    }

    private static float GetAdjustedCalories(int minAge, int maxAge, string gender, float averageValue)
    {
        float adjustmentFactor = 1.0f;

        if (minAge <= 30)
            adjustmentFactor = 1.2f; // Younger adults may have higher targets
        else if (minAge > 50)
            adjustmentFactor = 0.85f; // Older adults with a slightly lower adjustment

        if (gender == "Male")
            adjustmentFactor *= 1.1f;

        return averageValue * adjustmentFactor;
    }

    private static float GetAdjustedHeartRate(int minAge, int maxAge, string gender, float averageValue)
    {
        float targetAdjustment = -5f; // Target lowering resting heart rate

        if (minAge > 50)
            targetAdjustment += 5f; // Slight increase for older adults

        return averageValue + targetAdjustment;
    }
}