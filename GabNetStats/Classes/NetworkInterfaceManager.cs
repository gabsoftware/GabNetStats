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
        //
        //  Constants
        //
        private const int    NIC_REFRESH_INTERVAL_MS  = 10000;
        private const string INTERFACE_LIST_UNSET      = "TOSET";
        private const string MAC_LIST_SEPARATOR        = ";";

        //
        //  Static state (lock-free, shared across threads)
        //
        internal static TrayIconManager.eState connectionStatus;

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

        //
        //  Instance state
        //
        internal List<TrackedInterface> selectedInterfaces = new List<TrackedInterface>();
        internal IPGlobalProperties properties;
        internal IPGlobalStatistics ipv4stat;
        internal IPGlobalStatistics ipv6stat;
        internal int nbNIC       = 0;
        internal int nNICRefresh = NIC_REFRESH_INTERVAL_MS; //time interval for refreshing the NIC list (10s by default)

        //
        //  Nested type
        //
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

        //
        //  Static methods
        //
        internal static void RefreshEnabledInterfacesCache()
        {
            HashSet<string> newSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string enabledList = Settings.Default.EnabledInterfaceMACList;

            if (!String.IsNullOrWhiteSpace(enabledList) && !String.Equals(enabledList, INTERFACE_LIST_UNSET, StringComparison.OrdinalIgnoreCase))
            {
                string[] entries = enabledList.Split(new[] { MAC_LIST_SEPARATOR[0] }, StringSplitOptions.RemoveEmptyEntries);
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

        internal static bool IsPreferredInterface(NetworkInterface nic)
        {
            string description = nic?.Description;
            if (string.IsNullOrEmpty(description))
            {
                return false;
            }

            string descriptionLower = description.ToLowerInvariant();
            return descriptionLower.Contains("ethernet") ||
                   descriptionLower.Contains("gigabit") ||
                   descriptionLower.Contains("gbe");
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
                Settings.Default.KnownInterfaceMACList += (Settings.Default.KnownInterfaceMACList == string.Empty ? String.Empty : MAC_LIST_SEPARATOR) + mac;
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

            if (tmp == INTERFACE_LIST_UNSET)
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
                        tmp = tmp.Replace(MAC_LIST_SEPARATOR + mac, ""); //mac is second to last value
                        tmp = tmp.Replace(mac + MAC_LIST_SEPARATOR, ""); //mac is first value
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
                    tmp      = tmp + (empty ? String.Empty : MAC_LIST_SEPARATOR) + mac;
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

        //
        //  Instance methods
        //
        internal sealed class NicDisplayInfo
        {
            public NicDisplayInfo(NetworkInterface netInterface, string mac, string ip, Bitmap icon)
            {
                Interface = netInterface;
                Mac       = mac;
                Ip        = ip;
                Icon      = icon;
            }

            public NetworkInterface Interface { get; }
            public string           Mac       { get; }
            public string           Ip        { get; }
            public Bitmap           Icon      { get; }
        }

        /// <summary>
        /// Refreshes selectedInterfaces and connectionStatus from the current system state.
        /// Returns one NicDisplayInfo per displayable interface for the caller to build UI from.
        /// </summary>
        internal List<NicDisplayInfo> RefreshInterfaces()
        {
            int    nUp             = 0;
            string ip              = "";
            bool   isFirstTime     = Settings.Default.EnabledInterfaceMACList == INTERFACE_LIST_UNSET;
            string mac             = string.Empty;
            bool   shouldSaveSettings = false;
            var    result          = new List<NicDisplayInfo>();
            IPInterfaceProperties ipproperties;

            NetworkInterface[] nicSnapshot;
            try
            {
                nicSnapshot = NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (NetworkInformationException)
            {
                nicSnapshot = Array.Empty<NetworkInterface>();
            }

            lock (selectedInterfaces)
            {
                selectedInterfaces.Clear();

                foreach (NetworkInterface netInterface in nicSnapshot)
                {
                    if (!ShouldDisplayInterface(netInterface))
                    {
                        continue;
                    }

                    //test if the interface is active
                    if (netInterface.OperationalStatus != OperationalStatus.Up)
                    {
                        ip = Res.str_NotConnected;
                    }
                    else
                    {
                        try
                        {
                            ipproperties = netInterface.GetIPProperties();
                        }
                        catch (NetworkInformationException)
                        {
                            ip = Res.str_NoIpAvailable;
                            continue;
                        }
                        catch (PlatformNotSupportedException)
                        {
                            ip = Res.str_NoIpAvailable;
                            continue;
                        }
                        if (ipproperties.UnicastAddresses.Count > 0)
                        {
                            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                            {
                                nUp++;
                            }
                            try
                            {
                                ip = ipproperties.UnicastAddresses[ipproperties.UnicastAddresses.Count - 1].Address.ToString();
                            }
                            catch (IndexOutOfRangeException) { }
                            catch (System.Net.Sockets.SocketException) { }
                        }
                        else
                        {
                            ip = Res.str_NoIpAvailable;
                        }
                    }

                    //retrieve the mac address of the interface
                    mac = netInterface.GetPhysicalAddress().ToString();

                    // if we never selected an interface before, enable it for statistics by default.
                    if (isFirstTime)
                    {
                        if (EnableStatisticsForInterface(mac, true, false, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    // if this is the first time we encounter this interface, enable it for statistics by default.
                    if (!AddToKnownInterface(mac, false))
                    {
                        shouldSaveSettings = true;
                        if (EnableStatisticsForInterface(mac, true, false, false))
                        {
                            shouldSaveSettings = true;
                        }
                    }

                    selectedInterfaces.Add(new TrackedInterface(netInterface, mac));
                    result.Add(new NicDisplayInfo(netInterface, mac, ip, GetInterfaceIcon(netInterface)));
                }
            }

            if (nUp == 0)
            {
                connectionStatus = TrayIconManager.eState.disconnected;
            }
            else
            {
                connectionStatus = TrayIconManager.eState.up;
            }

            if (shouldSaveSettings)
            {
                Settings.Default.Save();
                RefreshEnabledInterfacesCache();
            }

            return result;
        }

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
