# PowerShell script to test the integration between BuildingService and AttachmentService
# This script verifies that BuildingService correctly retrieves AttachmentUrls from AttachmentService

$buildingServiceUrl = "http://localhost:5002"
$attachmentServiceUrl = "http://localhost:5009"
$bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
$tenantHeader = "test"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Testing BuildingService + AttachmentService Integration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Test AttachmentService directly
Write-Host "Step 1: Testing AttachmentService directly..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $bearerToken"
        "Tenant" = $tenantHeader
    }
    
    # Test getting a single attachment
    $attachmentId = "e31853f8-f323-476c-8ca9-c0d81dab76a5"
    $response = Invoke-RestMethod -Uri "$attachmentServiceUrl/api/attachments/$attachmentId" -Method Get -Headers $headers
    Write-Host "✅ AttachmentService responds correctly" -ForegroundColor Green
    Write-Host "   Attachment Name: $($response.name)" -ForegroundColor Cyan
    Write-Host "   Attachment URL: $($response.attachmentUrl)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ AttachmentService test failed: $_" -ForegroundColor Red
    Write-Host "   Make sure AttachmentService is running on port 5009" -ForegroundColor Yellow
}

Write-Host ""

# Step 2: Test BuildingService with attachment URLs
Write-Host "Step 2: Testing BuildingService API (should include AttachmentUrls)..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $bearerToken"
        "Tenant" = $tenantHeader
    }
    
    # Get building details with attachments
    $buildingId = "33a2c435-f24c-4605-9286-b3bd6389f3e7"  # Condominio Bella Vista
    $response = Invoke-RestMethod -Uri "$buildingServiceUrl/api/buildings/$buildingId" -Method Get -Headers $headers
    
    Write-Host "✅ BuildingService responds correctly" -ForegroundColor Green
    Write-Host "   Building Name: $($response.name)" -ForegroundColor Cyan
    Write-Host "   Number of IBANs: $($response.ibans.Count)" -ForegroundColor Cyan
    Write-Host "   Number of Attachments: $($response.attachments.Count)" -ForegroundColor Cyan
    
    if ($response.attachments.Count -gt 0) {
        Write-Host ""
        Write-Host "   Attachments Details:" -ForegroundColor Yellow
        foreach ($attachment in $response.attachments) {
            Write-Host "   - $($attachment.name)" -ForegroundColor White
            if ($attachment.attachmentUrl) {
                Write-Host "     ✅ URL: $($attachment.attachmentUrl)" -ForegroundColor Green
            } else {
                Write-Host "     ⚠️ URL: null (AttachmentService might be down or attachment not found)" -ForegroundColor Yellow
            }
        }
    }
    
} catch {
    Write-Host "❌ BuildingService test failed: $_" -ForegroundColor Red
    Write-Host "   Make sure BuildingService is running on port 5002" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Integration Test Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan