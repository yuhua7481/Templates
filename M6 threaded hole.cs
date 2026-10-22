// Designcenter 2512
// Journal created by yuhua on Thu Oct 22 16:55:46 2026 中国标准时间

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
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD5EA758 [67492]
    NXOpen.Features.HolePackageBuilder nullNXOpen_Features_HolePackageBuilder = null;
    nullNXOpen_Features_HolePackageBuilder.TypeOfHole = NXOpen.Features.HolePackageBuilder.TypesOfHole.Countersink;
    
    NXOpen.Expression expression1 = ((NXOpen.Expression)workPart.Expressions.FindObject("p179"));
    expression1.SetFormula("13.71");
    
    NXOpen.Expression expression2 = ((NXOpen.Expression)workPart.Expressions.FindObject("p181"));
    expression2.SetFormula("0.8");
    
    NXOpen.Session.UndoMarkId markId1;
    markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "孔");
    
    NXOpen.DisplayableObject[] objects1 = new NXOpen.DisplayableObject[2];
    NXOpen.Line line1 = ((NXOpen.Line)workPart.Lines.FindObject("ENTITY 3 1 1"));
    objects1[0] = line1;
    NXOpen.Line line2 = ((NXOpen.Line)workPart.Lines.FindObject("ENTITY 3 2 1"));
    objects1[1] = line2;
    theSession.DisplayManager.BlankObjects(objects1);
    
    int nErrs1;
    nErrs1 = theSession.UpdateManager.AddToDeleteList(line1);
    
    int nErrs2;
    nErrs2 = theSession.UpdateManager.AddToDeleteList(line2);
    
    theSession.DeleteUndoMark(markId1, null);
    
    NXOpen.Session.UndoMarkId markId2;
    markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "孔");
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD5EA758 [67492]
    nullNXOpen_Features_HolePackageBuilder.ScrewType = "Socket Flat Head, 10642";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4CD585AE8 [67499]
    NXOpen.GeometricUtilities.StartHoleData nullNXOpen_GeometricUtilities_StartHoleData = null;
    nullNXOpen_GeometricUtilities_StartHoleData.ScrewType = "Socket Head, 4762";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4CD585AE8 [67499]
    nullNXOpen_GeometricUtilities_StartHoleData.ScrewSize = "M1.6";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::StartHoleData> &00007FF4CD585AE8 [67499]
    nullNXOpen_GeometricUtilities_StartHoleData.FitOption = "Close (H12)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::MiddleHoleData> &00007FF4CD585380 [66560]
    NXOpen.GeometricUtilities.MiddleHoleData nullNXOpen_GeometricUtilities_MiddleHoleData = null;
    nullNXOpen_GeometricUtilities_MiddleHoleData.FitOption = "Close (H12)";
    
    // can not output code to find <JABL>: <UGS::ModlUtilsJA::EndHoleData> &00007FF4CD5EB330 [66587]
    NXOpen.GeometricUtilities.EndHoleData nullNXOpen_GeometricUtilities_EndHoleData = null;
    nullNXOpen_GeometricUtilities_EndHoleData.FitOption = "Close (H12)";
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD5EA758 [67492]
    NXOpen.Builder nullNXOpen_Builder = null;
    NXOpen.NXObject nXObject1;
    nXObject1 = nullNXOpen_Builder.Commit();
    
    theSession.DeleteUndoMark(markId2, null);
    
    theSession.SetUndoMarkName((NXOpen.Session.UndoMarkId)(1961), "孔");
    
    NXOpen.Section section1 = ((NXOpen.Section)workPart.FindObject("ENTITY 113 4"));
    section1.CleanMappingData();
    
    // can not output code to find <JABL>: <UGS::ModlFeatureJA::HolePackageBuilder> &00007FF4CD5EA758 [67492]
    nullNXOpen_Builder.Destroy();
    
    theSession.DeleteUndoMark((NXOpen.Session.UndoMarkId)(1961), null);
    
    NXOpen.Features.EditWithRollbackManager editWithRollbackManager1 = ((NXOpen.Features.EditWithRollbackManager)workPart.Find("EditWithRollbackData {model2}"));
    editWithRollbackManager1.UpdateFeature(false);
    
    editWithRollbackManager1.Stop();
    
    editWithRollbackManager1.Destroy();
    
    
  }
  public static int GetUnloadOption(string dummy) { return (int)NXOpen.Session.LibraryUnloadOption.Immediately; }
}
