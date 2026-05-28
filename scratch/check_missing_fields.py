import re

with open('f:\\extrude\\Models\\SpsMaster.cs', 'r') as f:
    cs_content = f.read()

with open('f:\\extrude\\Views\\SpsMaster\\Edit.cshtml', 'r', encoding='utf-8') as f:
    cshtml_content = f.read()

# Extract properties from SpsMaster.cs
prop_pattern = re.compile(r'public \w+\?? (\w+) \{ get; set; \}')
properties = prop_pattern.findall(cs_content)

# Extract asp-for fields from Edit.cshtml
aspfor_pattern = re.compile(r'asp-for="([^"]+)"')
aspfor_fields = aspfor_pattern.findall(cshtml_content)

missing_fields = set(properties) - set(aspfor_fields)

print("Properties in model:", len(properties))
print("Fields in view:", len(aspfor_fields))
print("Missing fields in view:", len(missing_fields))
for field in sorted(missing_fields):
    print("- " + field)
