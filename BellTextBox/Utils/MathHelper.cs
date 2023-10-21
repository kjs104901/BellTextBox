namespace Bell.Utils;

public static class MathHelper
{
    private const float FloatTolerance = 0.1f;

    public static bool IsSame(float r, float l) => Math.Abs(r - l) < FloatTolerance;
    public static bool IsNotSame(float r, float l) => !IsSame(r, l);
}