using System;
using System.Text;
using System.Windows.Forms;
using GabNetStats.Properties;
using System.Net.NetworkInformation;
using System.Globalization;
using System.Net;

namespace GabNetStats
{
    public partial class frmAdvanced : Form
    {
        private static NetworkInterfaceComponent ProtocolVersion { get; set; }
        private static NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
        private static NetworkInterface[] nics;

        static frmAdvanced()
        {
            ProtocolVersion = NetworkInterfaceComponent.IPv4;
            nfi.NumberDecimalDigits = 0; //we don't want decimals !
        }

        public frmAdvanced()
        {
            InitializeComponent();

            this.tabStats.Selecting += new TabControlCancelEventHandler(DisableTab_Selecting);
            //this.tabStats.DrawMode = TabDrawMode.OwnerDrawFixed;
            //this.tabStats.DrawItem += new DrawItemEventHandler(tabStats_DrawItem);  

        }

        /// <summary>
        /// Cancel the selecting event if the TabPage is disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisableTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Enabled == false)
            {
                e.Cancel = true;
            }
        }

        #region Draw Tabs
        /*

        void tabStats_DrawItem(object sender, DrawItemEventArgs e)
        {
            Color foreColor;
            TabPage tab;

            tab = tabStats.TabPages[e.Index];
            foreColor = this.tabStats.TabPages[e.Index].Enabled ? SystemColors.ControlText : SystemColors.GrayText;
            DrawTabText(this.tabStats, e, foreColor, tab.Text);
        }


        /// <summary>
        /// Draw the specified caption into a tab.
        /// </summary>
        public static void DrawTabText(TabControl tabControl, DrawItemEventArgs e, string caption)
        {
            DrawTabText(tabControl, e, SystemColors.ControlText, caption);
        }

        /// <summary>
        /// Using the specified text colour, draw the specified caption into a tab.
        /// </summary>
        public static void DrawTabText(TabControl tabControl, DrawItemEventArgs e, Color foreColor, string caption)
        {
            DrawTabText(tabControl, e, Color.Transparent, foreColor, caption);
        }

        /// <summary>
        /// Using the specified text and background colours, draw the specified caption into a tab.
        /// </summary>
        public static void DrawTabText(TabControl tabControl, DrawItemEventArgs e, Color backColor, Color foreColor, string caption)
        {

            //e.DrawBackground();
            //return;

            #region setup
            Font tabFont;
            SolidBrush foreBrush = new SolidBrush(foreColor);
            Rectangle r = e.Bounds;
            SolidBrush backBrush = new SolidBrush(backColor);
            string tabName = tabControl.TabPages[e.Index].Text;
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show, Trimming = StringTrimming.None };
            bool selected = (tabControl.SelectedIndex == e.Index);
            #endregion

            // draw
            e.Graphics.FillRectangle(backBrush, r);

            r = new Rectangle(r.X + 2 + ((selected) ? 4 : 0), r.Y + 3, r.Width - 2 - ((selected) ? 4 : 0) + 4, r.Height - 3);
            // 0 too high 4 too wide (it's a cheat for "Summary"

            tabFont = new Font(e.Font, FontStyle.Regular);

            e.Graphics.DrawString(caption, tabFont, foreBrush, r, sf);

            // clean up
            sf.Dispose();
            if (selected)
            {
                tabFont.Dispose();
                backBrush.Dispose();
            }
            else
            {
                backBrush.Dispose();
                foreBrush.Dispose();
            }
        }*/
        #endregion




        private void timerAdvanced_Tick(object sender, EventArgs e)
        {
            switch (tabStats.SelectedTab.Name)
            {
                case "tabPageGlobal" :
                    UpdateGlobalStats(ProtocolVersion);
                    break;

                case "tabPageNetworkInterfaces" :
                    UpdateNICStats(ProtocolVersion);
                    break;

                case "tabPageTCP" :
                    UpdateTCPStats(ProtocolVersion);
                    break;

                case "tabPageTCPConnections" :
                    UpdateTCPConnections();
                    break;

                case "tabPageTCPListeners" :
                    UpdateTCPListeners();
                    break;

                case "tabPageUDP" :
                    UpdateUDPStats(ProtocolVersion);
                    break;

                case "tabPageUDPListeners" :
                    UpdateUDPListeners();
                    break;

                case "tabPageICMPv4" :
                    UpdateICMPv4Stats();
                    break;

                case "tabPageICMPv6" :
                    UpdateICMPv6Stats();
                    break;

            }

            
        }

