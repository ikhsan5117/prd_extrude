-- Add 39 new tolerance columns (13 fields × 3: Min, Asli, Max)
-- Batch 2: AmMeter, PresetValue, ControlValue, SpiralPitch, CoolConveyor, Tolerance, Selisih

PRINT 'Adding AmMeter tolerance columns...';
ALTER TABLE SpsMasters ADD AmMeter_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD AmMeter_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD AmMeter_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD AmMeter2_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD AmMeter2_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD AmMeter2_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD AmMeter3_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD AmMeter3_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD AmMeter3_Max DECIMAL(18,4) NULL;

PRINT 'Adding Control Value tolerance columns...';
ALTER TABLE SpsMasters ADD PresetValue_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD PresetValue_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD PresetValue_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD ControlValue_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ControlValue_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ControlValue_Max DECIMAL(18,4) NULL;

PRINT 'Adding Spiral Pitch tolerance columns...';
ALTER TABLE SpsMasters ADD SpiralPitchSetting_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD SpiralPitchSetting_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD SpiralPitchSetting_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD SpiralPitchDisplay_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD SpiralPitchDisplay_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD SpiralPitchDisplay_Max DECIMAL(18,4) NULL;

PRINT 'Adding Cool Conveyor Speed tolerance columns...';
ALTER TABLE SpsMasters ADD CoolConveyorSpeed_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD CoolConveyorSpeed_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD CoolConveyorSpeed_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD CoolConveyorSpeed2_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD CoolConveyorSpeed2_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD CoolConveyorSpeed2_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD ConveyorRatio_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ConveyorRatio_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ConveyorRatio_Max DECIMAL(18,4) NULL;

PRINT 'Adding Tolerance tolerance columns...';
ALTER TABLE SpsMasters ADD ToleranceInner_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ToleranceInner_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ToleranceInner_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD ToleranceOuter_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ToleranceOuter_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD ToleranceOuter_Max DECIMAL(18,4) NULL;

ALTER TABLE SpsMasters ADD SelisihTebal_Min DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD SelisihTebal_Asli DECIMAL(18,4) NULL;
ALTER TABLE SpsMasters ADD SelisihTebal_Max DECIMAL(18,4) NULL;

PRINT 'Migration complete! Added 39 new tolerance columns.';
PRINT 'Total tolerance columns now: 174 (58 fields × 3)';
