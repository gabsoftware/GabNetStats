# Japanese locale (.ja.resx) — handoff / remaining work

## Status summary

Working directory: `e:\dev\gabsoftware\GabNetStats\GabNetStats\`

### DONE (verified translated, following the exclusion rules in the original task)

1. **Res.ja.resx** — COMPLETE. All 39 `str_*` entries translated. File written from scratch with full ResX skeleton. Located at `e:\dev\gabsoftware\GabNetStats\GabNetStats\Res.ja.resx`.

2. **Forms\FormAbout.ja.resx** — COMPLETE. Copied from `FormAbout.fr.resx` (preserves binary `pictureBox1.Image` / `$this.Icon` blobs unchanged — correct, these are not translatable text). Edited only two entries:
   - `label3.Text` → `謝辞：Igor Tolmachev`
   - `$this.Text` → `GabNetStats について...`
   - Excluded per rules: `label1.Text` (version placeholder), `buttonOK.Text` (kept "OK"), `linkLabel1/2.Text` (URLs), `label2.Text` (copyright line).

3. **Forms\FormStatsOverlay.ja.resx** — COMPLETE. Copied from `.fr.resx`, then edited:
   - `lblStatisticsData.Text` → `統計情報を取得中...`
   - `lblSeconds.Text` → `秒`
   - `lblStatisticsTitle.Text` → `:: 統計 ::`
   - `btnAdvanced.Text` → `詳細設定...`
   - `chkAutoClose.Text` → `自動的に閉じる：`
   - Removed `$this.Text` entry entirely (excluded per rules — "GabNetStats - by GabSoftware" is branding, even though fr/ru translated it, task instructions explicitly listed `FormStatsOverlay.$this.Text` as excluded).

4. **Forms\FormSettings.ja.resx** — COMPLETE. Rewritten cleanly (not just copied) with only `.Text` entries, no Location/Size overrides (per rule #5 — Japanese text is compact). Contains 25 translated entries: buttonCancel, label1, radioCustomSpeed, groupBox1, label6, label5, grpBandwidthPreferences, label4, lblUpload ("/秒"), lblDownload ("/秒"), rbBytes, rbBits, label2, label3, radioDefault, chkSettingsAutoPingNotification, label8 (kept as "ms" untranslated — treated like a unit abbreviation, consistent with Res.resx keeping Kbit/MiB/etc. in Latin form), label7, chkSettingsAutoPingEnabled, groupBox2, checkBoxStartup, lblLanguage, chkShowDisconnectedInterfaces, groupBox3, `$this.Text`.
   - Excluded per rules: buttonOK.Text ("OK"), txtDownload/txtUpload.Text ("12500000"), btnRefreshIconSets.Text ("↻"), textBoxDuration.Text ("100"), txtSettingsAutoPingHost.Text ("google.com").

5. **Forms\FormMain.ja.resx** — COMPLETE. Copied from `.fr.resx`, then all `.Size` overrides stripped and all `.Text`/`.BalloonTipText`/`.BalloonTipTitle` entries translated (17 entries): aboutToolStripMenuItem, settingsToolStripMenuItem, advancedStatisticsToolStripMenuItem, manageWirelessNetworksToolStripMenuItem, homeGroupToolStripMenuItem, networkMapToolStripMenuItem, networkdomainworkgroupToolStripMenuItem, networkToolStripMenuItem, FirewallSettingsToolStripMenuItem, FirewallAllowedAppsToolStripMenuItem, NetworkSharingCenterToolStripMenuItem, NetworkAndInternetToolStripMenuItem, NetworkConnectionsToolStripMenuItem, NetworkAdaptersToolStripMenuItem, exitToolStripMenuItem, notifyIconActivity.BalloonTipText, notifyIconActivity.Text, notifyIconPing.BalloonTipText, notifyIconPing.BalloonTipTitle, notifyIconPing.Text.
   - Used Japanese Windows-menu convention of appending `(&X)` mnemonic suffix (e.g. `バージョン情報(&A)...`) instead of embedding `&` mid-string like French does.
   - Excluded per rules: `$this.Text` = "GabNetStats" (was already absent from the fr-derived copy — correct, do not add it), `notifyIconActivity.BalloonTipTitle` = "GabNetStats" (also already absent — correct).
   - Icon binary blobs (`notifyIconActivity.Icon`, `notifyIconPing.Icon`, `$this.Icon`) preserved unchanged from the fr copy — correct, not translatable text.

### NOT DONE — remaining work

6. **Forms\FormNetworkDetails.ja.resx** — NOT DONE. A file exists at this path (copied verbatim from `FormNetworkDetails.fr.resx` via `cp`) but **still contains French text, not Japanese**. This file MUST be edited before this task is complete.

## What's needed for FormNetworkDetails.ja.resx

The base `FormNetworkDetails.resx` has 181 translatable entries (`.Text` suffix only — no `.BalloonTip*`/`.ToolTipText` in this file). Of those, exactly **3 are excluded** (confirmed by diffing against both `.fr.resx` and `.ru.resx`, which both omit these three and only these three):

- `label29.Text` = "MTU" (technical acronym, stays untranslated — do not add an override)
- `radioButtonIPv4.Text` = "IPv4" (technical acronym, stays untranslated)
- `radioButtonIPv6.Text` = "IPv6" (technical acronym, stays untranslated)

So **178 entries need Japanese translations**. Note `$this.Text` = "GabNetStats - Advanced Statistics" **should be translated** (both fr and ru translate it, e.g. fr → "GabNetStats - Statistiques avancées") — this differs from FormMain/FormStatsOverlay where `$this.Text` is pure branding and excluded. Suggested translation: `GabNetStats - 詳細統計`.

A complete, ready-to-use English→Japanese translation map for all 178 entries was already drafted and saved to:

`C:\Users\gabri\AppData\Local\Temp\claude\e--dev-gabsoftware\eddee8b4-13f3-4de1-9c4b-9299fb576077\scratchpad\ja_map.tsv`

(tab-separated: `data-name<TAB>Japanese translation`, one entry per line, verified to exactly match the 178 required keys via `comm -3` against the base resx key list). **Check whether this file still exists in the scratchpad** — if so, reuse it directly rather than re-translating. If the scratchpad was cleared, the English source strings and suggested translations are listed below in the "Translation reference" section — regenerate the TSV from that table.

### How to finish (recommended steps)

1. Confirm `Forms\FormNetworkDetails.ja.resx` currently equals `Forms\FormNetworkDetails.fr.resx` (it was created via plain file copy and not yet edited). If the scratchpad `ja_map.tsv` still exists, use it; otherwise rebuild it from the "Translation reference" table below.
2. For each of the 178 entries in the base resx (extracted via regex on `<data name="X.Text" xml:space="preserve">\s*<value>...</value>` in `FormNetworkDetails.resx`), replace the French `<value>` text currently in `FormNetworkDetails.ja.resx` with the Japanese translation, **for each `<data name="...">` block matching those 178 names**. The safest mechanical approach:
   - Extract `(name, value)` pairs from `FormNetworkDetails.fr.resx` for the same `.Text` suffix filter.
   - For each `name` in the 178-key set, find the corresponding `<data name="NAME" ...><value>FRENCH</value></data>` block in the ja.resx copy and replace only the `<value>...</value>` inner text with the Japanese string from the map — leave the `<data name=...>` opening tag, indentation, and closing `</data>` untouched.
   - Leave all non-`.Text` entries (Location/Size/binary images if any — check whether this file has any `.Image`/`.Icon` binary blocks) untouched/as copied from fr.
   - Do NOT add `label29.Text`, `radioButtonIPv4.Text`, or `radioButtonIPv6.Text` — if these exist in the fr-derived copy (they do, since fr excludes them too, so they should already be absent), leave them absent.
3. Verify the file is valid UTF-8 XML and every one of the 178 expected keys is present with a Japanese (not French) value, using a diff/grep check analogous to:
   ```
   grep -oP '<data name="\K[^"]+(?=\.Text" xml:space)' FormNetworkDetails.ja.resx | sort > ja_keys.txt
   grep -oP '<data name="\K[^"]+(?=\.Text" xml:space)' FormNetworkDetails.resx | sort > base_keys.txt
   comm -3 base_keys.txt ja_keys.txt   # should show only label29, radioButtonIPv4, radioButtonIPv6
   ```
   Also spot check a few `<value>` fields no longer contain French words (accented characters like é, è, à are a quick red flag if still present).
4. Delete this `JA_LOCALE_TODO.md` file once FormNetworkDetails.ja.resx is confirmed complete — it's a working note, not permanent project documentation.

## Translation reference (English → Japanese, all 178 FormNetworkDetails.resx entries)

Format: `data-name` = `English source` → `Japanese translation`

```
label21.Text = Outbound packets requests → 送信要求パケット数
label18.Text = Packets with no route → 経路のないパケット数
label20.Text = Routing discards → ルーティング破棄数
label19.Text = Transmited packets discards → 送信破棄パケット数
groupBoxGlobalOutbound.Text = Outbound packets → 送信パケット
label11.Text = Packets with unknown protocol → 不明なプロトコルのパケット数
label12.Text = Packets with header errors → ヘッダーエラーのパケット数
label13.Text = Packets with address errors → アドレスエラーのパケット数
label14.Text = Forwarded packets → 転送パケット数
label15.Text = Discarded packets → 破棄パケット数
label16.Text = Delivered packets → 配信パケット数
label17.Text = Received packets → 受信パケット数
groupBoxGlobalInbound.Text = Inbound packets → 受信パケット
label22.Text = Reassembled packets → 再構築パケット数
label23.Text = Fragmented packets → フラグメント化パケット数
label24.Text = Reassembly timeout → 再構築タイムアウト数
label25.Text = Reassembly failures → 再構築失敗数
label26.Text = Reassemblies required → 再構築が必要な数
label27.Text = Packet fragmentation failures → パケット断片化失敗数
groupBoxGlobalFragmentation.Text = Packet fragmentation → パケットの断片化
label10.Text = Node type → ノードタイプ
label9.Text = Is a WINS proxy → WINSプロキシ
label8.Text = Host name → ホスト名
label7.Text = Domain name → ドメイン名
label6.Text = DHCP scope name → DHCPスコープ名
label5.Text = Number of routes → ルート数
label4.Text = Number of IP addresses → IPアドレス数
label3.Text = Number of interfaces → インターフェース数
label2.Text = Forwarding enabled → 転送を有効にする
label1.Text = Default TTL → デフォルトTTL
groupBoxGlobalMisc.Text = Miscellaneous → その他
tabPageGlobal.Text = Global statistics → 全体統計
label144.Text = MAC address → MACアドレス
label64.Text = Supports multicast → マルチキャスト対応
label65.Text = Speed → 速度
label61.Text = Reception only → 受信専用
label62.Text = ID → ID
label66.Text = Operational status → 動作状態
label58.Text = Network interface type → ネットワークインターフェースの種類
label59.Text = Name → 名前
label60.Text = Loopback interface index → ループバックインターフェースインデックス
label63.Text = Description → 説明
groupBox6.Text = Properties (2) → プロパティ (2)
label57.Text = WINS server addresses → WINSサーバーアドレス
label55.Text = Unicast addresses → ユニキャストアドレス
label56.Text = Multicast addresses → マルチキャストアドレス
label47.Text = Dynamic DNS enabled → 動的DNSの有効化
label48.Text = DNS enabled → DNSの有効化
label49.Text = Gateway addresses → ゲートウェイアドレス
label50.Text = DNS suffix → DNSサフィックス
label52.Text = DNS addresses → DNSアドレス
label53.Text = DHCP server addresses → DHCPサーバーアドレス
label54.Text = Anycast addresses → エニーキャストアドレス
groupBox5.Text = IP configuration → IP構成
label42.Text = Uses WINS → WINSの使用
label29.Text = MTU → [EXCLUDED — do not add]
label30.Text = Packet forwarding enabled → パケット転送の有効化
label31.Text = DHCP enabled → DHCPの有効化
label32.Text = APIPA address enabled → APIPAアドレスの有効化
label37.Text = Has APIPA address → APIPAアドレスあり
label39.Text = Index → インデックス
groupBox4.Text = Properties (1) → プロパティ (1)
label41.Text = Unicast packets sent → 送信ユニキャストパケット数
label43.Text = Output queue length → 出力キュー長
label44.Text = Outgoing packets with errors → 送信エラーパケット数
label45.Text = Outgoing packets discarded → 送信破棄パケット数
label46.Text = Non-unicast packets sent → 送信非ユニキャストパケット数
label51.Text = Bytes sent → 送信バイト数
groupBox3.Text = Outgoing → 送信
label40.Text = Unicast packets received → 受信ユニキャストパケット数
label33.Text = Non-unicast packets received → 受信非ユニキャストパケット数
label34.Text = Incoming packets with unknown protocol → 不明なプロトコルの受信パケット数
label35.Text = Incoming packets with errors → 受信エラーパケット数
label36.Text = Incoming packets discarded → 受信破棄パケット数
label38.Text = Bytes received → 受信バイト数
groupBox2.Text = Incoming → 受信
tabPageNetworkInterfaces.Text = Network interface statistics → ネットワークインターフェース統計
label81.Text = TCP errors received → 受信TCPエラー数
groupBox9.Text = TCP errors statistics → TCPエラー統計
label82.Text = TCP segments received → 受信TCPセグメント数
label83.Text = TCP reset sent (RST flag) → 送信TCPリセット数 (RSTフラグ)
label84.Text = TCP segments re-sent → 再送信TCPセグメント数
label85.Text = TCP segments sent → 送信TCPセグメント数
label86.Text = TCP segments minimum transmission timeout → TCPセグメント最小再送タイムアウト
label93.Text = TCP segments maximum transmission timeout → TCPセグメント最大再送タイムアウト
groupBox8.Text = TCP segments statistics → TCPセグメント統計
label68.Text = Established TCP connections → 確立済みTCP接続数
label69.Text = Maximum supported TCP connections → 最大サポートTCP接続数
label70.Text = Failed TCP connection attempts → 失敗したTCP接続試行数
label71.Text = Reset TCP connections → リセットされたTCP接続数
label72.Text = Current TCP connections → 現在のTCP接続数
label73.Text = Initiated TCP connection requests → 開始されたTCP接続要求数
label75.Text = Accepted TCP connection requests → 受理されたTCP接続要求数
groupBox7.Text = TCP connections statistics → TCP接続統計
tabPageTCP.Text = TCP statistics → TCP統計
btnSelectAll.Text = Select all → すべて選択
btnCopySelection.Text = Copy selected cells to Clipboard → 選択したセルをクリップボードにコピー
tabPageTCPConnections.Text = TCP connections → TCP接続
btnTCPLSelectAll.Text = Select all → すべて選択
btnTCPLCopySelected.Text = Copy selected cells to Clipboard → 選択したセルをクリップボードにコピー
tabPageTCPListeners.Text = TCP listeners → TCPリスナー
label74.Text = UDP listeners → UDPリスナー
label67.Text = Incoming datagrams discarded → 受信破棄データグラム数
label78.Text = Incoming datagrams with errors → 受信エラーデータグラム数
label79.Text = Datagrams sent → 送信データグラム数
label80.Text = Datagrams received → 受信データグラム数
groupBox10.Text = UDP connections statistics → UDP接続統計
tabPageUDP.Text = UDP statistics → UDP統計
btnUDPLSelectAll.Text = Select all → すべて選択
btnUDPLCopySelected.Text = Copy selected cells to Clipboard → 選択したセルをクリップボードにコピー
tabPageUDPListeners.Text = UDP listeners → UDPリスナー
label99.Text = Timestamp requests → タイムスタンプ要求
label100.Text = Timestamp replies → タイムスタンプ応答
label101.Text = Time exceeded → 時間超過
label102.Text = Source quenches → 発信元抑制
label103.Text = Redirects → リダイレクト
label104.Text = Parameter problems → パラメーター異常
label105.Text = Messages → メッセージ数
label106.Text = Errors → エラー数
label107.Text = Echo requests → エコー要求
label108.Text = Echo replies → エコー応答
label109.Text = Destination unreachable → 到達不能
label110.Text = Address mask requests → アドレスマスク要求
label111.Text = Address mask replies → アドレスマスク応答
groupBox12.Text = ICMPv4 sent messages → ICMPv4送信メッセージ
label96.Text = Timestamp requests → タイムスタンプ要求
label97.Text = Timestamp replies → タイムスタンプ応答
label98.Text = Time exceeded → 時間超過
label76.Text = Source quenches → 発信元抑制
label77.Text = Redirects → リダイレクト
label87.Text = Parameter problems → パラメーター異常
label88.Text = Messages → メッセージ数
label89.Text = Errors → エラー数
label90.Text = Echo requests → エコー要求
label91.Text = Echo replies → エコー応答
label92.Text = Destination unreachable → 到達不能
label94.Text = Address mask requests → アドレスマスク要求
label95.Text = Address mask replies → アドレスマスク応答
groupBox11.Text = ICMPv4 received messages → ICMPv4受信メッセージ
tabPageICMPv4.Text = ICMPv4 statistics → ICMPv4統計
label112.Text = Time exceeded → 時間超過
label113.Text = Router solicits → ルーター送信要求
label114.Text = Router advertisement → ルーター通知
label115.Text = Redirects → リダイレクト
label116.Text = Parameter problems → パラメーター異常
label117.Text = Packet too big → パケット過大
label118.Text = Neighbor solicitation → 近隣要請
label119.Text = Neighbor advertisement → 近隣広告
label120.Text = Messages → メッセージ数
label121.Text = IGMP membership reports → IGMPメンバーシップ報告
label122.Text = IGMP membership reductions → IGMPメンバーシップ脱退
label123.Text = IGMP membership queries → IGMPメンバーシップ照会
label124.Text = Errors → エラー数
label141.Text = Echo requests → エコー要求
label142.Text = Echo replies → エコー応答
label143.Text = Destination unreachable → 到達不能
groupBox13.Text = ICMPv6 sent messages → ICMPv6送信メッセージ
label138.Text = Time exceeded → 時間超過
label139.Text = Router solicits → ルーター送信要求
label140.Text = Router advertisement → ルーター通知
label125.Text = Redirects → リダイレクト
label126.Text = Parameter problems → パラメーター異常
label127.Text = Packet too big → パケット過大
label128.Text = Neighbor solicitation → 近隣要請
label129.Text = Neighbor advertisement → 近隣広告
label130.Text = Messages → メッセージ数
label131.Text = IGMP membership reports → IGMPメンバーシップ報告
label132.Text = IGMP membership reductions → IGMPメンバーシップ脱退
label133.Text = IGMP membership queries → IGMPメンバーシップ照会
label134.Text = Errors → エラー数
label135.Text = Echo requests → エコー要求
label136.Text = Echo replies → エコー応答
label137.Text = Destination unreachable → 到達不能
groupBox14.Text = ICMPv6 received messages → ICMPv6受信メッセージ
tabPageICMPv6.Text = ICMPv6 statistics → ICMPv6統計
radioButtonIPv4.Text = IPv4 → [EXCLUDED — do not add]
radioButtonIPv6.Text = IPv6 → [EXCLUDED — do not add]
groupBoxGlobalVersion.Text = Protocol version → プロトコルバージョン
label28.Text = Active interface : → アクティブなインターフェース：
groupBox1.Text = Network interface → ネットワークインターフェース
$this.Text = GabNetStats - Advanced Statistics → GabNetStats - 詳細統計
```

Note: `label99`/`label96` etc. duplicate English text across the ICMPv4/ICMPv6 sent/received groups (e.g. "Timestamp requests" appears at both `label99` and `label96` — these are legitimately two different labels in two different group boxes, both need the same translation, this is expected and matches the base resx).

## Final verification checklist (after FormNetworkDetails.ja.resx is finished)

For all 6 `.ja.resx` files, confirm every translatable key from the base `.resx` is present except known exclusions:
- `Res.ja.resx` — all 39 `str_*` keys present. (Already verified.)
- `Forms\FormAbout.ja.resx` — `label3.Text`, `$this.Text` present; `label1.Text`, `buttonOK.Text`, `linkLabel1.Text`, `linkLabel2.Text`, `label2.Text` absent. (Already verified.)
- `Forms\FormStatsOverlay.ja.resx` — `lblStatisticsData.Text`, `lblSeconds.Text`, `lblStatisticsTitle.Text`, `btnAdvanced.Text`, `chkAutoClose.Text` present; `$this.Text` absent. (Already verified.)
- `Forms\FormSettings.ja.resx` — 25 entries present (see list above); `buttonOK.Text`, `txtDownload.Text`, `txtUpload.Text`, `btnRefreshIconSets.Text`, `textBoxDuration.Text`, `txtSettingsAutoPingHost.Text` absent. (Already verified.)
- `Forms\FormMain.ja.resx` — 17 entries present (excludes menu `.Size`, includes all `.Text`/`.BalloonTip*`); `$this.Text`, `notifyIconActivity.BalloonTipTitle` absent. (Already verified.)
- `Forms\FormNetworkDetails.ja.resx` — **STILL TODO**: 178 `.Text` entries present, only `label29.Text`, `radioButtonIPv4.Text`, `radioButtonIPv6.Text` absent.

Once FormNetworkDetails.ja.resx is done and verified, delete this TODO file.
