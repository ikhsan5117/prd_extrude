-- Migration: Add Feed3 tolerance columns
-- Date: 2026-05-11

ALTER TABLE SpsMasters ADD Feed3_Min decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed3_Asli decimal(18,4) NULL;
ALTER TABLE SpsMasters ADD Feed3_Max decimal(18,4) NULL;