        private void frmAdvanced_Load(object sender, EventArgs e)
        {
            radioButtonIPv4.Checked = true;
            radioButtonIPv4_CheckedChanged(this, new EventArgs());

            try
            {
                nics = NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (NetworkInformationException)
            {
                
            }
            
            NetworkInterface selectedNic = null;
            foreach (NetworkInterface nic in nics)
            {
                comboInterfaces.Items.Add(nic);
                comboInterfaces.DisplayMember = "Description";
                if (nic.Description.ToLower().Contains("ethernet") || nic.Description.ToLower().Contains("gigabit") || nic.Description.ToLower().Contains("gbe"))
                {
                    //we set a preference for the ethernet nic.
                    selectedNic = nic;
                }
            }
            if (selectedNic == null)
            {
                comboInterfaces.SelectedItem = nics[0];
            }
            else
            {
                comboInterfaces.SelectedItem = selectedNic;
            }

            timerAdvanced.Interval = Settings.Default.BlinkDuration;
            timerAdvanced.Start();
        }

        private void UpdateICMPv6Stats()
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                IcmpV6Statistics stat = ipgp.GetIcmpV6Statistics();

                txtDestinationUnreachableMessagesReceivedICMPv6.Text = stat.DestinationUnreachableMessagesReceived.ToString("n", nfi);
                txtEchoRepliesReceivedICMPv6.Text = stat.EchoRepliesReceived.ToString("n", nfi);
                txtEchoRequestsReceivedICMPv6.Text = stat.EchoRequestsReceived.ToString("n", nfi);
                txtErrorsReceivedICMPv6.Text = stat.ErrorsReceived.ToString("n", nfi);
                txtMembershipQueriesReceivedICMPv6.Text = stat.MembershipQueriesReceived.ToString("n", nfi);
                txtMembershipReductionsReceivedICMPv6.Text = stat.MembershipReductionsReceived.ToString("n", nfi);
                txtMembershipReportsReceivedICMPv6.Text = stat.MembershipReportsReceived.ToString("n", nfi);
                txtMessagesReceivedICMPv6.Text = stat.MessagesReceived.ToString("n", nfi);
                txtNeighborAdvertisementsReceivedICMPv6.Text = stat.NeighborAdvertisementsReceived.ToString("n", nfi);
                txtNeighborSolicitsReceivedICMPv6.Text = stat.NeighborSolicitsReceived.ToString("n", nfi);
                txtPacketTooBigMessagesReceivedICMPv6.Text = stat.PacketTooBigMessagesReceived.ToString("n", nfi);
                txtParameterProblemsReceivedICMPv6.Text = stat.ParameterProblemsReceived.ToString("n", nfi);
                txtRedirectsReceivedICMPv6.Text = stat.RedirectsReceived.ToString("n", nfi);
                txtRouterAdvertisementsReceivedICMPv6.Text = stat.RouterAdvertisementsReceived.ToString("n", nfi);
                txtRouterSolicitsReceivedICMPv6.Text = stat.RouterSolicitsReceived.ToString("n", nfi);
                txtTimeExceededMessagesReceivedICMPv6.Text = stat.TimeExceededMessagesReceived.ToString("n", nfi);

                txtDestinationUnreachableMessagesSentICMPv6.Text = stat.DestinationUnreachableMessagesSent.ToString("n", nfi);
                txtEchoRepliesSentICMPv6.Text = stat.EchoRepliesSent.ToString("n", nfi);
                txtEchoRequestsSentICMPv6.Text = stat.EchoRequestsSent.ToString("n", nfi);
                txtErrorsSentICMPv6.Text = stat.ErrorsSent.ToString("n", nfi);
                txtMembershipQueriesSentICMPv6.Text = stat.MembershipQueriesSent.ToString("n", nfi);
                txtMembershipReductionsSentICMPv6.Text = stat.MembershipReductionsSent.ToString("n", nfi);
                txtMembershipReportsSentICMPv6.Text = stat.MembershipReportsSent.ToString("n", nfi);
                txtMessagesSentICMPv6.Text = stat.MessagesSent.ToString("n", nfi);
                txtNeighborAdvertisementsSentICMPv6.Text = stat.NeighborAdvertisementsSent.ToString("n", nfi);
                txtNeighborSolicitsSentICMPv6.Text = stat.NeighborSolicitsSent.ToString("n", nfi);
                txtPacketTooBigMessagesSentICMPv6.Text = stat.PacketTooBigMessagesSent.ToString("n", nfi);
                txtParameterProblemsSentICMPv6.Text = stat.ParameterProblemsSent.ToString("n", nfi);
                txtRedirectsSentICMPv6.Text = stat.RedirectsSent.ToString("n", nfi);
                txtRouterAdvertisementsSentICMPv6.Text = stat.RouterAdvertisementsSent.ToString("n", nfi);
                txtRouterSolicitsSentICMPv6.Text = stat.RouterSolicitsSent.ToString("n", nfi);
                txtTimeExceededMessagesSentICMPv6.Text = stat.TimeExceededMessagesSent.ToString("n", nfi);
            }
            catch (NetworkInformationException)
            {
                
            }
            catch( PlatformNotSupportedException )
            {

            }
        }

        private void UpdateICMPv4Stats()
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                IcmpV4Statistics stat = ipgp.GetIcmpV4Statistics();

                txtAddressMaskRepliesReceived.Text = stat.AddressMaskRepliesReceived.ToString("n", nfi);
                txtAddressMaskRequestsReceived.Text = stat.AddressMaskRequestsReceived.ToString("n", nfi);
                txtDestinationUnreachableMessagesReceived.Text = stat.DestinationUnreachableMessagesReceived.ToString("n", nfi);
                txtEchoRepliesReceived.Text = stat.EchoRepliesReceived.ToString("n", nfi);
                txtEchoRequestsReceived.Text = stat.EchoRequestsReceived.ToString("n", nfi);
                txtErrorsReceivedICMPv4.Text = stat.ErrorsReceived.ToString("n", nfi);
                txtMessagesReceived.Text = stat.MessagesReceived.ToString("n", nfi);
                txtParameterProblemsReceived.Text = stat.ParameterProblemsReceived.ToString("n", nfi);
                txtRedirectsReceived.Text = stat.RedirectsReceived.ToString("n", nfi);
                txtSourceQuenchesReceived.Text = stat.SourceQuenchesReceived.ToString("n", nfi);
                txtTimeExceededMessagesReceived.Text = stat.TimeExceededMessagesReceived.ToString("n", nfi);
                txtTimestampRepliesReceived.Text = stat.TimestampRepliesReceived.ToString("n", nfi);
                txtTimestampRequestsReceived.Text = stat.TimestampRequestsReceived.ToString("n", nfi);

                txtAddressMaskRepliesSent.Text = stat.AddressMaskRepliesSent.ToString("n", nfi);
                txtAddressMaskRequestsSent.Text = stat.AddressMaskRequestsSent.ToString("n", nfi);
                txtDestinationUnreachableMessagesSent.Text = stat.DestinationUnreachableMessagesSent.ToString("n", nfi);
                txtEchoRepliesSent.Text = stat.EchoRepliesSent.ToString("n", nfi);
                txtEchoRequestsSent.Text = stat.EchoRequestsSent.ToString("n", nfi);
                txtErrorsSent.Text = stat.ErrorsSent.ToString("n", nfi);
                txtMessagesSent.Text = stat.MessagesSent.ToString("n", nfi);
                txtParameterProblemsSent.Text = stat.ParameterProblemsSent.ToString("n", nfi);
                txtRedirectsSent.Text = stat.RedirectsSent.ToString("n", nfi);
                txtSourceQuenchesSent.Text = stat.SourceQuenchesSent.ToString("n", nfi);
                txtTimeExceededMessagesSent.Text = stat.TimeExceededMessagesSent.ToString("n", nfi);
                txtTimestampRepliesSent.Text = stat.TimestampRepliesSent.ToString("n", nfi);
                txtTimestampRequestsSent.Text = stat.TimestampRequestsSent.ToString("n", nfi);
            }
            catch (NetworkInformationException)
            {

            }
        }

        private void UpdateUDPListeners()
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endpoints = ipgp.GetActiveUdpListeners();
                IPEndPoint ep;
                DataGridViewRow dgvr;

                if (endpoints.Length != dataGridViewUDPListeners.Rows.Count)
                {
                    dataGridViewUDPListeners.Rows.Clear();

                    foreach (IPEndPoint endpoint in endpoints)
                    {
                        dgvr = new DataGridViewRow();
                        dgvr.CreateCells(dataGridViewUDPListeners,
                            endpoint.Address.ToString(),
                            endpoint.Port.ToString()
                        );

                        dataGridViewUDPListeners.Rows.Add(dgvr);
                    }
                }
                else
                {
                    for (int i = 0; i < endpoints.Length; i++)
                    {
                        ep = endpoints[i];
                        dgvr = dataGridViewUDPListeners.Rows[i];
                        dgvr.SetValues(
                            ep.Address.ToString(),
                            ep.Port.ToString()
                        );
                    }
                }
            }
            catch (NetworkInformationException) { }
            catch( InvalidOperationException ) { }
            catch( System.Net.Sockets.SocketException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
        }

        private void UpdateUDPStats(NetworkInterfaceComponent version)
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                UdpStatistics udpstat;

                if (version == NetworkInterfaceComponent.IPv4)
                {
                    udpstat = ipgp.GetUdpIPv4Statistics();
                }
                else
                {
                    udpstat = ipgp.GetUdpIPv6Statistics();
                }

                txtDatagramsReceived.Text = udpstat.DatagramsReceived.ToString("n", nfi);
                txtDatagramsSent.Text = udpstat.DatagramsSent.ToString("n", nfi);
                txtIncomingDatagramsDiscarded.Text = udpstat.IncomingDatagramsDiscarded.ToString("n", nfi);
                txtIncomingDatagramsWithErrors.Text = udpstat.IncomingDatagramsWithErrors.ToString("n", nfi);
                txtUdpListeners.Text = udpstat.UdpListeners.ToString("n", nfi);
            }
            catch (NetworkInformationException) { }
            catch (PlatformNotSupportedException) { }
            catch (FormatException) { }
        }

        private void UpdateTCPListeners()
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endpoints = ipgp.GetActiveTcpListeners();
                IPEndPoint ep;
                DataGridViewRow dgvr;

                if (endpoints.Length != dataGridViewTCPListeners.Rows.Count)
                {
                    dataGridViewTCPListeners.Rows.Clear();

                    foreach (IPEndPoint endpoint in endpoints)
                    {
                        dgvr = new DataGridViewRow();
                        dgvr.CreateCells(dataGridViewTCPListeners,
                            endpoint.Address.ToString(),
                            endpoint.Port.ToString()
                        );

                        dataGridViewTCPListeners.Rows.Add(dgvr);
                    }
                }
                else
                {
                    for (int i = 0; i < endpoints.Length; i++)
                    {
                        ep = endpoints[i];
                        dgvr = dataGridViewTCPListeners.Rows[i];
                        dgvr.SetValues(
                            ep.Address.ToString(),
                            ep.Port.ToString()
                        );
                    }
                }
            }
            catch (NetworkInformationException) { }
            catch (InvalidOperationException) { }
            catch (System.Net.Sockets.SocketException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
        }

        private void UpdateTCPConnections()
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] connections = ipgp.GetActiveTcpConnections();
                TcpConnectionInformation c;
                DataGridViewRow dgvr;

                if (connections.Length != dataGridViewTCPConnections.Rows.Count)
                {
                    dataGridViewTCPConnections.Rows.Clear();

                    foreach (TcpConnectionInformation con in connections)
                    {
                        dgvr = new DataGridViewRow();
                        dgvr.CreateCells(dataGridViewTCPConnections,
                            con.LocalEndPoint.Address.ToString(),
                            con.LocalEndPoint.Port.ToString(),
                            con.RemoteEndPoint.Address.ToString(),
                            con.RemoteEndPoint.Port.ToString(),
                            con.State.ToString()
                        );
                        dataGridViewTCPConnections.Rows.Add(dgvr);
                    }
                }
                else
                {
                    for (int i = 0; i < connections.Length; i++)
                    {
                        c = connections[i];
                        dgvr = dataGridViewTCPConnections.Rows[i];
                        dgvr.SetValues(
                            c.LocalEndPoint.Address.ToString(),
                            c.LocalEndPoint.Port.ToString(),
                            c.RemoteEndPoint.Address.ToString(),
                            c.RemoteEndPoint.Port.ToString(),
                            c.State.ToString()
                        );
                    }
                }
            }
            catch (NetworkInformationException) { }
            catch (InvalidOperationException) { }
            catch (System.Net.Sockets.SocketException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
        }

        private void UpdateTCPStats(NetworkInterfaceComponent version)
        {
            try
            {
                IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
                TcpStatistics tcpstat;

                if (version == NetworkInterfaceComponent.IPv4)
                {
                    tcpstat = ipgp.GetTcpIPv4Statistics();
                }
                else
                {
                    tcpstat = ipgp.GetTcpIPv6Statistics();
                }

                txtConnectionsAccepted.Text = tcpstat.ConnectionsAccepted.ToString("n", nfi);
                txtConnectionsInitiated.Text = tcpstat.ConnectionsInitiated.ToString("n", nfi);
                txtCumulativeConnections.Text = tcpstat.CumulativeConnections.ToString("n", nfi);
                txtCurrentConnections.Text = tcpstat.CurrentConnections.ToString("n", nfi);
                txtFailedConnectionAttempts.Text = tcpstat.FailedConnectionAttempts.ToString("n", nfi);
                txtMaximumConnections.Text = tcpstat.MaximumConnections.ToString("n", nfi);
                txtResetConnections.Text = tcpstat.ResetConnections.ToString("n", nfi);

                txtErrorsReceived.Text = tcpstat.ErrorsReceived.ToString("n", nfi);

                txtMaximumTransmissionTimeout.Text = String.Format("{0} ms", tcpstat.MaximumTransmissionTimeout.ToString("n", nfi));
                txtMinimumTransmissionTimeout.Text = String.Format("{0} ms", tcpstat.MinimumTransmissionTimeout.ToString("n", nfi));
                txtResetsSent.Text = tcpstat.ResetsSent.ToString("n", nfi);
                txtSegmentsReceived.Text = tcpstat.SegmentsReceived.ToString("n", nfi);
                txtSegmentsResent.Text = tcpstat.SegmentsResent.ToString("n", nfi);
                txtSegmentsSent.Text = tcpstat.SegmentsSent.ToString("n", nfi);
            }
            catch (NetworkInformationException) { }
            catch (PlatformNotSupportedException) { }
            catch (FormatException) { }
        }

        private void UpdateNICStats(NetworkInterfaceComponent version)
        {
            NetworkInterface nic = (NetworkInterface)comboInterfaces.SelectedItem;
            IPInterfaceProperties ipip; // ipv4 & ipv6
            IPv4InterfaceStatistics ipv4is; // ipv4 only
            IPv4InterfaceProperties ipv4ip; // ipv4 only
            IPv6InterfaceProperties ipv6ip; // ipv6 only


            try
            {
                ipip = nic.GetIPProperties(); // ipv4 & ipv6
            }
            catch
            {
                ipip = null;
            }

            try
            {
                ipv4is = nic.GetIPv4Statistics(); // ipv4 only
            }
            catch
            {
                ipv4is = null;
            }

            try
            {
                ipv4ip = ipip.GetIPv4Properties(); // ipv4 only
            }
            catch
            {
                ipv4ip = null;
            }

            try
            {
                ipv6ip = ipip.GetIPv6Properties(); // ipv6 only
            }
            catch
            {
                ipv6ip = null;
            }


            StringBuilder builder = new StringBuilder();
            string speedunit = String.Empty;
            byte[] mac;

            if (version == NetworkInterfaceComponent.IPv4 || ipv6ip == null )
            {
                if (ipv4is != null)
                {
                    txtBytesReceived.Text = ipv4is.BytesReceived.ToString("n", nfi);
                    txtIncomingPacketsDiscarded.Text = ipv4is.IncomingPacketsDiscarded.ToString("n", nfi);
                    txtIncomingPacketsWithErrors.Text = ipv4is.IncomingPacketsWithErrors.ToString("n", nfi);
                    txtIncomingUnknownProtocolPackets.Text = ipv4is.IncomingUnknownProtocolPackets.ToString("n", nfi);
                    txtNonUnicastPacketsReceived.Text = ipv4is.NonUnicastPacketsReceived.ToString("n", nfi);
                    txtUnicastPacketsReceived.Text = ipv4is.UnicastPacketsReceived.ToString("n", nfi);

                    txtBytesSent.Text = ipv4is.BytesSent.ToString("n", nfi);
                    txtNonUnicastPacketsSent.Text = ipv4is.NonUnicastPacketsSent.ToString("n", nfi);
                    txtUnicastPacketsSent.Text = ipv4is.UnicastPacketsSent.ToString("n", nfi);
                    txtOutgoingPacketsDiscarded.Text = ipv4is.OutgoingPacketsDiscarded.ToString("n", nfi);
                    txtOutgoingPacketsWithErrors.Text = ipv4is.OutgoingPacketsWithErrors.ToString("n", nfi);
                    txtOutputQueueLength.Text = ipv4is.OutputQueueLength.ToString("n", nfi);
                }
                else
                {
                    txtBytesReceived.Text = String.Empty;
                    txtIncomingPacketsDiscarded.Text = String.Empty;
                    txtIncomingPacketsWithErrors.Text = String.Empty;
                    txtIncomingUnknownProtocolPackets.Text = String.Empty;
                    txtNonUnicastPacketsReceived.Text = String.Empty;
                    txtUnicastPacketsReceived.Text = String.Empty;

                    txtBytesSent.Text = String.Empty;
                    txtNonUnicastPacketsSent.Text = String.Empty;
                    txtUnicastPacketsSent.Text = String.Empty;
                    txtOutgoingPacketsDiscarded.Text = String.Empty;
                    txtOutgoingPacketsWithErrors.Text = String.Empty;
                    txtOutputQueueLength.Text = String.Empty;
                }

                if (ipv4ip != null)
                {
                    txtIndex.Text = ipv4ip.Index.ToString("n", nfi);
                    txtIsAutomaticPrivateAddressingActive.Text = ipv4ip.IsAutomaticPrivateAddressingActive ? Res.str_Yes : Res.str_No;
                    txtIsAutomaticPrivateAddressingEnabled.Text = ipv4ip.IsAutomaticPrivateAddressingEnabled ? Res.str_Yes : Res.str_No;
                    txtIsDhcpEnabled.Text = ipv4ip.IsDhcpEnabled ? Res.str_Yes : Res.str_No;
                    txtIsForwardingEnabled.Text = ipv4ip.IsForwardingEnabled ? Res.str_Yes : Res.str_No;
                    txtMtu.Text = ipv4ip.Mtu.ToString("n", nfi);
                    txtUsesWins.Text = ipv4ip.UsesWins ? Res.str_Yes : Res.str_No;
                }
                else
                {
                    txtIndex.Text = String.Empty;
                    txtIsAutomaticPrivateAddressingActive.Text = String.Empty;
                    txtIsAutomaticPrivateAddressingEnabled.Text = String.Empty;
                    txtIsDhcpEnabled.Text = String.Empty;
                    txtIsForwardingEnabled.Text = String.Empty;
                    txtMtu.Text = String.Empty;
                    txtUsesWins.Text = String.Empty;
                }

            }
            else
            {
                txtBytesReceived.Text = String.Empty;
                txtIncomingPacketsDiscarded.Text = String.Empty;
                txtIncomingPacketsWithErrors.Text = String.Empty;
                txtIncomingUnknownProtocolPackets.Text = String.Empty;
                txtNonUnicastPacketsReceived.Text = String.Empty;
                txtUnicastPacketsReceived.Text = String.Empty;

                txtBytesSent.Text = String.Empty;
                txtNonUnicastPacketsSent.Text = String.Empty;
                txtUnicastPacketsSent.Text = String.Empty;
                txtOutgoingPacketsDiscarded.Text = String.Empty;
                txtOutgoingPacketsWithErrors.Text = String.Empty;
                txtOutputQueueLength.Text = String.Empty;

                txtIsAutomaticPrivateAddressingActive.Text = String.Empty;
                txtIsAutomaticPrivateAddressingEnabled.Text = String.Empty;
                txtIsDhcpEnabled.Text = String.Empty;
                txtIsForwardingEnabled.Text = String.Empty;
                txtUsesWins.Text = String.Empty;

                if (ipv6ip != null)
                {
                    txtMtu.Text = ipv6ip.Mtu.ToString("n", nfi);
                    txtIndex.Text = ipv6ip.Index.ToString("n", nfi);
                }
                else
                {
                    txtMtu.Text = String.Empty;
                    txtIndex.Text = String.Empty;
                }
            }

            if (nic != null)
            {
                txtDescription.Text = nic.Description;
                txtName.Text = nic.Name;
                txtSpeed.Text = String.Format("{0} {1}/s", MainForm.computeSpeed(nic.Speed, ref speedunit, 2).ToString("n", nfi), speedunit);
                txtNetworkInterfaceType.Text = nic.NetworkInterfaceType.ToString();
                
                mac = nic.GetPhysicalAddress().GetAddressBytes();
                if (mac.Length == 6) // MAC-48
                {
                    txtMACaddress.Text = String.Format("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
                }
                else if (mac.Length == 8) // EUI-64 (MAC-48 + 0xFFFE)
                {
                    txtMACaddress.Text = String.Format("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}-{6:X2}-{7:X2}", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5], mac[6], mac[7]);
                }
                else if (mac.Length == 0) // Loopback interface for example
                {
                    txtMACaddress.Text = Res.str_None;
                }
                else // We got a winner here...
                {
                    txtMACaddress.Text = Res.str_Unsupported;
                }

                txtId.Text = nic.Id;
                txtIsReceiveOnly.Text = nic.IsReceiveOnly ? Res.str_Yes : Res.str_No;
                txtOperationalStatus.Text = nic.OperationalStatus.ToString();
                txtLoopbackInterfaceIndex.Text = NetworkInterface.LoopbackInterfaceIndex.ToString("n", nfi);
                txtSupportsMulticast.Text = nic.SupportsMulticast ? Res.str_Yes : Res.str_No;
            }
            else
            {
                txtDescription.Text = String.Empty;
                txtName.Text = String.Empty;
                txtSpeed.Text = String.Empty;
                txtNetworkInterfaceType.Text = String.Empty;
                txtId.Text = String.Empty;
                txtIsReceiveOnly.Text = String.Empty;
                txtOperationalStatus.Text = String.Empty;
                txtLoopbackInterfaceIndex.Text = String.Empty;
                txtSupportsMulticast.Text = String.Empty;
            }

            if (ipip != null)
            {
                builder.Remove(0, builder.Length);
                foreach (IPAddressInformation ip in ipip.AnycastAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.Address.ToString()));
                }
                txtAnycastAddresses.Text = builder.ToString();

                builder.Remove(0, builder.Length);
                foreach (IPAddress ip in ipip.DhcpServerAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.ToString()));
                }
                txtDhcpServerAddresses.Text = builder.ToString();

                builder.Remove(0, builder.Length);
                foreach (IPAddress ip in ipip.DnsAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.ToString()));
                }
                txtDnsAddresses.Text = builder.ToString();

                txtDnsSuffix.Text = ipip.DnsSuffix;

                builder.Remove(0, builder.Length);
                foreach (GatewayIPAddressInformation ip in ipip.GatewayAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.Address.ToString()));
                }
                txtGatewayAddresses.Text = builder.ToString();

                txtIsDnsEnabled.Text = ipip.IsDnsEnabled ? Res.str_Yes : Res.str_No;
                txtIsDynamicDnsEnabled.Text = ipip.IsDynamicDnsEnabled ? Res.str_Yes : Res.str_No;

                builder.Remove(0, builder.Length);
                foreach (MulticastIPAddressInformation ip in ipip.MulticastAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.Address.ToString()));
                }
                txtMulticastAddresses.Text = builder.ToString();

                builder.Remove(0, builder.Length);
                foreach (UnicastIPAddressInformation ip in ipip.UnicastAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.Address.ToString()));
                }
                txtUnicastAddresses.Text = builder.ToString();

                builder.Remove(0, builder.Length);
                foreach (IPAddress ip in ipip.WinsServersAddresses)
                {
                    builder.Append(String.Format("{0} ; ", ip.ToString()));
                }
                txtWinsServersAddresses.Text = builder.ToString();
            }
            else
            {
                txtAnycastAddresses.Text = String.Empty;
                txtDhcpServerAddresses.Text = String.Empty;
                txtDnsAddresses.Text = String.Empty;
                txtDnsSuffix.Text = String.Empty;
                txtGatewayAddresses.Text = String.Empty;
                txtIsDnsEnabled.Text = String.Empty;
                txtIsDynamicDnsEnabled.Text = String.Empty;
                txtMulticastAddresses.Text = String.Empty;
                txtUnicastAddresses.Text = String.Empty;
                txtWinsServersAddresses.Text = String.Empty;
            }
            
        }

        private void UpdateGlobalStats(NetworkInterfaceComponent version)
        {
            IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
            IPGlobalStatistics ipgs;
            if (version == NetworkInterfaceComponent.IPv4)
            {
                try
                {
                    ipgs = ipgp.GetIPv4GlobalStatistics();
                }
                catch (NetworkInformationException)
                {
                    ipgs = null;
                }
                
            }
            else
            {
                try
                {
                    ipgs = ipgp.GetIPv6GlobalStatistics();
                }
                catch
                {
                    ipgs = ipgp.GetIPv4GlobalStatistics();
                }
            }
            
            txtDhcpScopeName.Text = ipgp.DhcpScopeName;
            txtDomainName.Text = ipgp.DomainName;
            txtHostName.Text = ipgp.HostName;
            txtIsWinsProxy.Text = ipgp.IsWinsProxy ? Res.str_Yes : Res.str_No;
            txtNodeType.Text = ipgp.NodeType.ToString();

            if (ipgs != null)
            {
                txtDefaultTTL.Text = String.Format("{0} hops", ipgs.DefaultTtl);
                txtForwardingEnabled.Text = ipgs.ForwardingEnabled ? Res.str_Yes : Res.str_No;
                txtNumberOfInterfaces.Text = ipgs.NumberOfInterfaces.ToString("n", nfi);
                txtNumberOfIPAddresses.Text = ipgs.NumberOfIPAddresses.ToString("n", nfi);
                txtNumberOfRoutes.Text = ipgs.NumberOfRoutes.ToString("n", nfi);

                txtOutputPacketRequests.Text = ipgs.OutputPacketRequests.ToString("n", nfi);
                txtOutputPacketRoutingDiscards.Text = ipgs.OutputPacketRoutingDiscards.ToString("n", nfi);
                txtOutputPacketsDiscarded.Text = ipgs.OutputPacketsDiscarded.ToString("n", nfi);
                txtOutputPacketsWithNoRoute.Text = ipgs.OutputPacketsWithNoRoute.ToString("n", nfi);

                txtPacketFragmentFailures.Text = ipgs.PacketFragmentFailures.ToString("n", nfi);
                txtPacketReassembliesRequired.Text = ipgs.PacketReassembliesRequired.ToString("n", nfi);
                txtPacketReassemblyFailures.Text = ipgs.PacketReassemblyFailures.ToString("n", nfi);
                txtPacketReassemblyTimeout.Text = String.Format("{0} ms", ipgs.PacketReassemblyTimeout.ToString("n", nfi));
                txtPacketsFragmented.Text = ipgs.PacketsFragmented.ToString("n", nfi);
                txtPacketsReassembled.Text = ipgs.PacketsReassembled.ToString("n", nfi);

                txtReceivedPackets.Text = ipgs.ReceivedPackets.ToString("n", nfi);
                txtReceivedPacketsDelivered.Text = ipgs.ReceivedPacketsDelivered.ToString("n", nfi);
                txtReceivedPacketsDiscarded.Text = ipgs.ReceivedPacketsDiscarded.ToString("n", nfi);
                txtReceivedPacketsForwarded.Text = ipgs.ReceivedPacketsForwarded.ToString("n", nfi);
                txtReceivedPacketsWithAddressErrors.Text = ipgs.ReceivedPacketsWithAddressErrors.ToString("n", nfi);
                txtReceivedPacketsWithHeadersErrors.Text = ipgs.ReceivedPacketsWithHeadersErrors.ToString("n", nfi);
                txtReceivedPacketsWithUnknownProtocol.Text = ipgs.ReceivedPacketsWithUnknownProtocol.ToString("n", nfi);
            }
        }

        /*private void ReEnableControlsInTabPage(TabPage tab, bool enable = true)
        {
            foreach (Control ctrl in tab.Controls)
            {
                ctrl.Enabled = enable;
                if (ctrl.GetType().Name == "GroupBox")
                {
                    foreach (Control child in ((GroupBox)ctrl).Controls)
                    {
                        child.Enabled = enable;
                    }
                }
            }
        }*/

        private void radioButtonIPv4_CheckedChanged(object sender, EventArgs e)
        {
            

            if (radioButtonIPv4.Checked)
            {
                ProtocolVersion = NetworkInterfaceComponent.IPv4;

                //tabPageICMPv4.Enabled = true;
                //tabPageICMPv6.Enabled = false;
            }
            else
            {
                ProtocolVersion = NetworkInterfaceComponent.IPv6;

                //tabPageICMPv4.Enabled = false;
                //tabPageICMPv6.Enabled = true;
            }

        }

        private void tabStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboInterfaces.Enabled = tabStats.SelectedTab == tabPageNetworkInterfaces;
            groupBoxGlobalVersion.Enabled =
                tabStats.SelectedTab == tabPageGlobal ||
                tabStats.SelectedTab == tabPageNetworkInterfaces ||
                tabStats.SelectedTab == tabPageTCP ||
                tabStats.SelectedTab == tabPageUDP;
        }

        private void CopyToClipboard(DataGridView dgv)
        {
            if (dataGridViewTCPConnections.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                Clipboard.SetDataObject(dataGridViewTCPConnections.GetClipboardContent());
            }
        }

        private void btnCopySelection_Click(object sender, EventArgs e)
        {
            CopyToClipboard(dataGridViewTCPConnections);
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            dataGridViewTCPConnections.SelectAll();
        }

        private void btnTCPLSelectAll_Click(object sender, EventArgs e)
        {
            dataGridViewTCPListeners.SelectAll();
        }

        private void btnTCPLCopySelected_Click(object sender, EventArgs e)
        {
            CopyToClipboard(dataGridViewTCPListeners);
        }

        private void btnUDPLSelectAll_Click(object sender, EventArgs e)
        {
            dataGridViewUDPListeners.SelectAll();
        }

        private void btnUDPLCopySelected_Click(object sender, EventArgs e)
        {
            CopyToClipboard(dataGridViewUDPListeners);
        }
    }
}
