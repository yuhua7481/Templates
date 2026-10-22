// Phase 9.2: REAL M6 threaded-hole creation probe (disposable mm part).
// Run inside NX2512: File > Execute > Journal. Creates a NEW temp part
// (never model2 or the user's work part), builds a block, then one blind
// M6x1.0 threaded hole at the top-face center. Reports every step to the
// Listing Window. The temp part is NOT saved; user files are untouched.
//
// Depth-expression contract to VERIFY live (not assumed):
//   ThreadedHoleDepth = total hole depth (target: 20 mm)
//   ThreadDepth       = thread depth       (target: 12 mm)
// The script prints BOTH expressions' names+values after commit so the
// mapping is proven from the model, not from property names.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;

public class ThreadedHoleCreateProbe
{
    private static ListingWindow lw;

    public static void Main(string[] args)
    {
        Session theSession = Session.GetSession();
        lw = theSession.ListingWindow;
        lw.Open();
        lw.WriteLine("=== ThreadedHoleCreateProbe ===");

        Part originalWork = theSession.Parts.Work;
        Part originalDisplayed = theSession.Parts.Display;

        Part tempPart = null;
        string tempPath = Path.Combine(
            Path.GetTempPath(),
            $"nxai_thd_probe_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}.prt");
        try
        {
            // ---- STEP A: fresh mm part via SEED-PART COPY (the live-proven
            // path — FileNew is structurally unusable in this session, see
            // WORK_LOG 8.1.5: all 430 registered templates rejected with
            // "选定的模板不存在"; the seed copy + OpenDisplay is what the
            // geometry self-check has used successfully since 8.1.6).
            // Seed search order mirrors the self-check: env override → the
            // deployed selfcheck seed → %TEMP% copy. The seed itself is
            // NEVER touched; we open a unique %TEMP% copy.
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
                    $"nxai_thd_probe_copy_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}.prt");
                try
                {
                    File.Copy(seed, copyPath, overwrite: true);
                    var loaded = theSession.Parts.OpenDisplay(copyPath, out var loadStatus);
                    if (loaded == null)
                    {
                        lw.WriteLine($"STEP-A0-FAIL: OpenDisplay returned null for {copyPath} (status={loadStatus})");
                        try { File.Delete(copyPath); } catch { }
                        continue;
                    }
                    if (loaded.PartUnits != BasePart.Units.Millimeters)
                    {
                        lw.WriteLine($"STEP-A0-FAIL: seed part units={loaded.PartUnits}, expected Millimeters");
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
                lw.WriteLine("STEP-A-FAIL: no seed part could be opened (checked env override, deployed selfcheck seed, %TEMP%)");
                return;
            }
            theSession.Parts.SetWork(tempPart);
            lw.WriteLine($"STEP-A-OK: temp part {tempPart.Name} units={tempPart.PartUnits} (seed: {loadedFrom})");

            // ---- STEP B: block 60 x 60 x 30 at origin (top face z=30)
            Body block = CreateBlock(tempPart, 60, 60, 30, theSession);
            if (block == null)
            {
                lw.WriteLine("STEP-B-FAIL: block not created");
                return;
            }
            lw.WriteLine($"STEP-B-OK: block ref={block.JournalIdentifier} faces={CountFaces(block)}");

            // ---- STEP C: locate the TOP face (planar, normal +Z, center z=30)
            Face topFace = FindTopFace(block, 30.0);
            if (topFace == null)
            {
                lw.WriteLine("STEP-C-FAIL: top planar face not found");
                return;
            }
            lw.WriteLine($"STEP-C-OK: top face ref={topFace.JournalIdentifier}");

            // ---- STEP D: hole center = top-face center via the verified UF path
            var ufSession = UFSession.GetUFSession();
            double[] uv = new double[4];
            ufSession.Modl.AskFaceUvMinmax(topFace.Tag, uv);
            double[] center = new double[3];
            double[] u1 = new double[3], v1 = new double[3], u2 = new double[3], v2 = new double[3];
            double[] unitNorm = new double[3];
            double[] radii = new double[2];
            ufSession.Modl.AskFaceProps(topFace.Tag,
                new double[] { (uv[0] + uv[1]) / 2.0, (uv[2] + uv[3]) / 2.0 },
                center, u1, v1, u2, v2, unitNorm, radii);
            var holeCenter = new Point3d(center[0], center[1], center[2]);
            lw.WriteLine($"STEP-D-OK: hole center=({holeCenter.X:F3},{holeCenter.Y:F3},{holeCenter.Z:F3}) " +
                $"faceNormal=({unitNorm[0]:F3},{unitNorm[1]:F3},{unitNorm[2]:F3})");

            // ---- STEP E: builder + full threaded config
            Session.UndoMarkId markId = theSession.SetUndoMark(
                Session.MarkVisibility.Visible, "NXAI 螺纹孔探针");
            HolePackageBuilder builder = null;
            bool committed = false;
            try
            {
                builder = tempPart.Features.CreateHolePackageBuilder(null!);
                lw.WriteLine("STEP-E1-OK: CreateHolePackageBuilder(null) accepted");

                // Position (2026-10-22, single-variable fix #5 for the commit
                // AV): the reference journal hosts the hole-position point in
                // a SKETCH and feeds the hole section via
                // CreateRuleFeaturePoints(sketchFeature) — a bare
                // CurveDumbFromPoints rule AVs at commit even with Validate
                // passing. Rebuild that structure exactly, but on OUR top
                // face: sketch placed on the top plane via the LIVE-VERIFIED
                // selected-plane path (CreateXformByPlaneXDirPoint(face,
                // xDir, origin) → SimpleSketchInPlaceBuilder), sketch point
                // added with AddGeometry(DisplayableObject) at the hole
                // center, then FeaturePointsRule from sketch.Feature.
                //
                // Hole center (30,30,30) and +Z face normal are UNCHANGED
                // from STEP-D — the reference journal's (60,20,0) coordinates
                // belong to its own part and are NOT copied.
                var uDirRaw = new Vector3d(u1[0], u1[1], u1[2]);
                double uDirLen = Math.Sqrt(uDirRaw.X * uDirRaw.X + uDirRaw.Y * uDirRaw.Y + uDirRaw.Z * uDirRaw.Z);
                var uDir = new Vector3d(uDirRaw.X / uDirLen, uDirRaw.Y / uDirLen, uDirRaw.Z / uDirLen);
                var xDirection = tempPart.Directions.CreateDirection(
                    holeCenter, uDir, SmartObject.UpdateOption.WithinModeling);
                var originPoint = tempPart.Points.CreatePoint(holeCenter);
                var xform = tempPart.Xforms.CreateXformByPlaneXDirPoint(
                    topFace, xDirection, originPoint,
                    SmartObject.UpdateOption.WithinModeling, 0.625, false, false);
                var csys = tempPart.CoordinateSystems.CreateCoordinateSystem(
                    xform, SmartObject.UpdateOption.WithinModeling);
                lw.WriteLine("STEP-E2-pre-OK: top-face CSYS built for sketch placement");

                var sketchBuilder = tempPart.Sketches.CreateSimpleSketchInPlaceBuilder();
                sketchBuilder.UseWorkPartOrigin = false;
                sketchBuilder.CoordinateSystem = csys;
                sketchBuilder.HorizontalReference.Value = xDirection;
                sketchBuilder.SketchOrigin = originPoint;
                var sketchObj = sketchBuilder.Commit() as Sketch;
                sketchBuilder.Destroy();
                if (sketchObj == null)
                {
                    lw.WriteLine("STEP-E2-FAIL: sketch on top face did not commit");
                    builder.Destroy();
                    builder = null;
                    return;
                }
                var sketchFeature = sketchObj.Feature; // SketchFeature (verified: Sketch.Feature)
                lw.WriteLine($"STEP-E2-OK: sketch {sketchObj.JournalIdentifier} on top face, feature={SafeId(sketchFeature)}");

                // Host the hole-position point INSIDE the sketch (reference
                // journal: point created, coordinates set, AddGeometry, Update).
                sketchObj.Activate(Sketch.ViewReorient.False);
                Point sketchPoint = tempPart.Points.CreatePoint(holeCenter);
                sketchObj.AddGeometry(sketchPoint, Sketch.InferConstraintsOption.InferNoConstraints);
                sketchObj.Update();
                sketchObj.Deactivate(Sketch.ViewReorient.False, Sketch.UpdateLevel.Model);
                lw.WriteLine($"STEP-E2-OK: sketch point added at ({holeCenter.X:F3},{holeCenter.Y:F3},{holeCenter.Z:F3})");

                // Feed the hole section via FeaturePointsRule (reference
                // journal's exact rule type) on the BUILDER'S OWN section.
                Section holePositionSection = builder.HolePosition;
                if (holePositionSection == null)
                {
                    lw.WriteLine("STEP-E2-FAIL: builder.HolePosition section is null");
                    builder.Destroy();
                    builder = null; // abort path: prevent double destroy in finally
                    return;
                }
                lw.WriteLine($"STEP-E2a-OK: builder.HolePosition section={holePositionSection.JournalIdentifier}");
                var featurePointsRule = tempPart.ScRuleFactory.CreateRuleFeaturePoints(
                    new Feature[] { sketchFeature }, null);
                holePositionSection.AllowSelfIntersection(true);
                holePositionSection.AllowDegenerateCurves(false);
                holePositionSection.AddToSection(
                    new SelectionIntentRule[] { featurePointsRule },
                    null, null, null,
                    new Point3d(0.0, 0.0, 0.0),
                    Section.Mode.Create, false);
                holePositionSection.EvaluateAndAskOutputEntities(out _);
                lw.WriteLine("STEP-E2b-OK: FeaturePointsRule added to builder.HolePosition (+ EvaluateAndAskOutputEntities)");

                // Single-variable fix #2 for the CommitFeature AV (2026-10-22):
                // the reference journal sets SizeOfHole=Custom BEFORE
                // TypeOfHole (hole-package sizing mode must be established
                // before the hole type switches the internal data shape).
                builder.SizeOfHole = HolePackageBuilder.SizesOfHole.Custom;
                lw.WriteLine($"STEP-E2d-OK: SizeOfHole=Custom (readback={builder.SizeOfHole})");

                builder.TypeOfHole = HolePackageBuilder.TypesOfHole.ThreadedHole;
                lw.WriteLine($"STEP-E3-OK: TypeOfHole=ThreadedHole (readback={builder.TypeOfHole})");

                // Single-variable fix #1 (kept): ThreadStandard BEFORE
                // ThreadSize — the size string resolves against the current
                // standard's table. Order now:
                // SizeOfHole → TypeOfHole → ThreadStandard → ThreadSize.
                builder.ThreadStandard = "Metric Coarse";
                lw.WriteLine($"STEP-E4pre-OK: ThreadStandard='Metric Coarse' readback='{builder.ThreadStandard}'");

                // Thread spec: UI display string first, table-name fallback.
                string acceptedSize = null;
                try { builder.ThreadSize = "M6 x 1.0"; acceptedSize = builder.ThreadSize; lw.WriteLine($"STEP-E4a-OK: ThreadSize='M6 x 1.0' readback='{acceptedSize}'"); }
                catch (Exception ex) { lw.WriteLine($"STEP-E4a-FAIL M6 x 1.0: {ex.GetType().Name}: {ex.Message}"); }
                if (acceptedSize != "M6 x 1.0")
                {
                    try { builder.ThreadSize = "M6.0_X_1.00"; acceptedSize = builder.ThreadSize; lw.WriteLine($"STEP-E4b-OK: ThreadSize='M6.0_X_1.00' readback='{acceptedSize}'"); }
                    catch (Exception ex) { lw.WriteLine($"STEP-E4b-FAIL M6.0_X_1.00: {ex.GetType().Name}: {ex.Message}"); }
                }
                if (string.IsNullOrEmpty(acceptedSize))
                {
                    lw.WriteLine("STEP-E4-ABORT: neither thread size string accepted — NOT committing");
                    builder.Destroy();
                    builder = null; // abort path: prevent double destroy in finally
                    return;
                }
                lw.WriteLine($"STEP-E5-OK: ThreadStandard already 'Metric Coarse' (set pre-size, readback='{builder.ThreadStandard}')");
                builder.ThreadLengthOption = HolePackageBuilder.ThreadLengthOptions.Custom;
                lw.WriteLine("STEP-E6-OK: ThreadLengthOption=Custom");
                builder.RelateHoleDepthToThreadDepth = false;
                lw.WriteLine("STEP-E7-OK: RelateHoleDepthToThreadDepth=false");
                builder.DepthOption = HolePackageBuilder.HoleDepthOptions.ToConeTip;
                lw.WriteLine("STEP-E8-OK: DepthOption=ToConeTip");
                builder.HoleDepthLimitOption = HolePackageBuilder.HoleDepthLimitOptions.Value;
                lw.WriteLine("STEP-E9-OK: HoleDepthLimitOption=Value");

                // Pre-commit expression evidence (READ-ONLY report first).
                var hd = builder.ThreadedHoleDepth;
                var td = builder.ThreadDepth;
                var ta = builder.ThreadedTipAngle;
                ReportExpression("ThreadedHoleDepth", hd);
                ReportExpression("ThreadDepth", td);
                ReportExpression("ThreadedTipAngle", ta);

                // Depth values via SetFormula — each failure is reported and
                // BLOCKS the commit (safety gate 4).
                bool depthOk = true;
                try
                {
                    if (hd == null) { lw.WriteLine("STEP-E10-FAIL: ThreadedHoleDepth expression is null"); depthOk = false; }
                    else
                    {
                        hd.SetFormula("20");
                        lw.WriteLine($"STEP-E10-OK: ThreadedHoleDepth.SetFormula(20) name={hd.Name} readback={hd.Value:0.###}");
                        if (Math.Abs(hd.Value - 20.0) > 0.001) { lw.WriteLine("STEP-E10-GATE: readback != 20 — NOT committing"); depthOk = false; }
                    }
                }
                catch (Exception ex) { lw.WriteLine($"STEP-E10-FAIL: {ex.GetType().Name}: {ex.Message}"); depthOk = false; }
                try
                {
                    if (td == null) { lw.WriteLine("STEP-E11-FAIL: ThreadDepth expression is null"); depthOk = false; }
                    else
                    {
                        td.SetFormula("12");
                        lw.WriteLine($"STEP-E11-OK: ThreadDepth.SetFormula(12) name={td.Name} readback={td.Value:0.###}");
                        if (Math.Abs(td.Value - 12.0) > 0.001) { lw.WriteLine("STEP-E11-GATE: readback != 12 — NOT committing"); depthOk = false; }
                    }
                }
                catch (Exception ex) { lw.WriteLine($"STEP-E11-FAIL: {ex.GetType().Name}: {ex.Message}"); depthOk = false; }
                // Tip angle: only set when the expression is live; failure is
                // reported but does NOT block (118° is also the NX default).
                try
                {
                    if (ta != null) { ta.SetFormula("118"); lw.WriteLine("STEP-E12-OK: ThreadedTipAngle.SetFormula(118)"); }
                    else lw.WriteLine("STEP-E12-SKIP: ThreadedTipAngle expression is null (118 is NX default)");
                }
                catch (Exception ex) { lw.WriteLine($"STEP-E12-SKIP: {ex.GetType().Name}: {ex.Message}"); }

                // Boolean target (single-variable fix #6 for the commit AV):
                // bind the target via the TARGET BODIES COLLECTOR exactly as
                // the reference journal does — Type first, then
                // GetTargetBodiesCollector → CreateRuleOptions(
                // SetSelectedFromInactive(false)) → CreateRuleBodyDumb →
                // ReplaceRules. SetTargetBodies removed so only ONE binding
                // path is exercised.
                try
                {
                    var boolOp = builder.BooleanOperation;
                    boolOp.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Subtract;
                    lw.WriteLine("STEP-E13a-OK: BooleanType=Subtract");

                    var collector = boolOp.GetTargetBodiesCollector();
                    lw.WriteLine($"STEP-E13b-OK: target collector={collector.JournalIdentifier}");

                    var ruleOptions = tempPart.ScRuleFactory.CreateRuleOptions();
                    ruleOptions.SetSelectedFromInactive(false);

                    var bodyDumbRule = tempPart.ScRuleFactory.CreateRuleBodyDumb(
                        new Body[] { block }, true, ruleOptions);
                    // FreeResource is protected (compiler-confirmed) — the GC
                    // finalizer owns options cleanup; the reference journal's
                    // Dispose() call is not reproducible in C#.

                    collector.ReplaceRules(new SelectionIntentRule[] { bodyDumbRule }, false);
                    lw.WriteLine($"STEP-E13c-OK: ReplaceRules bound target={block.JournalIdentifier}");
                }
                catch (Exception ex) { lw.WriteLine($"STEP-E13-FAIL: {ex.GetType().Name}: {ex.Message}"); depthOk = false; }

                // Projection direction: default FaceNormal is correct for a
                // top-face hole; report the current setting (read-only access
                // to the options object, no forced set).
                try
                {
                    var proj = builder.ProjectionDirection;
                    lw.WriteLine($"STEP-E14-OK: projection method={proj.ProjectDirectionMethod}");
                }
                catch (Exception ex) { lw.WriteLine($"STEP-E14-SKIP: {ex.GetType().Name}: {ex.Message}"); }

                if (!depthOk)
                {
                    lw.WriteLine("GATE-ABORT: one or more pre-commit checks failed — NOT committing");
                    builder.Destroy();
                    builder = null; // abort path: prevent double destroy in finally
                    return;
                }

                // Distance tolerance (2026-10-22 live finding: Validate failed
                // with "指定的公差需要三个数" — the builder's tolerance state was
                // never initialized by the null-feature create path). Set it
                // explicitly before Validate. Type confirmed: double.
                try
                {
                    builder.Tolerance = 0.01;
                    lw.WriteLine($"STEP-E15-OK: Tolerance set to {builder.Tolerance:0.###}");
                }
                catch (Exception ex)
                {
                    lw.WriteLine($"STEP-E15-FAIL: {ex.GetType().Name}: {ex.Message} — Validate will judge");
                }

                // ---- STEP F: validate + commit (only if all gates passed)
                // Single-variable fix #4 for the commit AV: the reference
                // journal sets ParentFeatureInternal = true before Commit
                // (FeatureBuilder property, NX5.0+; verified settable).
                // Placed BEFORE Validate so the validation state matches the
                // commit-time configuration.
                try
                {
                    builder.ParentFeatureInternal = true;
                    lw.WriteLine($"STEP-E16-OK: ParentFeatureInternal=true (readback={builder.ParentFeatureInternal})");
                }
                catch (Exception ex)
                {
                    lw.WriteLine($"STEP-E16-FAIL: {ex.GetType().Name}: {ex.Message} — stopping, no substitute API");
                    builder.Destroy();
                    builder = null;
                    return;
                }

                try { builder.Validate(); lw.WriteLine("STEP-F1-OK: builder.Validate() passed"); }
                catch (Exception ex)
                {
                    lw.WriteLine($"STEP-F1-GATE: Validate failed: {ex.GetType().Name}: {ex.Message} — NOT committing");
                    builder.Destroy();
                    builder = null; // abort path: prevent double destroy in finally
                    return;
                }
                // reference journal commits the hole via Builder.Commit()
                // (never CommitFeature). Switching the call; everything after
                // only runs when the result is a usable Feature.
                NXObject committedObj = builder.Commit();
                lw.WriteLine($"STEP-F2a-OK: Commit returned {(committedObj != null ? committedObj.GetType().Name + " " + SafeId(committedObj) : "null")}");

                var committedFeatureCheck = committedObj as Feature;
                if (committedObj == null || committedFeatureCheck == null)
                {
                    // Not a usable feature object: record and skip all
                    // feature-dependent checks (still a successful commit
                    // call — the temp part holds whatever NX produced).
                    lw.WriteLine("STEP-F2-WARN: Commit result is null or not a Feature — skipping feature checks");
                    committed = true;
                }
                else
                {
                    theSession.UpdateManager.DoUpdate(markId);
                    committed = true;
                    lw.WriteLine("STEP-F2b-OK: DoUpdate completed for the committed feature");
                }

                var holeFeature = committedFeatureCheck as HolePackage;
                if (holeFeature != null)
                {
                    try
                    {
                        holeFeature.GetOrigins(out var origins);
                        holeFeature.GetDirections(out var dirs);
                        lw.WriteLine($"STEP-F3-OK: feature origins={origins.Length} first=({origins[0].X:F3},{origins[0].Y:F3},{origins[0].Z:F3}) " +
                            $"dir=({dirs[0].X:F3},{dirs[0].Y:F3},{dirs[0].Z:F3})");
                    }
                    catch (Exception ex) { lw.WriteLine($"STEP-F3-FAIL: {ex.GetType().Name}: {ex.Message}"); }
                }

                // ---- STEP G: post-commit expression proof (name→value shows
                // which p-variable holds hole depth vs thread depth).
                lw.WriteLine("STEP-G: expression scan (name = value):");
                foreach (Expression expr in tempPart.Expressions.ToArray())
                {
                    try { lw.WriteLine($"  {expr.Name} = {expr.Value:0.###}"); }
                    catch { }
                }

                lw.WriteLine("PROBE-DONE (temp part left open, NOT saved)");
            }
            catch (Exception ex)
            {
                lw.WriteLine($"STEP-E/F-EXCEPTION: {ex.GetType().Name}: {ex.Message}");
                lw.WriteLine(ex.StackTrace);
                if (committed)
                {
                    lw.WriteLine("NOTE: exception AFTER commit — temp part discarded, user part untouched");
                }
                else
                {
                    try { theSession.UndoToMark(markId, "NXAI 螺纹孔探针"); lw.WriteLine("STEP-X-OK: rolled back to undo mark"); }
                    catch (Exception ux) { lw.WriteLine($"STEP-X-FAIL rollback: {ux.Message}"); }
                }
            }
            finally
            {
                if (builder != null)
                {
                    try { builder.Destroy(); lw.WriteLine("STEP-Z-OK: builder destroyed"); }
                    catch (Exception dx) { lw.WriteLine($"STEP-Z-FAIL destroy: {dx.Message}"); }
                }
            }

            // Restore user's part context; temp part stays open, unsaved.
            try
            {
                if (originalDisplayed != null) theSession.Parts.SetDisplay(originalDisplayed, true, false, out _);
                if (originalWork != null) theSession.Parts.SetWork(originalWork);
                lw.WriteLine("STEP-H-OK: original work/display part restored (temp part open, unsaved)");
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

    // ---- helpers (patterns verified live in the project) ----

    private static Body CreateBlock(Part part, double length, double width, double height, Session session)
    {
        Session.UndoMarkId markId = session.SetUndoMark(
            Session.MarkVisibility.Visible, "NXAI 螺纹孔探针-块");
        BlockFeatureBuilder blockBuilder = part.Features.CreateBlockFeatureBuilder(default(Feature)!);
        try
        {
            blockBuilder.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
            blockBuilder.SetOriginAndLengths(
                new Point3d(0, 0, 0), F(length), F(width), F(height));
            blockBuilder.CommitFeature();
            session.UpdateManager.DoUpdate(markId);
            foreach (object item in part.Bodies)
                if (item is Body b) return b; // first (only) body
            return null;
        }
        finally
        {
            try { blockBuilder.Destroy(); } catch { }
        }
    }

    private static Face FindTopFace(Body body, double z)
    {
        foreach (Face face in body.GetFaces())
        {
            try
            {
                if (face.SolidFaceType.ToString().ToLowerInvariant() != "planar") continue;
                var ufSession = UFSession.GetUFSession();
                double[] uv = new double[4];
                ufSession.Modl.AskFaceUvMinmax(face.Tag, uv);
                double[] center = new double[3];
                double[] u1 = new double[3], v1 = new double[3], u2 = new double[3], v2 = new double[3];
                double[] n = new double[3];
                double[] r = new double[2];
                ufSession.Modl.AskFaceProps(face.Tag,
                    new double[] { (uv[0] + uv[1]) / 2.0, (uv[2] + uv[3]) / 2.0 },
                    center, u1, v1, u2, v2, n, r);
                if (Math.Abs(center[2] - z) < 0.01 && n[2] > 0.99)
                    return face;
            }
            catch { }
        }
        return null;
    }

    private static void ReportExpression(string what, Expression expr)
    {
        if (expr == null)
        {
            lw.WriteLine($"STEP-EXPR {what}: null");
            return;
        }
        try { lw.WriteLine($"STEP-EXPR {what}: name={expr.Name} value={expr.Value:0.###}"); }
        catch (Exception ex) { lw.WriteLine($"STEP-EXPR {what}: unreadable ({ex.Message})"); }
    }

    private static int CountFaces(Body body)
    {
        int n = 0;
        foreach (Face _ in body.GetFaces()) n++;
        return n;
    }

    private static string SafeId(NXObject obj)
    {
        try { return obj.JournalIdentifier; } catch { return "?"; }
    }

    private static string F(double v) => v.ToString("R", CultureInfo.InvariantCulture);

    public static int GetUnloadOption(string dummy) { return (int)Session.LibraryUnloadOption.Immediately; }
}
