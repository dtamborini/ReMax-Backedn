# PowerShell script to remove all bin and obj directories from Git tracking
Write-Host "Removing bin and obj directories from Git tracking..." -ForegroundColor Yellow

# Get all bin and obj directories
$directories = Get-ChildItem -Path . -Include bin,obj -Recurse -Directory

foreach ($dir in $directories) {
    $relativePath = $dir.FullName.Replace($PWD.Path + "\", "").Replace("\", "/")
    Write-Host "Removing: $relativePath" -ForegroundColor Cyan
    
    # Remove from Git tracking (ignore errors if not tracked)
    git rm -r --cached $relativePath 2>$null
}

Write-Host "`nDone! Now commit the changes:" -ForegroundColor Green
Write-Host "git commit -m 'Remove build artifacts from tracking'" -ForegroundColor White