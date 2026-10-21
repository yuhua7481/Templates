// Designcenter 2512
// Journal created by yuhua on Wed Oct 21 20:01:04 2026 中国标准时间

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
    NXOpen.Session.UndoMarkId markId1;
    markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Enter Sketch");
    
    theSession.BeginTaskEnvironment();
    
    NXOpen.Session.UndoMarkId markId2;
    markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "起点");
    
    NXOpen.Sketch nullNXOpen_Sketch = null;
    NXOpen.SketchInPlaceBuilder sketchInPlaceBuilder1;
    sketchInPlaceBuilder1 = workPart.Sketches.CreateSketchInPlaceBuilder2(nullNXOpen_Sketch);
    
    NXOpen.Point3d origin1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
    NXOpen.Vector3d normal1 = new NXOpen.Vector3d(0.0, 0.0, 1.0);
    NXOpen.Plane plane1;
    plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    sketchInPlaceBuilder1.PlaneReference = plane1;
    
    NXOpen.Unit unit1 = ((NXOpen.Unit)workPart.UnitCollection.FindObject("MilliMeter"));
    NXOpen.Expression expression1;
    expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.Expression expression2;
    expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.SketchAlongPathBuilder sketchAlongPathBuilder1;
    sketchAlongPathBuilder1 = workPart.Sketches.CreateSketchAlongPathBuilder(nullNXOpen_Sketch);
    
    NXOpen.SimpleSketchInPlaceBuilder simpleSketchInPlaceBuilder1;
    simpleSketchInPlaceBuilder1 = workPart.Sketches.CreateSimpleSketchInPlaceBuilder();
    
    sketchAlongPathBuilder1.PlaneLocation.Expression.SetFormula("0");
    
    theSession.SetUndoMarkName(markId2, "创建草图 对话框");
    
    simpleSketchInPlaceBuilder1.UseWorkPartOrigin = false;
    
    NXOpen.Scalar scalar1;
    scalar1 = workPart.Scalars.CreateScalar(0.0, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Features.Block block1 = ((NXOpen.Features.Block)workPart.Features.FindObject("BLOCK(1)"));
    NXOpen.Edge edge1 = ((NXOpen.Edge)block1.FindObject("EDGE * 2 * 5 {(30,0,0)(60,0,0)(90,0,0) BLOCK(1)}"));
    NXOpen.Point point1;
    point1 = workPart.Points.CreatePoint(edge1, scalar1, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Direction direction1;
    direction1 = workPart.Directions.CreateDirection(edge1, NXOpen.Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Face face1 = ((NXOpen.Face)block1.FindObject("FACE 5 {(60,20,0) BLOCK(1)}"));
    NXOpen.Xform xform1;
    xform1 = workPart.Xforms.CreateXformByPlaneXDirPoint(face1, direction1, point1, NXOpen.SmartObject.UpdateOption.WithinModeling, 0.625, false, false);
    
    NXOpen.CartesianCoordinateSystem cartesianCoordinateSystem1;
    cartesianCoordinateSystem1 = workPart.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    simpleSketchInPlaceBuilder1.CoordinateSystem = cartesianCoordinateSystem1;
    
    simpleSketchInPlaceBuilder1.HorizontalReference.Value = edge1;
    
    NXOpen.Point point2;
    point2 = simpleSketchInPlaceBuilder1.SketchOrigin;
    
    simpleSketchInPlaceBuilder1.SketchOrigin = point2;
    
    NXOpen.Scalar scalar2;
    scalar2 = workPart.Scalars.CreateScalar(0.0, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Point nullNXOpen_Point = null;
    NXOpen.Point point3;
    point3 = workPart.Points.CreatePoint(edge1, scalar2, NXOpen.PointCollection.PointOnCurveLocationOption.PercentParameter, nullNXOpen_Point, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Expression expression3;
    expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.Scalar scalar3;
    scalar3 = workPart.Scalars.CreateScalar(0.0, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Point point4;
    point4 = workPart.Points.CreatePoint(edge1, scalar3, NXOpen.PointCollection.PointOnCurveLocationOption.PercentParameter, nullNXOpen_Point, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Session.UndoMarkId markId3;
    markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "起点");
    
    NXOpen.Expression expression4;
    expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    string newsystemname1;
    newsystemname1 = workPart.Expressions.CreateSystemName("", "_x");
    
    NXOpen.Expression expression5;
    expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("p128_x=30.00000000000", unit1);
    
    string newsystemname2;
    newsystemname2 = workPart.Expressions.CreateSystemName("", "_y");
    
    NXOpen.Expression expression6;
    expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p129_y=0.00000000000", unit1);
    
    string newsystemname3;
    newsystemname3 = workPart.Expressions.CreateSystemName("", "_z");
    
    NXOpen.Expression expression7;
    expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p130_z=0.00000000000", unit1);
    
    string newsystemname4;
    newsystemname4 = workPart.Expressions.CreateSystemName("", "_xdelta");
    
    NXOpen.Expression expression8;
    expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p131_xdelta=0", unit1);
    
    string newsystemname5;
    newsystemname5 = workPart.Expressions.CreateSystemName("", "_ydelta");
    
    NXOpen.Expression expression9;
    expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("p132_ydelta=0", unit1);
    
    string newsystemname6;
    newsystemname6 = workPart.Expressions.CreateSystemName("", "_zdelta");
    
    NXOpen.Expression expression10;
    expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("p133_zdelta=0", unit1);
    
    string newsystemname7;
    newsystemname7 = workPart.Expressions.CreateSystemName("", "_radius");
    
    NXOpen.Expression expression11;
    expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("p134_radius=0", unit1);
    
    string newsystemname8;
    newsystemname8 = workPart.Expressions.CreateSystemName("", "_angle");
    
    NXOpen.Unit unit2 = ((NXOpen.Unit)workPart.UnitCollection.FindObject("Degrees"));
    NXOpen.Expression expression12;
    expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("p135_angle=0", unit2);
    
    string newsystemname9;
    newsystemname9 = workPart.Expressions.CreateSystemName("", "_zdelta");
    
    NXOpen.Expression expression13;
    expression13 = workPart.Expressions.CreateSystemExpressionWithUnits("p136_zdelta=0", unit1);
    
    string newsystemname10;
    newsystemname10 = workPart.Expressions.CreateSystemName("", "_radius");
    
    NXOpen.Expression expression14;
    expression14 = workPart.Expressions.CreateSystemExpressionWithUnits("p137_radius=0", unit1);
    
    string newsystemname11;
    newsystemname11 = workPart.Expressions.CreateSystemName("", "_angle1");
    
    NXOpen.Expression expression15;
    expression15 = workPart.Expressions.CreateSystemExpressionWithUnits("p138_angle1=0", unit2);
    
    string newsystemname12;
    newsystemname12 = workPart.Expressions.CreateSystemName("", "_angle2");
    
    NXOpen.Expression expression16;
    expression16 = workPart.Expressions.CreateSystemExpressionWithUnits("p139_angle2=0", unit2);
    
    string newsystemname13;
    newsystemname13 = workPart.Expressions.CreateSystemName("", "_distance");
    
    NXOpen.Expression expression17;
    expression17 = workPart.Expressions.CreateSystemExpressionWithUnits("p140_distance=0", unit1);
    
    string newsystemname14;
    newsystemname14 = workPart.Expressions.CreateSystemName("", "_arclen");
    
    NXOpen.Expression expression18;
    expression18 = workPart.Expressions.CreateSystemExpressionWithUnits("p141_arclen=0", unit1);
    
    string newsystemname15;
    newsystemname15 = workPart.Expressions.CreateSystemName("", "_percent");
    
    NXOpen.Unit nullNXOpen_Unit = null;
    NXOpen.Expression expression19;
    expression19 = workPart.Expressions.CreateSystemExpressionWithUnits("p142_percent=0", nullNXOpen_Unit);
    
    theSession.SetUndoMarkName(markId3, "点 对话框");
    
    expression5.SetFormula("30");
    
    expression6.SetFormula("0");
    
    expression7.SetFormula("0");
    
    expression5.SetFormula("30.00000000000");
    
    expression6.SetFormula("0.00000000000");
    
    expression7.SetFormula("0.00000000000");
    
    int nErrs1;
    nErrs1 = theSession.UpdateManager.AddToDeleteList(point4);
    
    NXOpen.Expression expression20;
    expression20 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.Edge edge2 = ((NXOpen.Edge)block1.FindObject("EDGE * 3 * 5 {(30,40,0)(30,20,0)(30,0,0) BLOCK(1)}"));
    NXOpen.Point point5;
    point5 = workPart.Points.CreateCurveMidPoint(edge2, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Point point6;
    point6 = workPart.Points.CreateCurveMidPoint(edge2, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    point6.RemoveViewDependency();
    
    point6.RemoveViewDependency();
    
    NXOpen.Expression expression21;
    expression21 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.Edge edge3 = ((NXOpen.Edge)block1.FindObject("EDGE * 5 * 6 {(90,0,0)(90,20,0)(90,40,0) BLOCK(1)}"));
    NXOpen.Point point7;
    point7 = workPart.Points.CreateCurveMidPoint(edge3, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Point point8;
    point8 = workPart.Points.CreateCurveMidPoint(edge3, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    point8.RemoveViewDependency();
    
    point8.RemoveViewDependency();
    
    NXOpen.Expression expression22;
    expression22 = workPart.Expressions.CreateSystemExpressionWithUnits("50", nullNXOpen_Unit);
    
    expression22.RightHandSide = "0.50000000000";
    
    NXOpen.Scalar scalar4;
    scalar4 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Point point9;
    point9 = workPart.Points.CreatePoint(point6, point8, scalar4, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    expression5.SetFormula("60");
    
    expression6.SetFormula("20");
    
    expression7.SetFormula("0");
    
    expression5.SetFormula("60.00000000000");
    
    expression6.SetFormula("20.00000000000");
    
    expression7.SetFormula("0.00000000000");
    
    NXOpen.Session.UndoMarkId markId4;
    markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "点");
    
    theSession.DeleteUndoMark(markId4, null);
    
    NXOpen.Session.UndoMarkId markId5;
    markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "点");
    
    workPart.Points.DeletePoint(point5);
    
    workPart.Points.DeletePoint(point7);
    
    theSession.DeleteUndoMark(markId5, null);
    
    theSession.SetUndoMarkName(markId3, "点");
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression5);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression6);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression7);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression8);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression9);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression10);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression11);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression12);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression13);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression14);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression15);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression16);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression17);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression18);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression19);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression4);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression20);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression21);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    theSession.DeleteUndoMark(markId3, null);
    
    workPart.Points.DeletePoint(point3);
    
    simpleSketchInPlaceBuilder1.SketchOrigin = point9;
    
    workPart.Points.DeletePoint(point2);
    
    NXOpen.Point point10;
    point10 = workPart.Points.CreatePoint(point6, point8, scalar4, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Session.UndoMarkId markId6;
    markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "创建草图");
    
    theSession.DeleteUndoMark(markId6, null);
    
    NXOpen.Session.UndoMarkId markId7;
    markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "创建草图");
    
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
    nXObject1 = simpleSketchInPlaceBuilder1.Commit();
    
    NXOpen.Sketch sketch1 = ((NXOpen.Sketch)nXObject1);
    NXOpen.Features.Feature feature1;
    feature1 = sketch1.Feature;
    
    NXOpen.Session.UndoMarkId markId8;
    markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "update");
    
    int nErrs2;
    nErrs2 = theSession.UpdateManager.DoUpdate(markId8);
    
    sketch1.Activate(NXOpen.Sketch.ViewReorient.True);
    
    theSession.Preferences.Sketch.FindMovableObjects = true;
    
    NXOpen.SketchFindMovableObjectsBuilder sketchFindMovableObjectsBuilder1;
    sketchFindMovableObjectsBuilder1 = workPart.Sketches.CreateFindMovableObjectsBuilder();
    
    NXOpen.NXObject nXObject2;
    nXObject2 = sketchFindMovableObjectsBuilder1.Commit();
    
    sketchFindMovableObjectsBuilder1.Destroy();
    
    NXOpen.SketchFindIssuesBuilder sketchFindIssuesBuilder1;
    sketchFindIssuesBuilder1 = workPart.Sketches.CreateFindIssuesBuilder();
    
    sketchFindIssuesBuilder1.ZoomScale = 1.5179205639769686;
    
    NXOpen.NXObject nXObject3;
    nXObject3 = sketchFindIssuesBuilder1.Commit();
    
    sketchFindIssuesBuilder1.Destroy();
    
    workPart.ModelingViews.WorkView.SetScale(1.5179205639769686);
    
    theSession.ActiveSketch.Preferences.DisplayReferenceGeometry = true;
    
    theSession.DeleteUndoMark(markId7, null);
    
    theSession.SetUndoMarkName(markId2, "创建草图");
    
    sketchInPlaceBuilder1.Destroy();
    
    sketchAlongPathBuilder1.Destroy();
    
    simpleSketchInPlaceBuilder1.Destroy();
    
    workPart.Points.DeletePoint(point10);
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression2);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression3);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    try
    {
      // 表达式仍然在使用中。
      workPart.Expressions.Delete(expression1);
    }
    catch (NXException ex)
    {
      ex.AssertErrorCode(1050029);
    }
    
    plane1.DestroyPlane();
    
    theSession.DeleteUndoMarksUpToMark(markId2, null, true);
    
    NXOpen.Session.UndoMarkId markId9;
    markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Open Sketch");
    
    theSession.ActiveSketch.SetName("SKETCH_000");
    
    NXOpen.Session.UndoMarkId markId10;
    markId10 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "起点");
    
    theSession.SetUndoMarkName(markId10, "草图点 对话框");
    
    NXOpen.Expression expression23;
    expression23 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.Point point11 = ((NXOpen.Point)theSession.ActiveSketch.FindObject("Origin"));
    NXOpen.Xform nullNXOpen_Xform = null;
    NXOpen.Point point12;
    point12 = workPart.Points.CreatePoint(point11, nullNXOpen_Xform, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Session.UndoMarkId markId11;
    markId11 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "草图点");
    
    point12.RemoveParameters();
    
    NXOpen.Point3d coordinates1 = new NXOpen.Point3d(60.0, 20.0, 0.0);
    point12.SetCoordinates(coordinates1);
    
    theSession.ActiveSketch.AddGeometry(point12, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
    
    theSession.ActiveSketch.Update();
    
    NXOpen.SketchFindMovableObjectsBuilder sketchFindMovableObjectsBuilder2;
    sketchFindMovableObjectsBuilder2 = workPart.Sketches.CreateFindMovableObjectsBuilder();
    
    NXOpen.NXObject nXObject4;
    nXObject4 = sketchFindMovableObjectsBuilder2.Commit();
    
    sketchFindMovableObjectsBuilder2.Destroy();
    
    NXOpen.SketchFindIssuesBuilder sketchFindIssuesBuilder2;
    sketchFindIssuesBuilder2 = workPart.Sketches.CreateFindIssuesBuilder();
    
    sketchFindIssuesBuilder2.ZoomScale = 1.5179205639769686;
    
    NXOpen.NXObject nXObject5;
    nXObject5 = sketchFindIssuesBuilder2.Commit();
    
    sketchFindIssuesBuilder2.Destroy();
    
    theSession.DeleteUndoMark(markId11, null);
    
    theSession.SetUndoMarkName(markId10, "草图点");
    
    theSession.SetUndoMarkVisibility(markId10, null, NXOpen.Session.MarkVisibility.Visible);
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression23);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    NXOpen.Session.UndoMarkId markId12;
    markId12 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");
    
    theSession.SetUndoMarkName(markId12, "草图点 对话框");
    
    NXOpen.Expression expression24;
    expression24 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    NXOpen.Point point13;
    point13 = workPart.Points.CreatePoint(point11, nullNXOpen_Xform, NXOpen.SmartObject.UpdateOption.WithinModeling);
    
    NXOpen.Session.UndoMarkId markId13;
    markId13 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "草图点");
    
    point13.RemoveParameters();
    
    NXOpen.Point3d coordinates2 = new NXOpen.Point3d(60.0, 20.0, 0.0);
    point13.SetCoordinates(coordinates2);
    
    theSession.ActiveSketch.AddGeometry(point13, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
    
    theSession.ActiveSketch.Update();
    
    NXOpen.SketchFindMovableObjectsBuilder sketchFindMovableObjectsBuilder3;
    sketchFindMovableObjectsBuilder3 = workPart.Sketches.CreateFindMovableObjectsBuilder();
    
    NXOpen.NXObject nXObject6;
    nXObject6 = sketchFindMovableObjectsBuilder3.Commit();
    
    sketchFindMovableObjectsBuilder3.Destroy();
    
    NXOpen.SketchFindIssuesBuilder sketchFindIssuesBuilder3;
    sketchFindIssuesBuilder3 = workPart.Sketches.CreateFindIssuesBuilder();
    
    sketchFindIssuesBuilder3.ZoomScale = 1.5179205639769686;
    
    NXOpen.NXObject nXObject7;
    nXObject7 = sketchFindIssuesBuilder3.Commit();
    
    sketchFindIssuesBuilder3.Destroy();
    
    theSession.DeleteUndoMark(markId13, null);
    
    theSession.SetUndoMarkName(markId12, "草图点");
    
    theSession.SetUndoMarkVisibility(markId12, null, NXOpen.Session.MarkVisibility.Visible);
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression24);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    NXOpen.Session.UndoMarkId markId14;
    markId14 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");
    
    theSession.SetUndoMarkName(markId14, "草图点 对话框");
    
    NXOpen.Expression expression25;
    expression25 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
    
    theSession.SetUndoMarkName(markId14, "草图点");
    
    workPart.MeasureManager.SetPartTransientModification();
    
    workPart.Expressions.Delete(expression25);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    theSession.DeleteUndoMark(markId14, null);
    
    theSession.ActiveSketch.CalculateStatus();
    
    theSession.Preferences.Sketch.SectionView = false;
    
    NXOpen.Session.UndoMarkId markId15;
    markId15 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Deactivate Sketch");
    
    theSession.ActiveSketch.Deactivate(NXOpen.Sketch.ViewReorient.True, NXOpen.Sketch.UpdateLevel.SketchOnly);
    
    theSession.DeleteUndoMarksSetInTaskEnvironment();
    
    theSession.EndTaskEnvironment();
    
    theSession.DeleteUndoMark(markId1, null);
    
    NXOpen.Section section1 = ((NXOpen.Section)workPart.FindObject("ENTITY 113 2"));
    section1.DistanceTolerance = 0.01;
    
    section1.ChainingTolerance = 0.0094999999999999998;
    
    NXOpen.Session.UndoMarkId markId16;
    markId16 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);
    
    NXOpen.Features.Feature[] features1 = new NXOpen.Features.Feature[1];
    NXOpen.Features.SketchFeature sketchFeature1 = ((NXOpen.Features.SketchFeature)feature1);
    features1[0] = sketchFeature1;
    NXOpen.DisplayableObject nullNXOpen_DisplayableObject = null;
    NXOpen.FeaturePointsRule featurePointsRule1;
    featurePointsRule1 = workPart.ScRuleFactory.CreateRuleFeaturePoints(features1, nullNXOpen_DisplayableObject);
    
    section1.AllowSelfIntersection(true);
    
    section1.AllowDegenerateCurves(false);
    
    NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
    rules1[0] = featurePointsRule1;
    NXOpen.NXObject nullNXOpen_NXObject = null;
    NXOpen.Point3d helpPoint1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
    section1.AddToSection(rules1, nullNXOpen_NXObject, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
    
    theSession.DeleteUndoMark(markId16, null);
    
    NXOpen.NXObject[] refs1;
    section1.EvaluateAndAskOutputEntities(out refs1);
    
    // can not output code to find <JABL>: <UGS::BOOLEAN_data> &00007FF4EB2ADFF8 [63571]
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
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4EB2AD1E0 [63562]
    NXOpen.Features.HolePackageBuilder nullNXOpen_Features_HolePackageBuilder = null;
    nullNXOpen_Features_HolePackageBuilder.SizeOfHole = NXOpen.Features.HolePackageBuilder.SizesOfHole.Custom;
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4EB2AD1E0 [63562]
    nullNXOpen_Features_HolePackageBuilder.TypeOfHole = NXOpen.Features.HolePackageBuilder.TypesOfHole.ThreadedHole;
    
    NXOpen.Expression expression26 = ((NXOpen.Expression)workPart.Expressions.FindObject("p53"));
    expression26.SetFormula("5");
    
    NXOpen.Expression expression27 = ((NXOpen.Expression)workPart.Expressions.FindObject("p86"));
    expression27.SetFormula("3");
    
    NXOpen.Expression expression28 = ((NXOpen.Expression)workPart.Expressions.FindObject("p83"));
    expression28.SetFormula("6");
    
    NXOpen.Expression expression29 = ((NXOpen.Expression)workPart.Expressions.FindObject("p91"));
    expression29.SetFormula("0.4");
    
    NXOpen.Expression expression30 = ((NXOpen.Expression)workPart.Expressions.FindObject("p93"));
    expression30.SetFormula("6");
    
    NXOpen.Expression expression31 = ((NXOpen.Expression)workPart.Expressions.FindObject("p99"));
    expression31.SetFormula("6");
    
    NXOpen.Expression expression32 = ((NXOpen.Expression)workPart.Expressions.FindObject("p81"));
    expression32.SetFormula("12");
    
    NXOpen.Expression expression33 = ((NXOpen.Expression)workPart.Expressions.FindObject("p54"));
    expression33.SetFormula("9");
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4EB2AD1E0 [63562]
    nullNXOpen_Features_HolePackageBuilder.ThreadSize = "M6 x 1.0";
    
    expression32.SetFormula("12");
    
    expression33.SetFormula("20");
    
    expression32.SetFormula("20");
    
    expression32.SetFormula("20");
    
    NXOpen.Session.UndoMarkId markId17;
    markId17 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "孔");
    
    theSession.DeleteUndoMark(markId17, null);
    
    NXOpen.Session.UndoMarkId markId18;
    markId18 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "孔");
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4EB2AD1E0 [63562]
    NXOpen.Features.FeatureBuilder nullNXOpen_Features_FeatureBuilder = null;
    nullNXOpen_Features_FeatureBuilder.ParentFeatureInternal = true;
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4EB2A9728 [63572]
    NXOpen.GeometricUtilities.StartHoleData nullNXOpen_GeometricUtilities_StartHoleData = null;
    nullNXOpen_GeometricUtilities_StartHoleData.ScrewType = "General Screw Clearance";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4EB2A9728 [63572]
    nullNXOpen_GeometricUtilities_StartHoleData.ScrewSize = "M10";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4EB2A9728 [63572]
    nullNXOpen_GeometricUtilities_StartHoleData.FitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::MiddleHoleData> &00007FF4D8F7F0B8 [63822]
    NXOpen.GeometricUtilities.MiddleHoleData nullNXOpen_GeometricUtilities_MiddleHoleData = null;
    nullNXOpen_GeometricUtilities_MiddleHoleData.FitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::EndHoleData> &00007FF4D8FA0430 [63848]
    NXOpen.GeometricUtilities.EndHoleData nullNXOpen_GeometricUtilities_EndHoleData = null;
    nullNXOpen_GeometricUtilities_EndHoleData.FitOption = "Normal (H13)";
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4EB2AD1E0 [63562]
    NXOpen.Builder nullNXOpen_Builder = null;
    NXOpen.NXObject nXObject8;
    nXObject8 = nullNXOpen_Builder.Commit();
    
    theSession.DeleteUndoMark(markId18, null);
    
    theSession.SetUndoMarkName((NXOpen.Session.UndoMarkId)(250), "孔");
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4EB2AD1E0 [63562]
    nullNXOpen_Builder.Destroy();
    
    workPart.MeasureManager.SetPartTransientModification();
    
    NXOpen.Expression expression34 = ((NXOpen.Expression)workPart.Expressions.FindObject("p122"));
    workPart.Expressions.Delete(expression34);
    
    workPart.MeasureManager.ClearPartTransientModification();
    
    
  }
  public static int GetUnloadOption(string dummy) { return (int)NXOpen.Session.LibraryUnloadOption.Immediately; }
}
