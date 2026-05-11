-- Migration Batch 4: Add OdSensor and CuttingSpeed tolerance columns
-- Date: 2026-05-11
-- Fields: 2 fields × 3 columns = 6 new columns

-- OD Sensor tolerance columns
ALTER TABLE SpsMasters ADD OdSensor_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD OdSensor_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD OdSensor_Max decimal(18,4) NULL;

-- Cutting Speed tolerance columns
ALTER TABLE SpsMasters ADD CuttingSpeed_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD CuttingSpeed_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD CuttingSpeed_Max decimal(18,4) NULL;
