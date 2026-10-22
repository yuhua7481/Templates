// Phase 9.2-multi: NINE threaded holes in ONE journal run, ONE disposable
// mm part, ONE block. Spec order M12→M1.6; every spec resolved from the
// INSTALLED NX_Thread_Standard.xml by EXACT match on
//   Standard='Metric Coarse' AND MajorDiameter(numeric)=nominal AND
//   RadialEngage='0.75'
// — must be exactly ONE row, zero/multiple abort the remaining holes.
// Depths are the ONLY custom values (hole 20, thread 12, gate 12<20);
// every other thread parameter syncs from the matched record's raw fields
// with SetFormula + readback (±0.001 gate). Each hole gets a FRESH
// HolePackageBuilder. Any AV/ambiguity/gate failure STOPS all remaining
// holes (no dead-builder reuse). Temp part NOT saved; user parts restored.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;

public class ThreadedHoleTableMultiProbe
{
    private static ListingWindow lw;
    private const double CompareTolerance = 0.001;

    private static readonly (double Nominal, string Label)[] Targets =
    {
        (12.0, "M12"), (10.0, "M10"), (8.0, "M8"),
        (6.0, "M6"), (5.0, "M5"), (4.0, "M4"),
        (3.0, "M3"), (2.0, "M2"), (1.6, "M1.6"),
    };

    // 3×3 grid: X 30/120/210, Y 30/90/150, Z 40 (top face). Spacing ≥ 42mm
    // between columns and ≥ 24/60 rows — far beyond the largest tap drill
    // (M12 tap ≈ 10.25) plus chamfer, so holes cannot interact.
    private static readonly (double X, double Y)[] Positions =
    {
        (30.0, 30.0), (30.0, 90.0), (30.0, 150.0),
        (120.0, 30.0), (120.0, 90.0), (120.0, 150.0),
        (210.0, 30.0), (210.0, 90.0), (210.0, 150.0),
    };

    private const string XmlPath =
        @"D:\Program Files\Siemens\DesigncenterNX2512\UGII\modeling_standards\NX_Thread_Standard.xml";

