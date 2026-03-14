using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using GabNetStats.Properties;

namespace GabNetStats
{
    internal class NetworkInterfaceManager
    {
        // ── Static state (lock-free, shared across threads) ──────────────────

        private static HashSet<string> enabledInterfaceMacs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly string[] hiddenInterfaceKeywords = new[]
        {
            "virtual",
            "hyper-v",
            "vmware",
            "loopback",
            "kernel debug",
            "container",
            "wi-fi direct",
            "bluetooth device",
            "qos",
            "wfp",
            "wan miniport",
            "filter"
        };

        // ── Instance state ────────────────────────────────────────────────────

        internal List<TrackedInterface> selectedInterfaces = new List<TrackedInterface>();
        internal IPGlobalProperties properties;
        internal IPGlobalStatistics ipv4stat;
        internal IPGlobalStatistics ipv6stat;
        internal int nbNIC       = 0;
        internal int nNICRefresh = 10000; //time interval for refreshing the NIC list (10s by default)

        // ── Nested type ───────────────────────────────────────────────────────

        internal sealed class TrackedInterface
        {
            public TrackedInterface(NetworkInterface networkInterface, string macAddress)
            {
                Interface = networkInterface;
                MacAddress = macAddress;
            }

            public NetworkInterface Interface { get; }
            public string MacAddress { get; }
            public bool IsEnabled => NetworkInterfaceManager.IsInterfaceEnabled(MacAddress);
        }

        // ── Static methods ────────────────────────────────────────────────────

        internal static void RefreshEnabledInterfacesCache()
        {
            HashSet<string> newSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string enabledList = Settings.Default.EnabledInterfaceMACList;

            if (!String.IsNullOrWhiteSpace(enabledList) && !String.Equals(enabledList, "TOSET", StringComparison.OrdinalIgnoreCase))
            {
                string[] entries = enabledList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    newSet.Add(entry);
                }
            }

            Volatile.Write(ref enabledInterfaceMacs, newSet);
        }

        internal static bool IsInterfaceEnabled(string mac)
        {
            if (String.IsNullOrEmpty(mac))
            {
                return false;
            }

            HashSet<string> snapshot = Volatile.Read(ref enabledInterfaceMacs);
            return snapshot.Contains(mac);
        }

