-- Migration: Rollback - Add back deleted tolerance columns (except Feed3 and ToleranceSpiralPitch)
-- Date: 2026-05-11
-- Add back: 24 columns (MeshDim, ChillerWaterTemp, Feed1/2, ConveyorRatio tolerance columns)

-- Add back MeshDim tolerance columns
ALTER TABLE SpsMasters ADD MeshDim1_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim1_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim1_Max decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim2_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim2_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim2_Max decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim3_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim3_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD MeshDim3_Max decimal(18,4) NULL;

-- Add back ChillerWaterTemp tolerance columns
ALTER TABLE SpsMasters ADD ChillerWaterTemp_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD ChillerWaterTemp_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD ChillerWaterTemp_Max decimal(18,4) NULL;

-- Add back Feed1/2 tolerance columns (NOT Feed3 - it's truly empty)
ALTER TABLE SpsMasters ADD Feed1_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed1_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed1_Max decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed2_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed2_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed2_Max decimal(18,4) NULL;

-- Add back ConveyorRatio tolerance columns
ALTER TABLE SpsMasters ADD ConveyorRatio_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD ConveyorRatio_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD ConveyorRatio_Max decimal(18,4) NULL;
