using OfficeOpenXml;
using System;

class Program {
    static void Main() {
        Console.WriteLine("Enum values for EPPlusLicenseType:");
        foreach (var val in Enum.GetValues(typeof(EPPlusLicenseType))) {
            Console.WriteLine($"- {val}");
        }
        
        var licenseType = typeof(ExcelPackage).GetProperty("License").PropertyType;
        Console.WriteLine("\nMethods of EPPlusLicense:");
        foreach (var m in licenseType.GetMethods()) {
            if (m.Name.Contains("Set"))
                Console.WriteLine($"- {m.Name}");
        }
    }
}
