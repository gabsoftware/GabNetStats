# Dutch (nl) locale task — status and remaining work

## Context

Task: add a Dutch locale to GabNetStats by creating `.nl.resx` sibling files for the
6 base resx files, following the existing `.fr.resx` / `.ru.resx` pattern. Full
original instructions are in the conversation that spawned this; the key rules are
summarized below in "Rules recap" in case that context is not available.

## Files created (all believed complete)

1. `e:\dev\gabsoftware\GabNetStats\GabNetStats\Res.nl.resx` — DONE
   - All 39 `str_*` entries translated (matches fr/ru which translate all 39).
   - Written from scratch (skeleton copied from Res.fr.resx pattern).

2. `e:\dev\gabsoftware\GabNetStats\GabNetStats\Forms\FormAbout.nl.resx` — DONE
   - 2 entries: `label3.Text` ("Met dank aan Igor Tolmachev"), `$this.Text` ("Over GabNetStats...")
   - Excluded (matches fr/ru pattern): `label1.Text` (app name+version), `buttonOK.Text` (OK),
     `linkLabel1.Text` / `linkLabel2.Text` (URLs), `label2.Text` (copyright line).

3. `e:\dev\gabsoftware\GabNetStats\GabNetStats\Forms\FormStatsOverlay.nl.resx` — DONE
   - 5 entries: lblStatisticsData.Text, lblSeconds.Text, lblStatisticsTitle.Text,
     btnAdvanced.Text, chkAutoClose.Text.
   - Excluded: `$this.Text` = "GabNetStats - by GabSoftware" (app name/branding placeholder).

