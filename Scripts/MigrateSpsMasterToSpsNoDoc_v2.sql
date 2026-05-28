-- MigrateSpsMasterToSpsNoDoc_v2.sql
BEGIN TRANSACTION;

-- 1. Insert ke SpsNoDocs untuk yang Punya Document Number (Di-group)
INSERT INTO SpsNoDocs (
    ExcelId, No, Machine, DocumentNumber, RevisionNumber, Customer, RevisionDate, Formulasi, HoseType, Dimensi, Material,
    InnerTube, OuterCover, MiddleTube, UseLimitsInner, UseLimitsOuter, UseLimitsMiddle, Nipple, TubeDie, CoverDie, MiddleDie, SpacerDie, ADistance,
    Yarn, TensionYarnInner, TensionYarnOuter, MeshDim1, MeshScreen1, MeshDim2, MeshScreen2, MeshDim3, MeshScreen3,
    HeadTemp1, HeadTemp2, Cylinder1_1, Cylinder1_2, Cylinder2_1, Cylinder2_2, Cylinder3_1, Cylinder3_2, Cylinder3_3, HeadTemp3, Cylinder1_3, Cylinder2_3, ScrewTemp3,
    Feed1, Feed2, ScrewTemp1, ScrewTemp2, ScrewSpeed1, ScrewSpeed2, Pressure1, Pressure2, Feed3, ScrewSpeed3, Pressure3,
    AmMeter, OdSensor, MarkingSort, TextMarkingMaterial, MarkingColour, ChillerWaterTemp, CuttingSpeed, TakeUpConveyorSpeed,
    ToleranceInner, ToleranceOuter, TebalInner, TebalInnerMiddle, TebalOuter, TebalTotal, SelisihTebal, ToleranceSpiralPitch, MachineCode,
    PitchYarn, FeedRollRatio1, FeedRollRatio2, FeedRollRatio3, CurrentValue, AmMeter2, AmMeter3, PresetValue, ControlValue, SpiralPitchSetting, SpiralPitchDisplay,
    SpiralSpeed, HoseSpeed, UnsmoothSurface, DancerPosition, CaterpillarGap, CoolConveyorSpeed, CoolConveyorSpeed2, ConveyorRatio,
    InnerTarget, InnerTol, InnerLCL, InnerMin, InnerUCL, InnerMax, InnerMidTarget, InnerMidTol, InnerMidLCL, InnerMidMin, InnerMidUCL, InnerMidMax,
    ThickTarget, ThickTol, ThickLCL, ThickMin, ThickUCL, ThickMax, TotalTarget, TotalTol, TotalLCL, TotalMin, TotalUCL, TotalMax,
    Nipple_Min, Nipple_Asli, Nipple_Max, TubeDie_Min, TubeDie_Asli, TubeDie_Max, CoverDie_Min, CoverDie_Asli, CoverDie_Max, MiddleDie_Min, MiddleDie_Asli, MiddleDie_Max, SpacerDie_Min, SpacerDie_Asli, SpacerDie_Max, ADistance_Min, ADistance_Asli, ADistance_Max,
    MeshDim1_Min, MeshDim1_Asli, MeshDim1_Max, MeshDim2_Min, MeshDim2_Asli, MeshDim2_Max, MeshDim3_Min, MeshDim3_Asli, MeshDim3_Max,
    HeadTemp1_Min, HeadTemp1_Asli, HeadTemp1_Max, HeadTemp2_Min, HeadTemp2_Asli, HeadTemp2_Max, HeadTemp3_Min, HeadTemp3_Asli, HeadTemp3_Max,
    Cylinder1_1_Min, Cylinder1_1_Asli, Cylinder1_1_Max, Cylinder1_2_Min, Cylinder1_2_Asli, Cylinder1_2_Max, Cylinder1_3_Min, Cylinder1_3_Asli, Cylinder1_3_Max,
    Cylinder2_1_Min, Cylinder2_1_Asli, Cylinder2_1_Max, Cylinder2_2_Min, Cylinder2_2_Asli, Cylinder2_2_Max, Cylinder2_3_Min, Cylinder2_3_Asli, Cylinder2_3_Max,
    Cylinder3_1_Min, Cylinder3_1_Asli, Cylinder3_1_Max, Cylinder3_2_Min, Cylinder3_2_Asli, Cylinder3_2_Max, Cylinder3_3_Min, Cylinder3_3_Asli, Cylinder3_3_Max,
    ScrewTemp1_Min, ScrewTemp1_Asli, ScrewTemp1_Max, ScrewTemp2_Min, ScrewTemp2_Asli, ScrewTemp2_Max, ScrewTemp3_Min, ScrewTemp3_Asli, ScrewTemp3_Max,
    ScrewSpeed1_Min, ScrewSpeed1_Asli, ScrewSpeed1_Max, ScrewSpeed2_Min, ScrewSpeed2_Asli, ScrewSpeed2_Max, ScrewSpeed3_Min, ScrewSpeed3_Asli, ScrewSpeed3_Max,
    Pressure1_Min, Pressure1_Asli, Pressure1_Max, Pressure2_Min, Pressure2_Asli, Pressure2_Max, Pressure3_Min, Pressure3_Asli, Pressure3_Max,
    HoseSpeed_Min, HoseSpeed_Asli, HoseSpeed_Max, TakeUpConveyorSpeed_Min, TakeUpConveyorSpeed_Asli, TakeUpConveyorSpeed_Max, ChillerWaterTemp_Min, ChillerWaterTemp_Asli, ChillerWaterTemp_Max,
    Feed1_Min, Feed1_Asli, Feed1_Max, Feed2_Min, Feed2_Asli, Feed2_Max, Feed3_Min, Feed3_Asli, Feed3_Max,
    FeedRollRatio1_Min, FeedRollRatio1_Asli, FeedRollRatio1_Max, FeedRollRatio2_Min, FeedRollRatio2_Asli, FeedRollRatio2_Max, FeedRollRatio3_Min, FeedRollRatio3_Asli, FeedRollRatio3_Max,
    CaterpillarGap_Min, CaterpillarGap_Asli, CaterpillarGap_Max, SpiralSpeed_Min, SpiralSpeed_Asli, SpiralSpeed_Max,
    TebalInner_Min, TebalInner_Asli, TebalInner_Max, TebalOuter_Min, TebalOuter_Asli, TebalOuter_Max, TebalTotal_Min, TebalTotal_Asli, TebalTotal_Max, TebalInnerMiddle_Min, TebalInnerMiddle_Asli, TebalInnerMiddle_Max,
    AmMeter_Min, AmMeter_Asli, AmMeter_Max, AmMeter2_Min, AmMeter2_Asli, AmMeter2_Max, AmMeter3_Min, AmMeter3_Asli, AmMeter3_Max,
    PresetValue_Min, PresetValue_Asli, PresetValue_Max, ControlValue_Min, ControlValue_Asli, ControlValue_Max,
    SpiralPitchSetting_Min, SpiralPitchSetting_Asli, SpiralPitchSetting_Max, SpiralPitchDisplay_Min, SpiralPitchDisplay_Asli, SpiralPitchDisplay_Max,
    CoolConveyorSpeed_Min, CoolConveyorSpeed_Asli, CoolConveyorSpeed_Max, CoolConveyorSpeed2_Min, CoolConveyorSpeed2_Asli, CoolConveyorSpeed2_Max, ConveyorRatio_Min, ConveyorRatio_Asli, ConveyorRatio_Max,
    ToleranceInner_Min, ToleranceInner_Asli, ToleranceInner_Max, ToleranceOuter_Min, ToleranceOuter_Asli, ToleranceOuter_Max, SelisihTebal_Min, SelisihTebal_Asli, SelisihTebal_Max,
    PitchYarn_Min, PitchYarn_Asli, PitchYarn_Max, DancerPosition_Min, DancerPosition_Asli, DancerPosition_Max, OdSensor_Min, OdSensor_Asli, OdSensor_Max, CuttingSpeed_Min, CuttingSpeed_Asli, CuttingSpeed_Max
)
SELECT 
    MAX(ExcelId), MAX(No), MAX(Machine), DocumentNumber, MAX(RevisionNumber), MAX(Customer), MAX(RevisionDate), MAX(Formulasi), MAX(HoseType), MAX(Dimensi), MAX(Material),
    MAX(InnerTube), MAX(OuterCover), MAX(MiddleTube), MAX(UseLimitsInner), MAX(UseLimitsOuter), MAX(UseLimitsMiddle), MAX(Nipple), MAX(TubeDie), MAX(CoverDie), MAX(MiddleDie), MAX(SpacerDie), MAX(ADistance),
    MAX(Yarn), MAX(TensionYarnInner), MAX(TensionYarnOuter), MAX(MeshDim1), MAX(MeshScreen1), MAX(MeshDim2), MAX(MeshScreen2), MAX(MeshDim3), MAX(MeshScreen3),
    MAX(HeadTemp1), MAX(HeadTemp2), MAX(Cylinder1_1), MAX(Cylinder1_2), MAX(Cylinder2_1), MAX(Cylinder2_2), MAX(Cylinder3_1), MAX(Cylinder3_2), MAX(Cylinder3_3), MAX(HeadTemp3), MAX(Cylinder1_3), MAX(Cylinder2_3), MAX(ScrewTemp3),
    MAX(Feed1), MAX(Feed2), MAX(ScrewTemp1), MAX(ScrewTemp2), MAX(ScrewSpeed1), MAX(ScrewSpeed2), MAX(Pressure1), MAX(Pressure2), MAX(Feed3), MAX(ScrewSpeed3), MAX(Pressure3),
    MAX(AmMeter), MAX(OdSensor), MAX(MarkingSort), MAX(TextMarkingMaterial), MAX(MarkingColour), MAX(ChillerWaterTemp), MAX(CuttingSpeed), MAX(TakeUpConveyorSpeed),
    MAX(ToleranceInner), MAX(ToleranceOuter), MAX(TebalInner), MAX(TebalInnerMiddle), MAX(TebalOuter), MAX(TebalTotal), MAX(SelisihTebal), MAX(ToleranceSpiralPitch), MAX(MachineCode),
    MAX(PitchYarn), MAX(FeedRollRatio1), MAX(FeedRollRatio2), MAX(FeedRollRatio3), MAX(CurrentValue), MAX(AmMeter2), MAX(AmMeter3), MAX(PresetValue), MAX(ControlValue), MAX(SpiralPitchSetting), MAX(SpiralPitchDisplay),
    MAX(SpiralSpeed), MAX(HoseSpeed), MAX(UnsmoothSurface), MAX(DancerPosition), MAX(CaterpillarGap), MAX(CoolConveyorSpeed), MAX(CoolConveyorSpeed2), MAX(ConveyorRatio),
    MAX(InnerTarget), MAX(InnerTol), MAX(InnerLCL), MAX(InnerMin), MAX(InnerUCL), MAX(InnerMax), MAX(InnerMidTarget), MAX(InnerMidTol), MAX(InnerMidLCL), MAX(InnerMidMin), MAX(InnerMidUCL), MAX(InnerMidMax),
    MAX(ThickTarget), MAX(ThickTol), MAX(ThickLCL), MAX(ThickMin), MAX(ThickUCL), MAX(ThickMax), MAX(TotalTarget), MAX(TotalTol), MAX(TotalLCL), MAX(TotalMin), MAX(TotalUCL), MAX(TotalMax),
    MAX(Nipple_Min), MAX(Nipple_Asli), MAX(Nipple_Max), MAX(TubeDie_Min), MAX(TubeDie_Asli), MAX(TubeDie_Max), MAX(CoverDie_Min), MAX(CoverDie_Asli), MAX(CoverDie_Max), MAX(MiddleDie_Min), MAX(MiddleDie_Asli), MAX(MiddleDie_Max), MAX(SpacerDie_Min), MAX(SpacerDie_Asli), MAX(SpacerDie_Max), MAX(ADistance_Min), MAX(ADistance_Asli), MAX(ADistance_Max),
    MAX(MeshDim1_Min), MAX(MeshDim1_Asli), MAX(MeshDim1_Max), MAX(MeshDim2_Min), MAX(MeshDim2_Asli), MAX(MeshDim2_Max), MAX(MeshDim3_Min), MAX(MeshDim3_Asli), MAX(MeshDim3_Max),
    MAX(HeadTemp1_Min), MAX(HeadTemp1_Asli), MAX(HeadTemp1_Max), MAX(HeadTemp2_Min), MAX(HeadTemp2_Asli), MAX(HeadTemp2_Max), MAX(HeadTemp3_Min), MAX(HeadTemp3_Asli), MAX(HeadTemp3_Max),
    MAX(Cylinder1_1_Min), MAX(Cylinder1_1_Asli), MAX(Cylinder1_1_Max), MAX(Cylinder1_2_Min), MAX(Cylinder1_2_Asli), MAX(Cylinder1_2_Max), MAX(Cylinder1_3_Min), MAX(Cylinder1_3_Asli), MAX(Cylinder1_3_Max),
    MAX(Cylinder2_1_Min), MAX(Cylinder2_1_Asli), MAX(Cylinder2_1_Max), MAX(Cylinder2_2_Min), MAX(Cylinder2_2_Asli), MAX(Cylinder2_2_Max), MAX(Cylinder2_3_Min), MAX(Cylinder2_3_Asli), MAX(Cylinder2_3_Max),
    MAX(Cylinder3_1_Min), MAX(Cylinder3_1_Asli), MAX(Cylinder3_1_Max), MAX(Cylinder3_2_Min), MAX(Cylinder3_2_Asli), MAX(Cylinder3_2_Max), MAX(Cylinder3_3_Min), MAX(Cylinder3_3_Asli), MAX(Cylinder3_3_Max),
    MAX(ScrewTemp1_Min), MAX(ScrewTemp1_Asli), MAX(ScrewTemp1_Max), MAX(ScrewTemp2_Min), MAX(ScrewTemp2_Asli), MAX(ScrewTemp2_Max), MAX(ScrewTemp3_Min), MAX(ScrewTemp3_Asli), MAX(ScrewTemp3_Max),
    MAX(ScrewSpeed1_Min), MAX(ScrewSpeed1_Asli), MAX(ScrewSpeed1_Max), MAX(ScrewSpeed2_Min), MAX(ScrewSpeed2_Asli), MAX(ScrewSpeed2_Max), MAX(ScrewSpeed3_Min), MAX(ScrewSpeed3_Asli), MAX(ScrewSpeed3_Max),
    MAX(Pressure1_Min), MAX(Pressure1_Asli), MAX(Pressure1_Max), MAX(Pressure2_Min), MAX(Pressure2_Asli), MAX(Pressure2_Max), MAX(Pressure3_Min), MAX(Pressure3_Asli), MAX(Pressure3_Max),
    MAX(HoseSpeed_Min), MAX(HoseSpeed_Asli), MAX(HoseSpeed_Max), MAX(TakeUpConveyorSpeed_Min), MAX(TakeUpConveyorSpeed_Asli), MAX(TakeUpConveyorSpeed_Max), MAX(ChillerWaterTemp_Min), MAX(ChillerWaterTemp_Asli), MAX(ChillerWaterTemp_Max),
    MAX(Feed1_Min), MAX(Feed1_Asli), MAX(Feed1_Max), MAX(Feed2_Min), MAX(Feed2_Asli), MAX(Feed2_Max), MAX(Feed3_Min), MAX(Feed3_Asli), MAX(Feed3_Max),
    MAX(FeedRollRatio1_Min), MAX(FeedRollRatio1_Asli), MAX(FeedRollRatio1_Max), MAX(FeedRollRatio2_Min), MAX(FeedRollRatio2_Asli), MAX(FeedRollRatio2_Max), MAX(FeedRollRatio3_Min), MAX(FeedRollRatio3_Asli), MAX(FeedRollRatio3_Max),
    MAX(CaterpillarGap_Min), MAX(CaterpillarGap_Asli), MAX(CaterpillarGap_Max), MAX(SpiralSpeed_Min), MAX(SpiralSpeed_Asli), MAX(SpiralSpeed_Max),
    MAX(TebalInner_Min), MAX(TebalInner_Asli), MAX(TebalInner_Max), MAX(TebalOuter_Min), MAX(TebalOuter_Asli), MAX(TebalOuter_Max), MAX(TebalTotal_Min), MAX(TebalTotal_Asli), MAX(TebalTotal_Max), MAX(TebalInnerMiddle_Min), MAX(TebalInnerMiddle_Asli), MAX(TebalInnerMiddle_Max),
    MAX(AmMeter_Min), MAX(AmMeter_Asli), MAX(AmMeter_Max), MAX(AmMeter2_Min), MAX(AmMeter2_Asli), MAX(AmMeter2_Max), MAX(AmMeter3_Min), MAX(AmMeter3_Asli), MAX(AmMeter3_Max),
    MAX(PresetValue_Min), MAX(PresetValue_Asli), MAX(PresetValue_Max), MAX(ControlValue_Min), MAX(ControlValue_Asli), MAX(ControlValue_Max),
    MAX(SpiralPitchSetting_Min), MAX(SpiralPitchSetting_Asli), MAX(SpiralPitchSetting_Max), MAX(SpiralPitchDisplay_Min), MAX(SpiralPitchDisplay_Asli), MAX(SpiralPitchDisplay_Max),
    MAX(CoolConveyorSpeed_Min), MAX(CoolConveyorSpeed_Asli), MAX(CoolConveyorSpeed_Max), MAX(CoolConveyorSpeed2_Min), MAX(CoolConveyorSpeed2_Asli), MAX(CoolConveyorSpeed2_Max), MAX(ConveyorRatio_Min), MAX(ConveyorRatio_Asli), MAX(ConveyorRatio_Max),
    MAX(ToleranceInner_Min), MAX(ToleranceInner_Asli), MAX(ToleranceInner_Max), MAX(ToleranceOuter_Min), MAX(ToleranceOuter_Asli), MAX(ToleranceOuter_Max), MAX(SelisihTebal_Min), MAX(SelisihTebal_Asli), MAX(SelisihTebal_Max),
    MAX(PitchYarn_Min), MAX(PitchYarn_Asli), MAX(PitchYarn_Max), MAX(DancerPosition_Min), MAX(DancerPosition_Asli), MAX(DancerPosition_Max), MAX(OdSensor_Min), MAX(OdSensor_Asli), MAX(OdSensor_Max), MAX(CuttingSpeed_Min), MAX(CuttingSpeed_Asli), MAX(CuttingSpeed_Max)
