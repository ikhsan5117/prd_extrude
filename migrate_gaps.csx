#r "nuget: System.Data.SqlClient, 4.8.5"
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System;

string connStr = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;";

(decimal? min, decimal? max, decimal? target) ParseRange(string raw)
{
    if (string.IsNullOrWhiteSpace(raw) || raw == "-") return (null, null, null);
    
    raw = raw.Trim().Replace(",", ".");
    
    // Handle "X±Y" or "X ± Y"
    var match = Regex.Match(raw, @"^([0-9.]+)\s*±\s*([0-9.]+)$");
    if (match.Success)
    {
        if (decimal.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var t) &&
            decimal.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var tol))
        {
            return (t - tol, t + tol, t);
        }
    }
    
    // Handle "Max. X" or "Max X"
    match = Regex.Match(raw, @"(?i)max\.?\s*([0-9.]+)(?:\s*°C)?$");
    if (match.Success)
    {
        if (decimal.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var t))
        {
            return (null, t, t);
        }
    }

    // Handle just a number "X"
    if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
    {
        return (null, null, val);
    }
    
    return (null, null, null);
}

using (var conn = new SqlConnection(connStr))
{
    conn.Open();
    // Select the raw string and the _Asli so we only update if _Asli is NULL
    string sql = @"
        SELECT 
            DocumentNumber, 
            CaterpillarGap, CaterpillarGap_Asli,
            ChillerWaterTemp, ChillerWaterTemp_Asli,
            TakeUpConveyorSpeed, TakeUpConveyorSpeed_Asli,
            CuttingSpeed, CuttingSpeed_Asli
        FROM SpsNoDocs";
        
    var updates = new List<string>();
    
    using (var cmd = new SqlCommand(sql, conn))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            string docNumber = reader.GetString(0);
            
            string catGap = reader.IsDBNull(1) ? null : reader.GetString(1);
            bool catNeedsUpdate = catGap != null && reader.IsDBNull(2);
            
            string chiller = reader.IsDBNull(3) ? null : reader.GetString(3);
            bool chillNeedsUpdate = chiller != null && reader.IsDBNull(4);
            
            string takeup = reader.IsDBNull(5) ? null : reader.GetString(5);
            bool takeNeedsUpdate = takeup != null && reader.IsDBNull(6);
            
            string cutting = reader.IsDBNull(7) ? null : reader.GetString(7);
            bool cutNeedsUpdate = cutting != null && reader.IsDBNull(8);
            
            if (catNeedsUpdate || chillNeedsUpdate || takeNeedsUpdate || cutNeedsUpdate)
            {
                string setClause = "";
                string Fmt(decimal? d) => d.HasValue ? d.Value.ToString(CultureInfo.InvariantCulture) : "NULL";
                
                if (catNeedsUpdate) {
                    var parsed = ParseRange(catGap);
                    if (parsed.target != null)
                        setClause += $"CaterpillarGap_Min={Fmt(parsed.min)}, CaterpillarGap_Asli={Fmt(parsed.target)}, CaterpillarGap_Max={Fmt(parsed.max)}, ";
                }
                
                if (chillNeedsUpdate) {
                    var parsed = ParseRange(chiller);
                    if (parsed.target != null)
                        setClause += $"ChillerWaterTemp_Min={Fmt(parsed.min)}, ChillerWaterTemp_Asli={Fmt(parsed.target)}, ChillerWaterTemp_Max={Fmt(parsed.max)}, ";
                }
                
                if (takeNeedsUpdate) {
                    var parsed = ParseRange(takeup);
                    if (parsed.target != null)
                        setClause += $"TakeUpConveyorSpeed_Min={Fmt(parsed.min)}, TakeUpConveyorSpeed_Asli={Fmt(parsed.target)}, TakeUpConveyorSpeed_Max={Fmt(parsed.max)}, ";
                }
                
                if (cutNeedsUpdate) {
                    var parsed = ParseRange(cutting);
                    if (parsed.target != null)
                        setClause += $"CuttingSpeed_Min={Fmt(parsed.min)}, CuttingSpeed_Asli={Fmt(parsed.target)}, CuttingSpeed_Max={Fmt(parsed.max)}, ";
                }
                
                setClause = setClause.TrimEnd(',', ' ');
                
                if (setClause != "")
                {
                    updates.Add($"UPDATE SpsNoDocs SET {setClause} WHERE DocumentNumber = '{docNumber.Replace("'", "''")}';");
                }
            }
        }
    }
    
    Console.WriteLine($"Found {updates.Count} rows to update.");
    int i = 0;
    foreach(var u in updates)
    {
        using(var uCmd = new SqlCommand(u, conn))
        {
            uCmd.ExecuteNonQuery();
        }
        i++;
    }
    Console.WriteLine($"Successfully updated {i} rows.");
}
