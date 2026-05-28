using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class SplitSpsMasterToDocumentAndItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conditionally drop FK if it still exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_SpsItemLists_SpsNoDocs_SpsNoDocId'
                )
                BEGIN
                    ALTER TABLE [SpsItemLists] DROP CONSTRAINT [FK_SpsItemLists_SpsNoDocs_SpsNoDocId];
                END
            ");

            // Conditionally drop index IX_SpsItemLists_SpsNoDocId
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.indexes 
                    WHERE name = 'IX_SpsItemLists_SpsNoDocId' AND object_id = OBJECT_ID('SpsItemLists')
                )
                BEGIN
                    DROP INDEX [IX_SpsItemLists_SpsNoDocId] ON [SpsItemLists];
                END
            ");

            // Conditionally drop PK_SpsNoDocs (if still on Id column)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.key_constraints kc
                    INNER JOIN sys.index_columns ic ON kc.unique_index_id = ic.index_id AND kc.parent_object_id = ic.object_id
                    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE kc.name = 'PK_SpsNoDocs' AND c.name = 'Id'
                )
                BEGIN
                    ALTER TABLE [SpsNoDocs] DROP CONSTRAINT [PK_SpsNoDocs];
                END
            ");

            // Conditionally drop Id column from SpsNoDocs
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID('SpsNoDocs') AND name = 'Id'
                )
                BEGIN
                    ALTER TABLE [SpsNoDocs] DROP COLUMN [Id];
                END
            ");

            // Conditionally drop ExcelId column from SpsNoDocs
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID('SpsNoDocs') AND name = 'ExcelId'
                )
                BEGIN
                    ALTER TABLE [SpsNoDocs] DROP COLUMN [ExcelId];
                END
            ");

            // Conditionally drop SpsNoDocId column from SpsItemLists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID('SpsItemLists') AND name = 'SpsNoDocId'
                )
                BEGIN
                    ALTER TABLE [SpsItemLists] DROP COLUMN [SpsNoDocId];
                END
            ");

            // Alter DocumentNumber in SpsNoDocs to nvarchar(450) NOT NULL
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_SpsItemLists_DocumentNumber' OR name = 'FK_SpsItemLists_SpsNoDocs_DocumentNumber'
                )
                BEGIN
                    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SpsItemLists_DocumentNumber')
                        ALTER TABLE [SpsItemLists] DROP CONSTRAINT [FK_SpsItemLists_DocumentNumber];
                    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SpsItemLists_SpsNoDocs_DocumentNumber')
                        ALTER TABLE [SpsItemLists] DROP CONSTRAINT [FK_SpsItemLists_SpsNoDocs_DocumentNumber];
                END

                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID('SpsNoDocs') AND name = 'DocumentNumber' AND (is_nullable = 1 OR max_length = -1)
                )
                BEGIN
                    ALTER TABLE [SpsNoDocs] ALTER COLUMN [DocumentNumber] nvarchar(450) NOT NULL;
                END
            ");

            // Add DocumentNumber column to SpsItemLists if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID('SpsItemLists') AND name = 'DocumentNumber'
                )
                BEGIN
                    ALTER TABLE [SpsItemLists] ADD [DocumentNumber] nvarchar(450) NOT NULL DEFAULT '';
                END
            ");

            // Add PK on DocumentNumber if not already set
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.key_constraints 
                    WHERE type = 'PK' AND parent_object_id = OBJECT_ID('SpsNoDocs')
                )
                BEGIN
                    ALTER TABLE [SpsNoDocs] ADD CONSTRAINT [PK_SpsNoDocs] PRIMARY KEY ([DocumentNumber]);
                END
            ");

            // Create index on SpsItemLists.DocumentNumber if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes 
                    WHERE name = 'IX_SpsItemLists_DocumentNumber' AND object_id = OBJECT_ID('SpsItemLists')
                )
                BEGIN
                    CREATE INDEX [IX_SpsItemLists_DocumentNumber] ON [SpsItemLists] ([DocumentNumber]);
                END
            ");

            // Add FK if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_SpsItemLists_SpsNoDocs_DocumentNumber'
                )
                BEGIN
                    ALTER TABLE [SpsItemLists] ADD CONSTRAINT [FK_SpsItemLists_SpsNoDocs_DocumentNumber]
                    FOREIGN KEY ([DocumentNumber]) REFERENCES [SpsNoDocs] ([DocumentNumber]) ON DELETE CASCADE;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpsItemLists_SpsNoDocs_DocumentNumber",
                table: "SpsItemLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpsNoDocs",
                table: "SpsNoDocs");

            migrationBuilder.DropIndex(
                name: "IX_SpsItemLists_DocumentNumber",
                table: "SpsItemLists");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "SpsItemLists");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentNumber",
                table: "SpsNoDocs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SpsNoDocs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "ExcelId",
                table: "SpsNoDocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpsNoDocId",
                table: "SpsItemLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpsNoDocs",
                table: "SpsNoDocs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SpsItemLists_SpsNoDocId",
                table: "SpsItemLists",
                column: "SpsNoDocId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpsItemLists_SpsNoDocs_SpsNoDocId",
                table: "SpsItemLists",
                column: "SpsNoDocId",
                principalTable: "SpsNoDocs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
