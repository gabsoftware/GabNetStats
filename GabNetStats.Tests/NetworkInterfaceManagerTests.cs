using GabNetStats.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GabNetStats.Tests;

[TestClass]
public sealed class NetworkInterfaceManagerTests
{
    [TestMethod]
    public void EnabledInterfaceMatchingUsesExactTokens()
    {
        string originalEnabledList = Settings.Default.EnabledInterfaceMACList;

        try
        {
            Settings.Default.EnabledInterfaceMACList = "ABCDEF";
            NetworkInterfaceManager.RefreshEnabledInterfacesCache();

            NetworkInterfaceManager.InterfaceStatisticsChangeResult result =
                NetworkInterfaceManager.EnableStatisticsForInterface("ABC", false, false);

            Assert.AreEqual(NetworkInterfaceManager.InterfaceStatisticsChangeResult.NoChange, result);
            Assert.IsTrue(NetworkInterfaceManager.IsInterfaceEnabled("ABCDEF"));
            Assert.IsFalse(NetworkInterfaceManager.IsInterfaceEnabled("ABC"));
        }
        finally
        {
            Settings.Default.EnabledInterfaceMACList = originalEnabledList;
            NetworkInterfaceManager.RefreshEnabledInterfacesCache();
        }
    }

    [TestMethod]
    public void KnownInterfaceMatchingUsesExactTokens()
    {
        string originalKnownList = Settings.Default.KnownInterfaceMACList;

        try
        {
            Settings.Default.KnownInterfaceMACList = "ABCDEF";

            bool wasKnown = NetworkInterfaceManager.AddToKnownInterface("ABC", false);

            Assert.IsFalse(wasKnown);
            CollectionAssert.AreEquivalent(
                new[] { "ABCDEF", "ABC" },
                Settings.Default.KnownInterfaceMACList.Split(';', StringSplitOptions.RemoveEmptyEntries));
        }
        finally
        {
            Settings.Default.KnownInterfaceMACList = originalKnownList;
        }
    }
}
