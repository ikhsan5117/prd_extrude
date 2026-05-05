USE ELWP_PRD;
GO

-- Insert Area
IF NOT EXISTS (SELECT * FROM produksi.tb_elwp_produksi_areas WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT produksi.tb_elwp_produksi_areas ON;
    INSERT INTO produksi.tb_elwp_produksi_areas (Id, Name, PlantId, Description, IsActive, CreatedAt, NoUrut)
    VALUES (1, 'Extrude Area', 1, 'Main Extrude Area', 1, GETDATE(), 1);
    SET IDENTITY_INSERT produksi.tb_elwp_produksi_areas OFF;
END
GO

-- Insert User
IF NOT EXISTS (SELECT * FROM produksi.tb_elwp_produksi_users WHERE Username = 'operator_ext')
BEGIN
    INSERT INTO produksi.tb_elwp_produksi_users (Username, PasswordHash, FullName, Email, NPK, PlantId, AreaId, Role, IsActive, CreatedAt, LastLoginAt)
    VALUES ('operator_ext', 'extrude123!', 'Operator Extrude 1', 'operator@example.com', '12345', 1, 1, 'Operator', 1, GETDATE(), NULL);
END
GO

-- Insert Machine
IF NOT EXISTS (SELECT * FROM produksi.tb_elwp_produksi_mesins WHERE KodeMesin = 'DL01')
BEGIN
    INSERT INTO produksi.tb_elwp_produksi_mesins (KodeMesin, NamaMesin, PlantId, AreaId, Keterangan, IsActive, CreatedAt, UpdatedAt, Kapasitas, RequiredManPower)
    VALUES ('DL01', 'Mesin Double Layer 1', 1, 1, 'Mesin DL01', 1, GETDATE(), GETDATE(), 100.0, 2);
    
    INSERT INTO produksi.tb_elwp_produksi_mesins (KodeMesin, NamaMesin, PlantId, AreaId, Keterangan, IsActive, CreatedAt, UpdatedAt, Kapasitas, RequiredManPower)
    VALUES ('DL02', 'Mesin Double Layer 2', 1, 1, 'Mesin DL02', 1, GETDATE(), GETDATE(), 100.0, 2);
END
GO