        internal static bool ShouldDisplayInterface(NetworkInterface netInterface)
        {
            if (netInterface == null)
            {
                return false;
            }

            bool showDisconnected = Settings.Default.ShowDisconnectedInterfaces;
            if (!showDisconnected && netInterface.OperationalStatus != OperationalStatus.Up)
            {
                return false;
            }

            if (!HasValidPhysicalAddress(netInterface))
            {
                return false;
            }

            switch (netInterface.NetworkInterfaceType)
            {
                case NetworkInterfaceType.Ethernet:
                case NetworkInterfaceType.Ethernet3Megabit:
                case NetworkInterfaceType.FastEthernetFx:
                case NetworkInterfaceType.FastEthernetT:
                case NetworkInterfaceType.GigabitEthernet:
                case NetworkInterfaceType.Wireless80211:
                case NetworkInterfaceType.Tunnel:
                    break;
                default:
                    return false;
            }

            string description = netInterface.Description ?? String.Empty;
            string name = netInterface.Name ?? String.Empty;
            foreach (string keyword in hiddenInterfaceKeywords)
            {
                if (description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        internal static Bitmap GetInterfaceIcon(NetworkInterface netInterface)
        {
            string combined = ((netInterface.Description ?? String.Empty) + (netInterface.Name ?? String.Empty));
            if (combined.IndexOf("bluetooth", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Properties.Resources.netshell_1613_16x16.ToBitmap();
            }

            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                return Properties.Resources.netshell_1612_16x16.ToBitmap();
            }

            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
            {
                return Properties.Resources.network_pipe_16x16.ToBitmap();
            }

            return Properties.Resources.deskadp_16x16.ToBitmap();
        }

        private static bool HasValidPhysicalAddress(NetworkInterface netInterface)
        {
            try
            {
                byte[] addressBytes = netInterface.GetPhysicalAddress().GetAddressBytes();
                if (addressBytes == null || addressBytes.Length == 0)
                {
                    return false;
                }

                bool allZero = true;
                foreach (byte b in addressBytes)
                {
                    if (b != 0)
                    {
                        allZero = false;
                        break;
                    }
                }

                return !allZero;
            }
            catch (NetworkInformationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Add the specified MAC address to the list of the known MAC addresses
        /// </summary>
        /// <param name="mac">A MAC address</param>
        /// <returns>True if the MAC address was already known, false otherwise</returns>
        internal static bool AddToKnownInterface(string mac, bool saveSettings = true)
        {
            bool contains = false;
            try
            {
                contains = Settings.Default.KnownInterfaceMACList.Contains(mac);
            }
            catch (ArgumentNullException) { }
            if (!contains)
            {
                Settings.Default.KnownInterfaceMACList += (Settings.Default.KnownInterfaceMACList == string.Empty ? String.Empty : ";") + mac;
                if (saveSettings)
                {
                    Settings.Default.Save();
                }
            }
            return contains;
        }

        /// <summary>
        /// Enable the statistics for the interface designed by the specified MAC address
        /// </summary>
        /// <param name="mac">A MAC address</param>
        /// <param name="enable">If set to True (default value), the interface will be enabled for statistics</param>
        internal static bool EnableStatisticsForInterface(string mac, bool enable = true, bool saveSettings = true, bool refreshCache = true)
        {
            string tmp = Settings.Default.EnabledInterfaceMACList;
            bool contains = false;
            try
            {
                contains = tmp.Contains(mac);
            }
            catch (ArgumentNullException) { }

            bool modified = false;
            bool empty = false;

            if (mac == String.Empty)
            {
                return false;
            }

            if (tmp == "TOSET")
            {
                tmp = String.Empty;
            }
            empty = tmp == String.Empty;

            if (contains)
            {
                if (!enable)
                {
                    try
                    {
                        tmp = tmp.Replace(";" + mac, ""); //mac is second to last value
                        tmp = tmp.Replace(mac + ";", ""); //mac is first value
                        tmp = tmp.Replace(mac, ""); //mac was the only value
                        modified = true;
                    }
                    catch (ArgumentNullException) { }
                    catch (ArgumentException) { }
                }
            }
            else
            {
                if (enable)
                {
                    tmp      = tmp + (empty ? String.Empty : ";") + mac;
                    modified = true;
                }
            }
            if (modified)
            {
                Settings.Default.EnabledInterfaceMACList = tmp;
                if (saveSettings)
                {
                    Settings.Default.Save();
                }
                if (refreshCache)
                {
                    RefreshEnabledInterfacesCache();
                }

                if (tmp == String.Empty)
                {
                    try
                    {
                        MessageBox.Show(Res.str_WarningNoInterfaceSelected, Res.str_WarningNoInterfaceSelectedCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (System.ComponentModel.InvalidEnumArgumentException) { }
                    catch (InvalidOperationException) { }
                }
            }
            return modified;
        }

        // ── Instance methods ──────────────────────────────────────────────────

        internal IReadOnlyList<TrackedInterface> GetDisplayableInterfacesSnapshot()
        {
            lock (selectedInterfaces)
            {
                if (selectedInterfaces.Count == 0)
                {
                    return Array.Empty<TrackedInterface>();
                }

                TrackedInterface[] snapshot = new TrackedInterface[selectedInterfaces.Count];
                for (int i = 0; i < selectedInterfaces.Count; i++)
                {
                    TrackedInterface tracked = selectedInterfaces[i];
                    snapshot[i] = new TrackedInterface(tracked.Interface, tracked.MacAddress);
                }

                return snapshot;
            }
        }

    }
}