    public static void Main(string[] args)
    {
        Session theSession = Session.GetSession();
        lw = theSession.ListingWindow;
        lw.Open();
        lw.WriteLine("=== ThreadedHoleTableMultiProbe ===");
        lw.WriteLine($"Standard XML: {XmlPath}");

        Part originalWork = theSession.Parts.Work;
        Part originalDisplayed = theSession.Parts.Display;

        Part tempPart = null;
        try
        {
            // ---- STEP A: seed-part copy (live-proven)
            string[] seedLocations =
            {
                Environment.GetEnvironmentVariable("NXAI_SELFCHECK_SEED_PART"),
                @"D:\AI\NX_AI_ASSISTANT\deploy\nxai_custom\application\selfcheck\seed-mm.prt",
                Path.Combine(Path.GetTempPath(), "nxai_selfcheck_seed_mm.prt"),
            };
            string loadedFrom = null;
            foreach (var seed in seedLocations)
            {
                if (string.IsNullOrWhiteSpace(seed) || !File.Exists(seed))
                {
                    lw.WriteLine($"STEP-A0: seed candidate skipped: {seed ?? "(null)"}");
                    continue;
                }
                var copyPath = Path.Combine(Path.GetTempPath(),
                    $"nxai_thd_multi_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}.prt");
                try
                {
                    File.Copy(seed, copyPath, overwrite: true);
                    var loaded = theSession.Parts.OpenDisplay(copyPath, out var loadStatus);
                    if (loaded == null)
                    {
                        lw.WriteLine($"STEP-A0-FAIL: OpenDisplay null (status={loadStatus})");
                        try { File.Delete(copyPath); } catch { }
                        continue;
                    }
                    if (loaded.PartUnits != BasePart.Units.Millimeters)
                    {
                        lw.WriteLine($"STEP-A0-FAIL: units={loaded.PartUnits}");
                        try { File.Delete(copyPath); } catch { }
                        continue;
                    }
                    tempPart = loaded;
                    loadedFrom = seed;
                    break;
                }
                catch (Exception ex)
                {
                    lw.WriteLine($"STEP-A0-FAIL seed {seed}: {ex.GetType().Name}: {ex.Message}");
                    try { File.Delete(copyPath); } catch { }
                }
            }
            if (tempPart == null)
            {
                lw.WriteLine("STEP-A-FAIL: no seed part could be opened");
                return;
            }
            theSession.Parts.SetWork(tempPart);
            lw.WriteLine($"STEP-A-OK: temp part {tempPart.Name} (seed: {loadedFrom})");

            // ---- STEP B: block 240 x 180 x 40 (fits the 3×3 grid)
            Body block = null;
            Session.UndoMarkId markBlock = theSession.SetUndoMark(
                Session.MarkVisibility.Visible, "NXAI 批量螺纹孔探针-块");
            BlockFeatureBuilder blockBuilder = tempPart.Features.CreateBlockFeatureBuilder(default(Feature)!);
            try
            {
                blockBuilder.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                blockBuilder.SetOriginAndLengths(new Point3d(0, 0, 0), F(240), F(180), F(40));
                blockBuilder.CommitFeature();
                theSession.UpdateManager.DoUpdate(markBlock);
                foreach (object item in tempPart.Bodies)
                    if (item is Body b) { block = b; break; }
            }
            finally
            {
                try { blockBuilder.Destroy(); } catch { }
            }
            if (block == null)
            {
                lw.WriteLine("STEP-B-FAIL: block not created");
                return;
            }
            lw.WriteLine($"STEP-B-OK: block ref={block.JournalIdentifier}");

            // ---- STEP C: load the standard table (all ThreadedHole rows)
            var table = LoadStandardTable();
            if (table == null) return; // load failure already logged
            lw.WriteLine($"STEP-C-OK: standard table loaded, {table.Count} ThreadedHole records");

            // ---- STEP D: nine holes, fresh builder each, stop-on-failure
            int okCount = 0;
            var failureReasons = new List<string>();
            for (int i = 0; i < Targets.Length; i++)
            {
                var (nominal, label) = Targets[i];
                var (px, py) = Positions[i];
                lw.WriteLine($"==== HOLE {i + 1}/9: {label} at ({px},{py},40) ====");

                // Table lookup: EXACT triple, numeric MajorDiameter.
                var matches = new List<Dictionary<string, string>>();
                foreach (var row in table)
                {
                    if (!string.Equals(row.GetValueOrDefault("Standard"), "Metric Coarse", StringComparison.Ordinal))
                        continue;
                    if (!TryParseNum(row.GetValueOrDefault("MajorDiameter"), out var major) || major != nominal)
                        continue;
                    if (!string.Equals(row.GetValueOrDefault("RadialEngage"), "0.75", StringComparison.Ordinal))
                        continue;
                    matches.Add(row);
                }
                lw.WriteLine($"  table matches (Metric Coarse + Major={nominal} + RadialEngage=0.75): {matches.Count}");
                foreach (var m in matches)
                    lw.WriteLine($"    Size='{m.GetValueOrDefault("Size")}' TapDrillDia='{m.GetValueOrDefault("TapDrillDia")}' Pitch='{m.GetValueOrDefault("Pitch")}'");
                if (matches.Count != 1)
                {
                    string reason = matches.Count == 0
                        ? $"NO matching record for {label}"
                        : $"AMBIGUOUS: {matches.Count} records for {label}";
                    lw.WriteLine($"  HOLE-GATE-STOP: {reason} — remaining holes skipped");
                    failureReasons.Add(reason);
                    break;
                }
                var rec = matches[0];

                // Hole center from the top face at the grid position.
                var holeCenter = new Point3d(px, py, 40.0);
                Point holePoint = tempPart.Points.CreatePoint(holeCenter);
                lw.WriteLine($"  position point={holePoint.JournalIdentifier}");

                Session.UndoMarkId markId = theSession.SetUndoMark(
                    Session.MarkVisibility.Visible, $"NXAI 批量螺纹孔 {label}");
                HolePackageBuilder builder = null;
                bool committed = false;
                bool fatal = false;
                try
                {
                    builder = tempPart.Features.CreateHolePackageBuilder(null!);
                    lw.WriteLine($"  E1-OK: CreateHolePackageBuilder(null) accepted");

                    // Threaded config, MINIMUM+sync set, order from the
                    // successful M6/M3 runs:
                    builder.SizeOfHole = HolePackageBuilder.SizesOfHole.Custom;
                    lw.WriteLine($"  E2-OK: SizeOfHole=Custom (readback={builder.SizeOfHole})");
                    builder.TypeOfHole = HolePackageBuilder.TypesOfHole.ThreadedHole;
                    lw.WriteLine($"  E3-OK: TypeOfHole=ThreadedHole (readback={builder.TypeOfHole})");

                    // Standard/Size/Radial from the TABLE RECORD (raw strings).
                    builder.ThreadStandard = rec["Standard"];
                    builder.ThreadSize = rec["Size"];
                    builder.RadialEngageOption = rec["RadialEngage"];
                    lw.WriteLine($"  E4-OK: ThreadStandard='{builder.ThreadStandard}' " +
                        $"ThreadSize='{builder.ThreadSize}' RadialEngageOption='{builder.RadialEngageOption}'");

                    // Position section (baseline-proven).
                    Section posSection = builder.HolePosition;
                    if (posSection == null)
                    {
                        lw.WriteLine("  F0-GATE: builder.HolePosition section is null — STOP");
                        failureReasons.Add($"{label}: null position section"); fatal = true;
                    }
                    else
                    {
                        posSection.SetAllowedEntityTypes(Section.AllowTypes.OnlyPoints);
                        posSection.DistanceTolerance = 0.01;
                        posSection.ChainingTolerance = 0.0095;
                        var pointRule = tempPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(
                            new Point[] { holePoint });
                        posSection.AddToSection(
                            new SelectionIntentRule[] { pointRule },
                            holePoint, null, null,
                            new Point3d(holeCenter.X, holeCenter.Y, holeCenter.Z),
                            Section.Mode.Create, false);
                        posSection.EvaluateAndAskOutputEntities(out var outputEntities);
                        lw.WriteLine($"  F4: position outputs={outputEntities.Length}");
                        if (outputEntities.Length != 1)
                        {
                            lw.WriteLine($"  F4-GATE: expected 1 output — STOP");
                            failureReasons.Add($"{label}: {outputEntities.Length} position outputs"); fatal = true;
                        }
                    }

                    if (!fatal)
                    {
                        // Sync ALL direct counterpart fields from the record
                        // (tap drill, chamfers, relief — NOT the depths).
                        bool syncOk = true;
                        SyncExpr(builder, rec, "TapDrillDiameter", "TapDrillDia", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedStartChamferDiameter", "StartChamferDiameter", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedStartChamferAngle", "StartChamferAngle", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedEndChamferDiameter", "EndChamferDiameter", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedEndChamferAngle", "EndChamferAngle", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedReliefDiameter", "ReliefDiameter", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedReliefDepth", "ReliefDepth", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedReliefAngle", "ReliefTipAngle", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedReliefChamferAngle", "ReliefChamferAngle", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedReliefChamferOffset", "ReliefChamferOffset", ref syncOk);
                        SyncExpr(builder, rec, "ThreadedTipAngle", "HoleTipAngle", ref syncOk);

                        // Depths: THE ONLY custom values, gated 12 < 20.
                        bool depthOk = syncOk;
                        SetExpr("ThreadedHoleDepth", builder.ThreadedHoleDepth, "20", ref depthOk, 20.0);
                        SetExpr("ThreadDepth", builder.ThreadDepth, "12", ref depthOk, 12.0);
                        if (12.0 >= 20.0) { lw.WriteLine("  G-DEPTH-GATE: 12 >= 20 — STOP"); depthOk = false; }
                        else lw.WriteLine("  G-DEPTH-OK: 12 < 20 confirmed");

                        if (ThreadLengthOptionLogged(ref failureReasons, builder)) { /* logged only */ }
                        builder.Tolerance = 0.01;
                        lw.WriteLine($"  H0-OK: Tolerance={builder.Tolerance:0.###}");

                        var boolOp = builder.BooleanOperation;
                        boolOp.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Subtract;
                        boolOp.SetTargetBodies(new Body[] { block });
                        lw.WriteLine($"  H1-OK: boolean=Subtract target={block.JournalIdentifier}");

                        // Validate (bool + exception both checked).
                        bool validateOk = false;
                        try
                        {
                            validateOk = builder.Validate();
                            lw.WriteLine($"  I1: Validate returned {validateOk}");
                        }
                        catch (Exception ex)
                        {
                            lw.WriteLine($"  I1-GATE: Validate threw: {ex.GetType().Name}: {ex.Message} — NOT committing");
                        }
                        if (!validateOk || !depthOk || !syncOk)
                        {
                            lw.WriteLine("  I1-GATE: validation/value gates failed — NOT committing, STOP all holes");
                            failureReasons.Add($"{label}: Validate={validateOk} syncOk={syncOk} depthOk={depthOk}");
                            fatal = true;
                        }
                        else
                        {
                            NXObject committedObj = builder.CommitFeature();
                            theSession.UpdateManager.DoUpdate(markId);
                            committed = true;
                            lw.WriteLine($"  J1-OK: CommitFeature returned {(committedObj != null ? committedObj.GetType().Name + " " + SafeId(committedObj) : "null")}");
                            if (committedObj is HolePackage hp)
                            {
                                try
                                {
                                    hp.GetOrigins(out var origins);
                                    hp.GetDirections(out var dirs);
                                    lw.WriteLine($"  J2-OK: origin=({origins[0].X:F3},{origins[0].Y:F3},{origins[0].Z:F3}) " +
                                        $"dir=({dirs[0].X:F3},{dirs[0].Y:F3},{dirs[0].Z:F3})");
                                }
                                catch (Exception ex) { lw.WriteLine($"  J2-FAIL: {ex.Message}"); }
                            }
                            okCount++;
                            lw.WriteLine($"  ==== {label} DONE ({okCount}/9) ====");
                        }
                    }
                }
                catch (Exception ex)
                {
                    lw.WriteLine($"  HOLE-EXCEPTION ({label}): {ex.GetType().Name}: {ex.Message}");
                    lw.WriteLine(ex.StackTrace);
                    failureReasons.Add($"{label}: {ex.GetType().Name}: {ex.Message}");
                    fatal = true;
                }
                finally
                {
                    if (builder != null)
                    {
                        try { builder.Destroy(); lw.WriteLine("  Z-OK: builder destroyed"); }
                        catch (Exception dx) { lw.WriteLine($"  Z-FAIL destroy: {dx.Message}"); }
                    }
                }

                // AV / fatal => stop immediately (no dead-builder reuse).
                if (fatal || failureReasons.Count > 0)
                {
                    lw.WriteLine($"==== STOPPED after {okCount}/9 holes — first failure: {failureReasons[0]} ====");
                    break;
                }
            }

            lw.WriteLine($"==== BATCH RESULT: {okCount}/9 holes committed ====");
            foreach (var f in failureReasons) lw.WriteLine($"  failure: {f}");

            // Restore user's part context; temp part stays open, unsaved.
            try
            {
                if (originalDisplayed != null) theSession.Parts.SetDisplay(originalDisplayed, true, false, out _);
                if (originalWork != null) theSession.Parts.SetWork(originalWork);
                lw.WriteLine("STEP-H-OK: original work/display part restored (temp part open, NOT saved)");
            }
            catch (Exception ex)
            {
                lw.WriteLine($"STEP-H-FAIL: {ex.GetType().Name}: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            lw.WriteLine($"FATAL: {ex.GetType().Name}: {ex.Message}");
            lw.WriteLine(ex.StackTrace);
        }
    }

    // Loads all ThreadedHole records as raw-string dictionaries.
    private static List<Dictionary<string, string>> LoadStandardTable()
    {
        try
        {
            var doc = new XmlDocument();
            doc.Load(XmlPath);
            var rows = new List<Dictionary<string, string>>();
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("//ThreadedHole"))
            {
                var row = new Dictionary<string, string>(StringComparer.Ordinal);
                if (node.Attributes != null)
                    foreach (XmlAttribute a in node.Attributes)
                        row[a.Name] = a.Value;
                rows.Add(row);
            }
            return rows;
        }
        catch (Exception ex)
        {
            lw.WriteLine($"STEP-C-FAIL: table load {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    }

    private static bool ThreadLengthOptionLogged(ref List<string> failureReasons, HolePackageBuilder builder)
    {
        try { lw.WriteLine($"  E7-DEFAULT ThreadLengthOption={builder.ThreadLengthOption}"); }
        catch (Exception ex) { lw.WriteLine($"  E7-DEFAULT ThreadLengthOption unreadable: {ex.Message}"); }
        return false;
    }

    private static bool TryParseNum(string raw, out double value)
    {
        value = double.NaN;
        return !string.IsNullOrEmpty(raw) &&
               double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    // Syncs ONE builder expression from its XML counterpart field with the
    // four-step gate (exists → non-null → SetFormula → readback ±0.001).
    private static void SyncExpr(HolePackageBuilder builder, Dictionary<string, string> rec,
        string builderProp, string xmlField, ref bool ok)
    {
        if (!rec.TryGetValue(xmlField, out var xmlRaw))
        {
            lw.WriteLine($"  SYNC {builderProp} ← {xmlField}: NOT PRESENT IN SOURCE — skipped");
            return;
        }
        if (!TryParseNum(xmlRaw, out var xmlVal))
        {
            lw.WriteLine($"  SYNC {builderProp} ← {xmlField}: raw='{xmlRaw}' PARSE FAILED"); ok = false;
            return;
        }
        try
        {
            var expr = builderProp switch
            {
                "TapDrillDiameter" => builder.TapDrillDiameter,
                "ThreadedStartChamferDiameter" => builder.ThreadedStartChamferDiameter,
                "ThreadedStartChamferAngle" => builder.ThreadedStartChamferAngle,
                "ThreadedEndChamferDiameter" => builder.ThreadedEndChamferDiameter,
                "ThreadedEndChamferAngle" => builder.ThreadedEndChamferAngle,
                "ThreadedReliefDiameter" => builder.ThreadedReliefDiameter,
                "ThreadedReliefDepth" => builder.ThreadedReliefDepth,
                "ThreadedReliefAngle" => builder.ThreadedReliefAngle,
                "ThreadedReliefChamferAngle" => builder.ThreadedReliefChamferAngle,
                "ThreadedReliefChamferOffset" => builder.ThreadedReliefChamferOffset,
                "ThreadedTipAngle" => builder.ThreadedTipAngle,
                _ => null,
            };
            if (expr == null)
            {
                lw.WriteLine($"  SYNC {builderProp}: builder expression is null — GATE FAIL"); ok = false;
                return;
            }
            expr.SetFormula(xmlVal.ToString("R", CultureInfo.InvariantCulture));
            double delta = expr.Value - xmlVal;
            lw.WriteLine($"  SYNC-OK {builderProp} ← {xmlField}: raw='{xmlRaw}' name={expr.Name} " +
                $"readback={expr.Value.ToString("G17", CultureInfo.InvariantCulture)} " +
                $"delta={delta.ToString("G17", CultureInfo.InvariantCulture)}");
            if (Math.Abs(delta) > CompareTolerance)
            {
                lw.WriteLine($"  SYNC-GATE {builderProp}: out of tolerance — GATE FAIL"); ok = false;
            }
        }
        catch (Exception ex)
        {
            lw.WriteLine($"  SYNC {builderProp}: FAILED ({ex.GetType().Name}: {ex.Message})"); ok = false;
        }
    }

    private static void SetExpr(string what, Expression expr, string formula, ref bool ok, double expected)
    {
        if (expr == null)
        {
            lw.WriteLine($"  G-FAIL {what}: expression is null"); ok = false; return;
        }
        try
        {
            expr.SetFormula(formula);
            lw.WriteLine($"  G-OK {what}: name={expr.Name} set='{formula}' readback={expr.Value.ToString("G17", CultureInfo.InvariantCulture)}");
            if (Math.Abs(expr.Value - expected) > CompareTolerance)
            {
                lw.WriteLine($"  G-GATE {what}: readback != {expected} — GATE FAIL"); ok = false;
            }
        }
        catch (Exception ex)
        {
            lw.WriteLine($"  G-FAIL {what}: {ex.GetType().Name}: {ex.Message}"); ok = false;
        }
    }

    private static string SafeId(NXObject obj)
    {
        try { return obj.JournalIdentifier; } catch { return "?"; }
    }

    private static string F(double v) => v.ToString("R", CultureInfo.InvariantCulture);

    public static int GetUnloadOption(string dummy) { return (int)Session.LibraryUnloadOption.Immediately; }
}
