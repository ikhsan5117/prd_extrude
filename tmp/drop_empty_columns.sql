-- Migration: Drop empty columns from SpsMasters table
-- Date: 2026-05-11
-- Total: 26 columns to drop

-- Drop original string fields that are empty
ALTER TABLE SpsMasters DROP COLUMN Feed3;
ALTER TABLE SpsMasters DROP COLUMN ToleranceSpiralPitch;

-- Drop MeshDim tolerance columns (all empty)
ALTER TABLE SpsMasters DROP COLUMN MeshDim1_Min;
ALTER TABLE SpsMasters DROP COLUMN MeshDim1_Asli;
ALTER TABLE SpsMasters DROP COLUMN MeshDim1_Max;
ALTER TABLE SpsMasters DROP COLUMN MeshDim2_Min;
ALTER TABLE SpsMasters DROP COLUMN MeshDim2_Asli;
ALTER TABLE SpsMasters DROP COLUMN MeshDim2_Max;
ALTER TABLE SpsMasters DROP COLUMN MeshDim3_Min;
ALTER TABLE SpsMasters DROP COLUMN MeshDim3_Asli;
ALTER TABLE SpsMasters DROP COLUMN MeshDim3_Max;

-- Drop ChillerWaterTemp tolerance columns (all empty)
ALTER TABLE SpsMasters DROP COLUMN ChillerWaterTemp_Min;
ALTER TABLE SpsMasters DROP COLUMN ChillerWaterTemp_Asli;
ALTER TABLE SpsMasters DROP COLUMN ChillerWaterTemp_Max;

-- Drop Feed1/2/3 tolerance columns (all empty)
ALTER TABLE SpsMasters DROP COLUMN Feed1_Min;
ALTER TABLE SpsMasters DROP COLUMN Feed1_Asli;
ALTER TABLE SpsMasters DROP COLUMN Feed1_Max;
ALTER TABLE SpsMasters DROP COLUMN Feed2_Min;
ALTER TABLE SpsMasters DROP COLUMN Feed2_Asli;
ALTER TABLE SpsMasters DROP COLUMN Feed2_Max;
ALTER TABLE SpsMasters DROP COLUMN Feed3_Min;
ALTER TABLE SpsMasters DROP COLUMN Feed3_Asli;
ALTER TABLE SpsMasters DROP COLUMN Feed3_Max;

-- Drop ConveyorRatio tolerance columns (all empty)
ALTER TABLE SpsMasters DROP COLUMN ConveyorRatio_Min;
ALTER TABLE SpsMasters DROP COLUMN ConveyorRatio_Asli;
ALTER TABLE SpsMasters DROP COLUMN ConveyorRatio_Max;