FROM SpsMasters
WHERE DocumentNumber IS NOT NULL AND DocumentNumber != '' AND DocumentNumber != '-'
GROUP BY DocumentNumber;

-- 2. Insert ke SpsNoDocs untuk yang TIDAK PUNYA Document Number (Masing-masing baris masuk sendiri tanpa di-group)
-- Kita akan menggunakan trik GROUP BY Id agar setiap baris punya record SpsNoDoc sendiri
INSERT INTO SpsNoDocs (
    ExcelId, No, Machine, DocumentNumber, RevisionNumber, Customer, RevisionDate, Formulasi, HoseType, Dimensi, Material,
    InnerTube, OuterCover, MiddleTube, UseLimitsInner, UseLimitsOuter, UseLimitsMiddle, Nipple, TubeDie, CoverDie, MiddleDie, SpacerDie, ADistance,
    Yarn, TensionYarnInner, TensionYarnOuter, MeshDim1, MeshScreen1, MeshDim2, MeshScreen2, MeshDim3, MeshScreen3,
    HeadTemp1, HeadTemp2, Cylinder1_1, Cylinder1_2, Cylinder2_1, Cylinder2_2, Cylinder3_1, Cylinder3_2, Cylinder3_3, HeadTemp3, Cylinder1_3, Cylinder2_3, ScrewTemp3,
    Feed1, Feed2, ScrewTemp1, ScrewTemp2, ScrewSpeed1, ScrewSpeed2, Pressure1, Pressure2, Feed3, ScrewSpeed3, Pressure3,
    AmMeter, OdSensor, MarkingSort, TextMarkingMaterial, MarkingColour, ChillerWaterTemp, CuttingSpeed, TakeUpConveyorSpeed,
    ToleranceInner, ToleranceOuter, TebalInner, TebalInnerMiddle, TebalOuter, TebalTotal, SelisihTebal, ToleranceSpiralPitch, MachineCode,
    PitchYarn, FeedRollRatio1, FeedRollRatio2, FeedRollRatio3, CurrentValue, AmMeter2, AmMeter3, PresetValue, ControlValue, SpiralPitchSetting, SpiralPitchDisplay,
    SpiralSpeed, HoseSpeed, UnsmoothSurface, DancerPosition, CaterpillarGap, CoolConveyorSpeed, CoolConveyorSpeed2, ConveyorRatio,
    InnerTarget, InnerTol, InnerLCL, InnerMin, InnerUCL, InnerMax, InnerMidTarget, InnerMidTol, InnerMidLCL, InnerMidMin, InnerMidUCL, InnerMidMax,
    ThickTarget, ThickTol, ThickLCL, ThickMin, ThickUCL, ThickMax, TotalTarget, TotalTol, TotalLCL, TotalMin, TotalUCL, TotalMax,
    Nipple_Min, Nipple_Asli, Nipple_Max, TubeDie_Min, TubeDie_Asli, TubeDie_Max, CoverDie_Min, CoverDie_Asli, CoverDie_Max, MiddleDie_Min, MiddleDie_Asli, MiddleDie_Max, SpacerDie_Min, SpacerDie_Asli, SpacerDie_Max, ADistance_Min, ADistance_Asli, ADistance_Max,
    MeshDim1_Min, MeshDim1_Asli, MeshDim1_Max, MeshDim2_Min, MeshDim2_Asli, MeshDim2_Max, MeshDim3_Min, MeshDim3_Asli, MeshDim3_Max,
    HeadTemp1_Min, HeadTemp1_Asli, HeadTemp1_Max, HeadTemp2_Min, HeadTemp2_Asli, HeadTemp2_Max, HeadTemp3_Min, HeadTemp3_Asli, HeadTemp3_Max,
    Cylinder1_1_Min, Cylinder1_1_Asli, Cylinder1_1_Max, Cylinder1_2_Min, Cylinder1_2_Asli, Cylinder1_2_Max, Cylinder1_3_Min, Cylinder1_3_Asli, Cylinder1_3_Max,
    Cylinder2_1_Min, Cylinder2_1_Asli, Cylinder2_1_Max, Cylinder2_2_Min, Cylinder2_2_Asli, Cylinder2_2_Max, Cylinder2_3_Min, Cylinder2_3_Asli, Cylinder2_3_Max,
    Cylinder3_1_Min, Cylinder3_1_Asli, Cylinder3_1_Max, Cylinder3_2_Min, Cylinder3_2_Asli, Cylinder3_2_Max, Cylinder3_3_Min, Cylinder3_3_Asli, Cylinder3_3_Max,
    ScrewTemp1_Min, ScrewTemp1_Asli, ScrewTemp1_Max, ScrewTemp2_Min, ScrewTemp2_Asli, ScrewTemp2_Max, ScrewTemp3_Min, ScrewTemp3_Asli, ScrewTemp3_Max,
    ScrewSpeed1_Min, ScrewSpeed1_Asli, ScrewSpeed1_Max, ScrewSpeed2_Min, ScrewSpeed2_Asli, ScrewSpeed2_Max, ScrewSpeed3_Min, ScrewSpeed3_Asli, ScrewSpeed3_Max,
    Pressure1_Min, Pressure1_Asli, Pressure1_Max, Pressure2_Min, Pressure2_Asli, Pressure2_Max, Pressure3_Min, Pressure3_Asli, Pressure3_Max,
    HoseSpeed_Min, HoseSpeed_Asli, HoseSpeed_Max, TakeUpConveyorSpeed_Min, TakeUpConveyorSpeed_Asli, TakeUpConveyorSpeed_Max, ChillerWaterTemp_Min, ChillerWaterTemp_Asli, ChillerWaterTemp_Max,
    Feed1_Min, Feed1_Asli, Feed1_Max, Feed2_Min, Feed2_Asli, Feed2_Max, Feed3_Min, Feed3_Asli, Feed3_Max,
    FeedRollRatio1_Min, FeedRollRatio1_Asli, FeedRollRatio1_Max, FeedRollRatio2_Min, FeedRollRatio2_Asli, FeedRollRatio2_Max, FeedRollRatio3_Min, FeedRollRatio3_Asli, FeedRollRatio3_Max,
    CaterpillarGap_Min, CaterpillarGap_Asli, CaterpillarGap_Max, SpiralSpeed_Min, SpiralSpeed_Asli, SpiralSpeed_Max,
    TebalInner_Min, TebalInner_Asli, TebalInner_Max, TebalOuter_Min, TebalOuter_Asli, TebalOuter_Max, TebalTotal_Min, TebalTotal_Asli, TebalTotal_Max, TebalInnerMiddle_Min, TebalInnerMiddle_Asli, TebalInnerMiddle_Max,
    AmMeter_Min, AmMeter_Asli, AmMeter_Max, AmMeter2_Min, AmMeter2_Asli, AmMeter2_Max, AmMeter3_Min, AmMeter3_Asli, AmMeter3_Max,
    PresetValue_Min, PresetValue_Asli, PresetValue_Max, ControlValue_Min, ControlValue_Asli, ControlValue_Max,
    SpiralPitchSetting_Min, SpiralPitchSetting_Asli, SpiralPitchSetting_Max, SpiralPitchDisplay_Min, SpiralPitchDisplay_Asli, SpiralPitchDisplay_Max,
    CoolConveyorSpeed_Min, CoolConveyorSpeed_Asli, CoolConveyorSpeed_Max, CoolConveyorSpeed2_Min, CoolConveyorSpeed2_Asli, CoolConveyorSpeed2_Max, ConveyorRatio_Min, ConveyorRatio_Asli, ConveyorRatio_Max,
    ToleranceInner_Min, ToleranceInner_Asli, ToleranceInner_Max, ToleranceOuter_Min, ToleranceOuter_Asli, ToleranceOuter_Max, SelisihTebal_Min, SelisihTebal_Asli, SelisihTebal_Max,
    PitchYarn_Min, PitchYarn_Asli, PitchYarn_Max, DancerPosition_Min, DancerPosition_Asli, DancerPosition_Max, OdSensor_Min, OdSensor_Asli, OdSensor_Max, CuttingSpeed_Min, CuttingSpeed_Asli, CuttingSpeed_Max
)
SELECT 
    ExcelId, No, Machine, DocumentNumber, RevisionNumber, Customer, RevisionDate, Formulasi, HoseType, Dimensi, Material,
    InnerTube, OuterCover, MiddleTube, UseLimitsInner, UseLimitsOuter, UseLimitsMiddle, Nipple, TubeDie, CoverDie, MiddleDie, SpacerDie, ADistance,
    Yarn, TensionYarnInner, TensionYarnOuter, MeshDim1, MeshScreen1, MeshDim2, MeshScreen2, MeshDim3, MeshScreen3,
    HeadTemp1, HeadTemp2, Cylinder1_1, Cylinder1_2, Cylinder2_1, Cylinder2_2, Cylinder3_1, Cylinder3_2, Cylinder3_3, HeadTemp3, Cylinder1_3, Cylinder2_3, ScrewTemp3,
    Feed1, Feed2, ScrewTemp1, ScrewTemp2, ScrewSpeed1, ScrewSpeed2, Pressure1, Pressure2, Feed3, ScrewSpeed3, Pressure3,
    AmMeter, OdSensor, MarkingSort, TextMarkingMaterial, MarkingColour, ChillerWaterTemp, CuttingSpeed, TakeUpConveyorSpeed,
    ToleranceInner, ToleranceOuter, TebalInner, TebalInnerMiddle, TebalOuter, TebalTotal, SelisihTebal, ToleranceSpiralPitch, MachineCode,
    PitchYarn, FeedRollRatio1, FeedRollRatio2, FeedRollRatio3, CurrentValue, AmMeter2, AmMeter3, PresetValue, ControlValue, SpiralPitchSetting, SpiralPitchDisplay,
    SpiralSpeed, HoseSpeed, UnsmoothSurface, DancerPosition, CaterpillarGap, CoolConveyorSpeed, CoolConveyorSpeed2, ConveyorRatio,
    InnerTarget, InnerTol, InnerLCL, InnerMin, InnerUCL, InnerMax, InnerMidTarget, InnerMidTol, InnerMidLCL, InnerMidMin, InnerMidUCL, InnerMidMax,
    ThickTarget, ThickTol, ThickLCL, ThickMin, ThickUCL, ThickMax, TotalTarget, TotalTol, TotalLCL, TotalMin, TotalUCL, TotalMax,
    Nipple_Min, Nipple_Asli, Nipple_Max, TubeDie_Min, TubeDie_Asli, TubeDie_Max, CoverDie_Min, CoverDie_Asli, CoverDie_Max, MiddleDie_Min, MiddleDie_Asli, MiddleDie_Max, SpacerDie_Min, SpacerDie_Asli, SpacerDie_Max, ADistance_Min, ADistance_Asli, ADistance_Max,
    MeshDim1_Min, MeshDim1_Asli, MeshDim1_Max, MeshDim2_Min, MeshDim2_Asli, MeshDim2_Max, MeshDim3_Min, MeshDim3_Asli, MeshDim3_Max,
    HeadTemp1_Min, HeadTemp1_Asli, HeadTemp1_Max, HeadTemp2_Min, HeadTemp2_Asli, HeadTemp2_Max, HeadTemp3_Min, HeadTemp3_Asli, HeadTemp3_Max,
    Cylinder1_1_Min, Cylinder1_1_Asli, Cylinder1_1_Max, Cylinder1_2_Min, Cylinder1_2_Asli, Cylinder1_2_Max, Cylinder1_3_Min, Cylinder1_3_Asli, Cylinder1_3_Max,
    Cylinder2_1_Min, Cylinder2_1_Asli, Cylinder2_1_Max, Cylinder2_2_Min, Cylinder2_2_Asli, Cylinder2_2_Max, Cylinder2_3_Min, Cylinder2_3_Asli, Cylinder2_3_Max,
    Cylinder3_1_Min, Cylinder3_1_Asli, Cylinder3_1_Max, Cylinder3_2_Min, Cylinder3_2_Asli, Cylinder3_2_Max, Cylinder3_3_Min, Cylinder3_3_Asli, Cylinder3_3_Max,
    ScrewTemp1_Min, ScrewTemp1_Asli, ScrewTemp1_Max, ScrewTemp2_Min, ScrewTemp2_Asli, ScrewTemp2_Max, ScrewTemp3_Min, ScrewTemp3_Asli, ScrewTemp3_Max,
    ScrewSpeed1_Min, ScrewSpeed1_Asli, ScrewSpeed1_Max, ScrewSpeed2_Min, ScrewSpeed2_Asli, ScrewSpeed2_Max, ScrewSpeed3_Min, ScrewSpeed3_Asli, ScrewSpeed3_Max,
    Pressure1_Min, Pressure1_Asli, Pressure1_Max, Pressure2_Min, Pressure2_Asli, Pressure2_Max, Pressure3_Min, Pressure3_Asli, Pressure3_Max,
    HoseSpeed_Min, HoseSpeed_Asli, HoseSpeed_Max, TakeUpConveyorSpeed_Min, TakeUpConveyorSpeed_Asli, TakeUpConveyorSpeed_Max, ChillerWaterTemp_Min, ChillerWaterTemp_Asli, ChillerWaterTemp_Max,
    Feed1_Min, Feed1_Asli, Feed1_Max, Feed2_Min, Feed2_Asli, Feed2_Max, Feed3_Min, Feed3_Asli, Feed3_Max,
    FeedRollRatio1_Min, FeedRollRatio1_Asli, FeedRollRatio1_Max, FeedRollRatio2_Min, FeedRollRatio2_Asli, FeedRollRatio2_Max, FeedRollRatio3_Min, FeedRollRatio3_Asli, FeedRollRatio3_Max,
    CaterpillarGap_Min, CaterpillarGap_Asli, CaterpillarGap_Max, SpiralSpeed_Min, SpiralSpeed_Asli, SpiralSpeed_Max,
    TebalInner_Min, TebalInner_Asli, TebalInner_Max, TebalOuter_Min, TebalOuter_Asli, TebalOuter_Max, TebalTotal_Min, TebalTotal_Asli, TebalTotal_Max, TebalInnerMiddle_Min, TebalInnerMiddle_Asli, TebalInnerMiddle_Max,
    AmMeter_Min, AmMeter_Asli, AmMeter_Max, AmMeter2_Min, AmMeter2_Asli, AmMeter2_Max, AmMeter3_Min, AmMeter3_Asli, AmMeter3_Max,
    PresetValue_Min, PresetValue_Asli, PresetValue_Max, ControlValue_Min, ControlValue_Asli, ControlValue_Max,
    SpiralPitchSetting_Min, SpiralPitchSetting_Asli, SpiralPitchSetting_Max, SpiralPitchDisplay_Min, SpiralPitchDisplay_Asli, SpiralPitchDisplay_Max,
    CoolConveyorSpeed_Min, CoolConveyorSpeed_Asli, CoolConveyorSpeed_Max, CoolConveyorSpeed2_Min, CoolConveyorSpeed2_Asli, CoolConveyorSpeed2_Max, ConveyorRatio_Min, ConveyorRatio_Asli, ConveyorRatio_Max,
    ToleranceInner_Min, ToleranceInner_Asli, ToleranceInner_Max, ToleranceOuter_Min, ToleranceOuter_Asli, ToleranceOuter_Max, SelisihTebal_Min, SelisihTebal_Asli, SelisihTebal_Max,
    PitchYarn_Min, PitchYarn_Asli, PitchYarn_Max, DancerPosition_Min, DancerPosition_Asli, DancerPosition_Max, OdSensor_Min, OdSensor_Asli, OdSensor_Max, CuttingSpeed_Min, CuttingSpeed_Asli, CuttingSpeed_Max