4. `e:\dev\gabsoftware\GabNetStats\GabNetStats\Forms\FormSettings.nl.resx` — DONE
   - Built by copying FormSettings.fr.resx then editing text values + adding
     `rbBits.Text` ("bits/s") and `label8.Text` ("ms") which fr omitted (looked like an
     oversight in fr — ru translates both; these are ordinary short unit words so I
     translated them for nl). Kept the `.Size`/`.Location` structural overrides that
     were already present in the fr file, unchanged (didn't invent new ones).
   - 23 entries total. Excluded (matches fr numeric/URL/glyph exclusion rules):
     buttonOK.Text, txtDownload.Text, txtUpload.Text, btnRefreshIconSets.Text,
     lblUpload.Text, lblDownload.Text, textBoxDuration.Text, txtSettingsAutoPingHost.Text.

5. `e:\dev\gabsoftware\GabNetStats\GabNetStats\Forms\FormMain.nl.resx` — DONE
   - Built by copying FormMain.fr.resx then editing each of the 18 non-excluded
     `.Text`/`.BalloonTipText`/`.BalloonTipTitle` values into Dutch (mnemonic `&` accelerator
     keys preserved/reassigned sensibly, no duplicate accelerators within the same menu
     level as far as checked).
   - Excluded: `$this.Text` = "GabNetStats" (pure branding),
     `notifyIconActivity.BalloonTipTitle` = "GabNetStats" (pure branding).
   - Verified via Grep that all 18 target entries now contain Dutch text (no leftover
     French strings).

6. `e:\dev\gabsoftware\GabNetStats\GabNetStats\Forms\FormNetworkDetails.nl.resx` — DONE
   - Largest file. Copied FormNetworkDetails.fr.resx as the skeleton, then
     programmatically replaced all 178 translatable `.Text` values using a Python
     script (`apply_nl.py` + `nl_map.py`, see "Scratchpad files" below) that matched
     `<data name="KEY" xml:space="preserve"><value>...</value></data>` blocks by key
     and substituted the Dutch value (with `&`/`<`/`>` XML-escaping).
   - Excluded (confirmed by diffing base vs fr key sets): `label29.Text` (MTU),
     `radioButtonIPv4.Text`, `radioButtonIPv6.Text` — technical/jargon-only labels,
     matches task's explicit exclusion example.
   - NOTE: ru additionally excludes `label62.Text` ("ID"), but per task instructions
     ordinary short words like "ID" should be translated normally, so nl translates
     it ("ID" — kept as "ID" per Dutch convention, same word, but IS present in the
     nl file, unlike ru).
   - `$this.Text` = "GabNetStats - Advanced Statistics" — this one IS translated
     (unlike FormMain/FormStatsOverlay's `$this.Text`) because fr also translates it
     ("GabNetStats - Statistiques avancées" -> nl "GabNetStats - Geavanceerde statistieken"),
     since it has a non-branding translatable suffix.
   - Post-generation validation: confirmed with Python `xml.etree.ElementTree` that
     the resulting file parses as well-formed XML, and confirmed (via ripgrep) that
     the file contains exactly 178 `.Text`-suffixed data entries, matching the map size.
   - Judgment calls made translating ICMP/network jargon (departing slightly from a
     first draft that had left some terms in English): translated "Time exceeded" ->
     "Time-out overschreden", "Redirects" -> "Omleidingen", "Source quenches" ->
     "Bronvertragingen", "Router solicits" -> "Router-verzoeken", "Router advertisement"
     -> "Router-aankondigingen", "Neighbor solicitation" -> "Neighbor-verzoeken",
     "Neighbor advertisement" -> "Neighbor-aankondigingen" — matching the level of
     translation fr uses for the same keys (fr translates these fully into French, e.g.
     "Solicitations du routeur"), rather than leaving them as raw English ICMP jargon.

## Remaining work — NOT YET DONE

**Final verification step was interrupted before running.** The task's last step asks to
mechanically confirm, for each of the 6 new `.nl.resx` files, that every translatable key from
the base `.resx` is present in the `.nl.resx` file EXCEPT the known/documented exclusions above.

To do this (a Bash/PowerShell approach):

1. For each pair (base `X.resx`, new `X.nl.resx`), extract the set of `data name="..."` keys
   matching `.Text`, `.BalloonTipText`, `.BalloonTipTitle`, `.ToolTipText` suffixes, plus (for
   `Res.resx` only) all `str_*` keys.
2. Diff base-keys minus nl-keys. The result should equal exactly the documented exclusion list
   for that file (see per-file "Excluded" bullets above). Any unexpected key in the diff means
   something was missed and needs to be added to the `.nl.resx` file.
3. Also do the reverse diff (nl-keys minus base-keys) to make sure no typo'd/extra key names were
   introduced that don't exist in the base file (e.g. from a copy-paste error).

This was being done via Grep/Bash `comm` commands when the session was interrupted (the last
attempted command is preserved in shell history intent, not yet executed). It should be
straightforward — all 6 files are believed complete and correct from careful manual construction,
but this mechanical double-check has not actually been *run and confirmed* yet. Please run it
before declaring the task fully done.

Also worth doing as a sanity check (optional but cheap): confirm the project still builds —
```
cd e:\dev\gabsoftware\GabNetStats
dotnet build -c Release .\GabNetStats.sln
```
— though since these are pure data-only resx additions with no project file changes referencing
them explicitly (WinForms resx-per-culture files are typically auto-included via the SDK-style
csproj glob, or may need a manual check that `GabNetStats.csproj` doesn't have an explicit
`<EmbeddedResource>` allowlist that would need the new files added — worth a quick look at
`GabNetStats/GabNetStats/GabNetStats.csproj` if the build doesn't pick them up).

## Scratchpad files (for reference, not part of the repo)

Located at:
`C:\Users\gabri\AppData\Local\Temp\claude\e--dev-gabsoftware\eddee8b4-13f3-4de1-9c4b-9299fb576077\scratchpad\`

- `nl_map.py` — the full 178-entry key -> Dutch-value translation dict used for FormNetworkDetails.
- `apply_nl.py` — script that applied `nl_map.py` onto the copied fr-based FormNetworkDetails.nl.resx.
- `base_pairs.txt`, `fr_pairs.txt` — extracted name/value pairs used to build the map.
- `pt_map.txt`, `translate_map.py` — **NOT MINE / unrelated leftovers** from a prior, different
  session (Portuguese/Italian translations for what appears to be a different task or file). Do
  not use these — they were already present in the scratchpad directory before this task started
  and were not touched.

## Rules recap (in case original task context is unavailable)

- Only translate `.Text`, `.BalloonTipText`, `.BalloonTipTitle`, `.ToolTipText` data entries
  (plus `str_*` in Res.resx). Never copy `.Location`, `.Size`, `.AutoSize`, `.Anchor`,
  `.TabIndex`, `.Margin`, `.Font`, `.Image`, `.Icon`, or `>>Name`/`>>Type`/`>>Parent`/`>>ZOrder`
  designer metadata into the locale file (they're inherited from the neutral resx at runtime) —
  EXCEPT where an existing fr/ru sibling already carries a `.Location`/`.Size` override for a
  control near a longer translated label, in which case mirror it with sensible values (this
  was only relevant for FormSettings.nl.resx, where the override lines were carried over
  unchanged from fr.resx since Dutch text lengths were comparable to French).
- Never translate: URLs, copyright/branding lines, "GabNetStats {version}"/"GabNetStats" pure
  app-name placeholders, numeric/placeholder default field values (ports, IPs, "100", etc.),
  the refresh glyph "↻", "OK" on buttonOK.
- Keep standard networking/protocol acronyms in English/as-is: TCP, UDP, MTU, IPv4, IPv6, MAC,
  DNS, ICMP, IP, URL, ID (though "ID" as a standalone ordinary UI word should still be
  translated/kept per normal Dutch usage, i.e. present in the file), etc. But do translate
  ordinary surrounding UI sentences/words even when they contain those acronyms.
- Dutch IT users are used to English tech terms — acceptable to keep some English loanwords in
  Dutch UI text when that reads more naturally, but default to translating ordinary UI words.
