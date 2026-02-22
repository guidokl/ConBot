# themedemo.ps1

$themes = @(
    "Default",
    "Cyberpunk",
    "Matrix",
    "DeepOcean",
    "Dracula",
    "Solarized",
    "MSDOS",
    "Nord",
    "LightMode",
    "Monokai",
    "Coffee",
    "CrimsonParchment",
    "LavenderMist",
    "MintyBreeze",
    "DustyRose",
    "SynthwaveExtreme",
    "ArcadeNeon",
    "ToxicAlert",
    "Hacker",
    "Midnight",
    "Autumn",
    "Royal",
    "Monochrome",
    "Forest",
    "CottonCandy"
)

$query = "How do I extract a .tar.gz file?"

foreach ($theme in $themes) {
    Write-Host ""
    Write-Host "=== Theme: $theme ===" -ForegroundColor Cyan
    Write-Host ""
    
    # Execute ConBot with the unquoted query and the dynamic theme flag
    conbot $query -t $theme
    
    Write-Host ""
}