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
missing_fields.discard("Id") # Already in Edit.cshtml
missing_fields.discard("ItemLists") # Navigation property

html = "<!-- HIDDEN FIELDS FOR PRESERVING DATA NOT IN FORM -->\n"
for field in sorted(missing_fields):
    html += f'<input type="hidden" asp-for="{field}" />\n'

with open('f:\\extrude\\scratch\\hidden_fields.html', 'w') as f:
    f.write(html)
