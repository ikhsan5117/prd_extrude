-- Migration: Add back Feed3 and ToleranceSpiralPitch original string columns
-- Date: 2026-05-11

ALTER TABLE SpsMasters ADD Feed3 nvarchar(max) NULL;
ALTER TABLE SpsMasters ADD ToleranceSpiralPitch nvarchar(max) NULL;
