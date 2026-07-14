using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GabNetStats.Tests;

[TestClass]
public sealed class SpeedUtilsTests
{
    [TestMethod]
    public void ComputeSpeedUsesBinaryByteUnits()
    {
        string unit = string.Empty;

        double value = SpeedUtils.ComputeSpeed(1536, ref unit, 1);

        Assert.AreEqual(1.5, value);
        Assert.AreEqual(SpeedUtils.SpeedUnitsByte.KiB, unit);
    }

    [TestMethod]
    public void ComputeSpeedUsesDecimalBitUnits()
    {
        string unit = string.Empty;

        double value = SpeedUtils.ComputeSpeed(1_500_000, ref unit, 2);

        Assert.AreEqual(1.5, value);
        Assert.AreEqual(SpeedUtils.SpeedUnitsBit.Mbit, unit);
    }
}
