using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VelastoProductionSystem.Helpers
{
    public static class ShiftHelper
    {
        public static readonly (string Name, string StartTime)[] CompanyShifts =
        {
            ("Shift 1", "19:30"),
            ("Shift 2", "07:30"),
        };

        public static string NormalizeShiftLabel(string? shift)
        {
            if (string.IsNullOrWhiteSpace(shift)) return "Shift 1";

            var raw = shift.Trim().ToUpper();
            var stripped = raw.Replace("SHIFT", "").Trim();

            if (stripped is "1" or "I" || raw.Contains("SIANG") || raw.Contains("ONE"))
                return "Shift 1";
            if (stripped is "2" or "II" || raw.Contains("SORE") || raw.Contains("TWO"))
                return "Shift 2";
            if (stripped is "3" or "III" || raw.Contains("MALAM") || raw.Contains("THREE"))
                return "Shift 3";

            return shift.Trim();
        }

        public static string GetCurrentShift(IEnumerable<ShiftMaster>? shiftMasters = null)
        {
            try
            {
                var shifts = shiftMasters?.ToList();
                if (shifts == null || !shifts.Any())
                    return GetCurrentShiftByFallback();

                var now = DateTime.Now.TimeOfDay;

                var shiftInfos = shifts
                    .Select(s =>
                    {
                        TimeSpan.TryParse(s.StartTime ?? "07:30", out var start);
                        return new { Name = NormalizeShiftLabel(s.ShiftName), Start = start };
                    })
                    .OrderBy(s => s.Start)
                    .ToList();

                for (int i = 0; i < shiftInfos.Count; i++)
                {
                    var current = shiftInfos[i];
                    var next = (i + 1 < shiftInfos.Count) ? shiftInfos[i + 1] : shiftInfos[0];

                    if (current.Start < next.Start)
                    {
                        if (now >= current.Start && now < next.Start) return current.Name;
                    }
                    else
                    {
                        if (now >= current.Start || now < next.Start) return current.Name;
                    }
                }

                return shiftInfos.FirstOrDefault()?.Name ?? "Shift 1";
            }
            catch
            {
                return GetCurrentShiftByFallback();
            }
        }

        private static string GetCurrentShiftByFallback()
        {
            var now = DateTime.Now.TimeOfDay;
            var shift2Start = new TimeSpan(7, 30, 0);
            var shift2End = new TimeSpan(19, 30, 0);
            return (now >= shift2Start && now < shift2End) ? "Shift 2" : "Shift 1";
        }

        public static async Task EnsureCompanyShiftsAsync(ApplicationDbContext context)
        {
            var existing = await context.ShiftMasters.ToListAsync();

            if (!existing.Any())
            {
                context.ShiftMasters.AddRange(CompanyShifts.Select(s => new ShiftMaster
                {
                    ShiftName = s.Name,
                    StartTime = s.StartTime
                }));
                await context.SaveChangesAsync();
                return;
            }

            foreach (var (name, startTime) in CompanyShifts)
            {
                var match = existing.FirstOrDefault(s => ShiftNumber(s.ShiftName) == ShiftNumber(name));
                if (match != null)
                {
                    match.ShiftName = name;
                    match.StartTime = startTime;
                    match.UpdatedAt = DateTime.Now;
                }
                else
                {
                    context.ShiftMasters.Add(new ShiftMaster { ShiftName = name, StartTime = startTime });
                }
            }

            await context.SaveChangesAsync();
        }

        private static int ShiftNumber(string? name)
        {
            var label = NormalizeShiftLabel(name);
            return label switch
            {
                "Shift 1" => 1,
                "Shift 2" => 2,
                "Shift 3" => 3,
                _ => 0
            };
        }
    }
}
