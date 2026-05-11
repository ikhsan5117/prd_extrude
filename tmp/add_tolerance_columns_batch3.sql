-- Migration Batch 3: Add PitchYarn and DancerPosition tolerance columns
-- Date: 2026-05-11
-- Fields: 2 fields × 3 columns = 6 new columns

-- Pitch Yarn tolerance columns
ALTER TABLE SpsMasters ADD PitchYarn_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD PitchYarn_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD PitchYarn_Max decimal(18,4) NULL;

-- Dancer Position tolerance columns
ALTER TABLE SpsMasters ADD DancerPosition_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD DancerPosition_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD DancerPosition_Max decimal(18,4) NULL;
