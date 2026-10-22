// Designcenter 2512
// Journal created by yuhua on Thu Oct 22 16:53:15 2026 中国标准时间

//
using System;
using NXOpen;

public class NXJournal
{
  public static void Main(string[] args)
  {
    NXOpen.Session theSession = NXOpen.Session.GetSession();
    NXOpen.Part workPart = theSession.Parts.Work;
    NXOpen.Part displayPart = theSession.Parts.Display;
    NXOpen.Scalar scalar1;
    scalar1 = workPart.Scalars.CreateScalar(0.0, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Features.Block block1 = ((NXOpen.Features.Block)workPart.Features.FindObject("BLOCK(1)"));
    NXOpen.Edge edge1 = ((NXOpen.Edge)block1.FindObject("EDGE * 1 * 4 {(0,100,30)(50,100,30)(100,100,30) BLOCK(1)}"));
    NXOpen.Point point1;
    point1 = workPart.Points.CreatePoint(edge1, scalar1, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Edge edge2 = ((NXOpen.Edge)block1.FindObject("EDGE * 1 * 2 {(100,0,30)(50,0,30)(0,0,30) BLOCK(1)}"));
    NXOpen.Direction direction1;
    direction1 = workPart.Directions.CreateDirection(edge2, NXOpen.Sense.Reverse, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Face face1 = ((NXOpen.Face)block1.FindObject("FACE 1 {(50,50,30) BLOCK(1)}"));
    NXOpen.Xform xform1;
    xform1 = workPart.Xforms.CreateXformByPlaneXDirPoint(face1, direction1, point1, NXOpen.SmartObject.UpdateOption.WithinModeling, 0.625, false, false);
    
    NXOpen.CartesianCoordinateSystem cartesianCoordinateSystem1;
    cartesianCoordinateSystem1 = workPart.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
    NXOpen.Features.DatumCsysBuilder datumCsysBuilder1;
    datumCsysBuilder1 = workPart.Features.CreateDatumCsysBuilder(nullNXOpen_Features_Feature);
    
    datumCsysBuilder1.Csys = cartesianCoordinateSystem1;
    
    datumCsysBuilder1.DisplayScaleFactor = 1.25;
    
    NXOpen.Features.Feature feature1;
    feature1 = datumCsysBuilder1.CommitFeature();
    
    datumCsysBuilder1.Destroy();
    
    NXOpen.Sketch nullNXOpen_Sketch = null;
    NXOpen.SketchInPlaceBuilder sketchInPlaceBuilder1;
    sketchInPlaceBuilder1 = workPart.Sketches.CreateSketchInPlaceBuilder2(nullNXOpen_Sketch);
    
    sketchInPlaceBuilder1.Csystem = cartesianCoordinateSystem1;
    
    sketchInPlaceBuilder1.PlaneOption = NXOpen.Sketch.PlaneOption.Inferred;
    
    theSession.Preferences.Sketch.CreateInferredConstraints = false;
    
    theSession.Preferences.Sketch.ContinuousAutoDimensioning = false;
    
    theSession.Preferences.Sketch.DimensionLabel = NXOpen.Preferences.SketchPreferences.DimensionLabelType.Expression;
    
    theSession.Preferences.Sketch.TextSizeFixed = false;
    
    theSession.Preferences.Sketch.FixedTextSize = 3.0;
    
    theSession.Preferences.Sketch.DisplayParenthesesOnReferenceDimensions = true;
    
    theSession.Preferences.Sketch.DisplayReferenceGeometry = false;
    
    theSession.Preferences.Sketch.DisplayShadedRegions = true;
    
    theSession.Preferences.Sketch.FindMovableObjects = true;
    
    theSession.Preferences.Sketch.ConstraintSymbolSize = 3.0;
    
    theSession.Preferences.Sketch.DisplayObjectColor = false;
    
    theSession.Preferences.Sketch.DisplayObjectName = true;
    
    theSession.Preferences.Sketch.EditDimensionOnCreation = true;
    
    theSession.Preferences.Sketch.CreateDimensionForTypedValues = true;
    
    theSession.Preferences.Sketch.AddRemoteFindingAfterSnap = true;
    
    NXOpen.NXObject nXObject1;
    nXObject1 = sketchInPlaceBuilder1.Commit();
    
    sketchInPlaceBuilder1.Destroy();
    
    NXOpen.Point3d coordinates1 = new NXOpen.Point3d(47.370633760382233, 51.674133975223711, 30.0);
    NXOpen.Point point2;
    point2 = workPart.Points.CreatePoint(coordinates1);
    
    NXOpen.Sketch sketch1 = ((NXOpen.Sketch)nXObject1);
    sketch1.Activate(NXOpen.Sketch.ViewReorient.False);
    
    theSession.ActiveSketch.AddGeometry(point2);
    
    theSession.ActiveSketch.Preferences.TextSizeMode = NXOpen.Preferences.SketchPreferences.DimensionTextSizeMode.FixedOnScreen;
    
    NXOpen.Point3d startPoint1 = new NXOpen.Point3d(0.0, 100.0, 30.0);
    NXOpen.Point3d endPoint1 = new NXOpen.Point3d(0.0, 51.674133975223711, 30.0);
    NXOpen.Line line1;
    line1 = workPart.Curves.CreateLine(startPoint1, endPoint1);
    
    NXOpen.Sketch.DimensionGeometry dimObject1_1 = new NXOpen.Sketch.DimensionGeometry();
    dimObject1_1.Geometry = point2;
    dimObject1_1.AssocType = NXOpen.Sketch.AssocType.StartPoint;
    dimObject1_1.AssocValue = 0;
    dimObject1_1.HelpPoint.X = 0.0;
    dimObject1_1.HelpPoint.Y = 0.0;
    dimObject1_1.HelpPoint.Z = 0.0;
    NXOpen.NXObject nullNXOpen_NXObject = null;
    dimObject1_1.View = nullNXOpen_NXObject;
    NXOpen.Sketch.DimensionGeometry dimObject2_1 = new NXOpen.Sketch.DimensionGeometry();
    NXOpen.InfiniteLine infiniteLine1 = ((NXOpen.InfiniteLine)theSession.ActiveSketch.FindObject("YAxis"));
    dimObject2_1.Geometry = infiniteLine1;
    dimObject2_1.AssocType = NXOpen.Sketch.AssocType.StartPoint;
    dimObject2_1.AssocValue = 0;
    dimObject2_1.HelpPoint.X = 0.0;
    dimObject2_1.HelpPoint.Y = 0.0;
    dimObject2_1.HelpPoint.Z = 0.0;
    dimObject2_1.View = nullNXOpen_NXObject;
    NXOpen.Point3d dimOrigin1 = new NXOpen.Point3d(23.685316880191117, 51.674133975223711, 30.0);
    NXOpen.Expression nullNXOpen_Expression = null;
    NXOpen.SketchDimensionalConstraint sketchDimensionalConstraint1;
    sketchDimensionalConstraint1 = theSession.ActiveSketch.CreateDimension(NXOpen.Sketch.ConstraintType.PerpendicularDim, dimObject1_1, dimObject2_1, dimOrigin1, nullNXOpen_Expression);
    
    NXOpen.Point3d startPoint2 = new NXOpen.Point3d(0.0, 100.0, 30.0);
    NXOpen.Point3d endPoint2 = new NXOpen.Point3d(47.370633760382233, 100.0, 30.0);
    NXOpen.Line line2;
    line2 = workPart.Curves.CreateLine(startPoint2, endPoint2);
    
    NXOpen.Sketch.DimensionGeometry dimObject1_2 = new NXOpen.Sketch.DimensionGeometry();
    dimObject1_2.Geometry = point2;
    dimObject1_2.AssocType = NXOpen.Sketch.AssocType.StartPoint;
    dimObject1_2.AssocValue = 0;
    dimObject1_2.HelpPoint.X = 0.0;
    dimObject1_2.HelpPoint.Y = 0.0;
    dimObject1_2.HelpPoint.Z = 0.0;
    dimObject1_2.View = nullNXOpen_NXObject;
    NXOpen.Sketch.DimensionGeometry dimObject2_2 = new NXOpen.Sketch.DimensionGeometry();
    NXOpen.InfiniteLine infiniteLine2 = ((NXOpen.InfiniteLine)theSession.ActiveSketch.FindObject("XAxis"));
    dimObject2_2.Geometry = infiniteLine2;
    dimObject2_2.AssocType = NXOpen.Sketch.AssocType.StartPoint;
    dimObject2_2.AssocValue = 0;
    dimObject2_2.HelpPoint.X = 0.0;
    dimObject2_2.HelpPoint.Y = 0.0;
    dimObject2_2.HelpPoint.Z = 0.0;
    dimObject2_2.View = nullNXOpen_NXObject;
    NXOpen.Point3d dimOrigin2 = new NXOpen.Point3d(47.370633760382233, 75.837066987611848, 30.0);
    NXOpen.SketchDimensionalConstraint sketchDimensionalConstraint2;
    sketchDimensionalConstraint2 = theSession.ActiveSketch.CreateDimension(NXOpen.Sketch.ConstraintType.PerpendicularDim, dimObject1_2, dimObject2_2, dimOrigin2, nullNXOpen_Expression);
    
    theSession.ActiveSketch.UpdateDimensionDisplay();
    
    theSession.ActiveSketch.ShowDimensions();
    
    theSession.ActiveSketch.Update();
    
    NXOpen.SketchFindMovableObjectsBuilder sketchFindMovableObjectsBuilder1;
    sketchFindMovableObjectsBuilder1 = workPart.Sketches.CreateFindMovableObjectsBuilder();
    
    NXOpen.NXObject nXObject2;
    nXObject2 = sketchFindMovableObjectsBuilder1.Commit();
    
    sketchFindMovableObjectsBuilder1.Destroy();
    
    theSession.ActiveSketch.Deactivate(NXOpen.Sketch.ViewReorient.False, NXOpen.Sketch.UpdateLevel.SketchOnly);
    
    NXOpen.Session.UndoMarkId markId1;
    markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);
    
    NXOpen.Features.Feature[] features1 = new NXOpen.Features.Feature[1];
    NXOpen.Features.SketchFeature sketchFeature1 = ((NXOpen.Features.SketchFeature)workPart.Features.FindObject("SKETCH(3)"));
    features1[0] = sketchFeature1;
    NXOpen.DisplayableObject nullNXOpen_DisplayableObject = null;
    NXOpen.FeaturePointsRule featurePointsRule1;
    featurePointsRule1 = workPart.ScRuleFactory.CreateRuleFeaturePoints(features1, nullNXOpen_DisplayableObject);
    
    NXOpen.Section section1 = ((NXOpen.Section)workPart.FindObject("ENTITY 113 2"));
    section1.AllowSelfIntersection(true);
    
    section1.AllowDegenerateCurves(false);
    
    NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
    rules1[0] = featurePointsRule1;
    NXOpen.Point3d helpPoint1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
    section1.AddToSection(rules1, nullNXOpen_NXObject, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
    
    theSession.DeleteUndoMark(markId1, null);
    
    NXOpen.NXObject[] refs1;
    section1.EvaluateAndAskOutputEntities(out refs1);
    
    // can not output code to find <JABL>: <UGS::BOOLEAN_data> &00007FF4CD5C4010 [67218]
    NXOpen.GeometricUtilities.BooleanOperation nullNXOpen_GeometricUtilities_BooleanOperation = null;
    NXOpen.ScCollector scCollector1;
    scCollector1 = nullNXOpen_GeometricUtilities_BooleanOperation.GetTargetBodiesCollector();
    
    NXOpen.SelectionIntentRule[] rules2 = new NXOpen.SelectionIntentRule[0];
    scCollector1.ReplaceRules(rules2, false);
    
    NXOpen.SelectionIntentRuleOptions selectionIntentRuleOptions1;
    selectionIntentRuleOptions1 = workPart.ScRuleFactory.CreateRuleOptions();
    
    selectionIntentRuleOptions1.SetSelectedFromInactive(false);
    
    NXOpen.Body[] bodies1 = new NXOpen.Body[1];
    NXOpen.Body body1 = ((NXOpen.Body)workPart.Bodies.FindObject("BLOCK(1)"));
    bodies1[0] = body1;
    NXOpen.BodyDumbRule bodyDumbRule1;
    bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true, selectionIntentRuleOptions1);
    
    selectionIntentRuleOptions1.Dispose();
    NXOpen.SelectionIntentRule[] rules3 = new NXOpen.SelectionIntentRule[1];
    rules3[0] = bodyDumbRule1;
    scCollector1.ReplaceRules(rules3, false);
    
    NXOpen.Session.UndoMarkId markId2;
    markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "孔");
    
    NXOpen.DisplayableObject[] objects1 = new NXOpen.DisplayableObject[2];
    objects1[0] = line2;
    objects1[1] = line1;
    theSession.DisplayManager.BlankObjects(objects1);
    
    int nErrs1;
    nErrs1 = theSession.UpdateManager.AddToDeleteList(line2);
    
    int nErrs2;
    nErrs2 = theSession.UpdateManager.AddToDeleteList(line1);
    
    theSession.DeleteUndoMark(markId2, null);
    
    NXOpen.Session.UndoMarkId markId3;
    markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "孔");
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD54FA68 [67201]
    NXOpen.Features.FeatureBuilder nullNXOpen_Features_FeatureBuilder = null;
    nullNXOpen_Features_FeatureBuilder.ParentFeatureInternal = true;
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD54FA68 [67201]
    NXOpen.Features.HolePackageBuilder nullNXOpen_Features_HolePackageBuilder = null;
    nullNXOpen_Features_HolePackageBuilder.ScrewType = "Socket Head, 4762";
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD54FA68 [67201]
    nullNXOpen_Features_HolePackageBuilder.ScrewSize = "M6";
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD54FA68 [67201]
    nullNXOpen_Features_HolePackageBuilder.ScrewFitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4CCCA5550 [67219]
    NXOpen.GeometricUtilities.StartHoleData nullNXOpen_GeometricUtilities_StartHoleData = null;
    nullNXOpen_GeometricUtilities_StartHoleData.ScrewType = "General Screw Clearance";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4CCCA5550 [67219]
    nullNXOpen_GeometricUtilities_StartHoleData.ScrewSize = "M10";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4CCCA5550 [67219]
    nullNXOpen_GeometricUtilities_StartHoleData.FitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::MiddleHoleData> &00007FF4CCD31218 [65521]
    NXOpen.GeometricUtilities.MiddleHoleData nullNXOpen_GeometricUtilities_MiddleHoleData = null;
    nullNXOpen_GeometricUtilities_MiddleHoleData.FitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::EndHoleData> &00007FF4D61422C0 [66572]
    NXOpen.GeometricUtilities.EndHoleData nullNXOpen_GeometricUtilities_EndHoleData = null;
    nullNXOpen_GeometricUtilities_EndHoleData.FitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD54FA68 [67201]
    NXOpen.Builder nullNXOpen_Builder = null;
    NXOpen.NXObject nXObject3;
    nXObject3 = nullNXOpen_Builder.Commit();
    
    theSession.DeleteUndoMark(markId3, null);
    
    theSession.SetUndoMarkName((NXOpen.Session.UndoMarkId)(1871), "孔");
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD54FA68 [67201]
    nullNXOpen_Builder.Destroy();
    
    workPart.MeasureManager.SetPartTransientModification();
    
    NXOpen.Expression expression1 = ((NXOpen.Expression)workPart.Expressions.FindObject("p126"));
    workPart.Expressions.Delete(expression1);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    
  }
  public static int GetUnloadOption(string dummy) { return (int)NXOpen.Session.LibraryUnloadOption.Immediately; }
}
