using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(""Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;"");
using var _context = new ApplicationDbContext(optionsBuilder.Options);

var sps = _context.SpsItemLists
    .Include(i => i.SpsMasterlist)
    .FirstOrDefault(i => i.ItemCode == ""NA3311"");

if (sps == null) {
    Console.WriteLine(""SpsItemList for NA3311 is NULL!"");
} else {
    var m = sps.SpsMasterlist;
    Console.WriteLine($""Found SPS for NA3311. Masterlist: {m.DocumentNumber}"");
    Console.WriteLine($""TakeUpSpeed: Min='{m.TakeUpSpeedMin}', Std='{m.TakeUpSpeedStd}', Max='{m.TakeUpSpeedMax}'"");
    Console.WriteLine($""ChillerTemp: Min='{m.ChillerTempMin}', Std='{m.ChillerTempStd}', Max='{m.ChillerTempMax}'"");
}