FROM SpsMasters
WHERE DocumentNumber IS NULL OR DocumentNumber = '' OR DocumentNumber = '-';

-- 3. Insert ke SpsItemLists untuk dokumen normal
INSERT INTO SpsItemLists (ItemList, SpsNoDocId)
SELECT m.ItemList, n.Id
FROM SpsMasters m
JOIN SpsNoDocs n ON m.DocumentNumber = n.DocumentNumber
WHERE (m.DocumentNumber IS NOT NULL AND m.DocumentNumber != '' AND m.DocumentNumber != '-')
  AND m.ItemList IS NOT NULL AND m.ItemList != '';

-- 4. Insert ke SpsItemLists untuk dokumen kosong (-)
-- Kita harus join berdasarkan semua atribut identitas supaya nggak salah pasang item
INSERT INTO SpsItemLists (ItemList, SpsNoDocId)
SELECT m.ItemList, n.Id
FROM SpsMasters m
JOIN SpsNoDocs n ON (m.DocumentNumber IS NULL OR m.DocumentNumber = '' OR m.DocumentNumber = '-') 
                 AND n.DocumentNumber = m.DocumentNumber
                 AND n.Formulasi = m.Formulasi
                 AND n.Machine = m.Machine
                 AND ISNULL(n.HoseType, '') = ISNULL(m.HoseType, '')
                 AND ISNULL(n.Dimensi, '') = ISNULL(m.Dimensi, '')
                 AND ISNULL(n.Material, '') = ISNULL(m.Material, '')
WHERE m.ItemList IS NOT NULL AND m.ItemList != '';

COMMIT;
