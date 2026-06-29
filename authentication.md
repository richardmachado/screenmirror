## signup

curl.exe -X POST "http://localhost:4000/auth/signup" -H "Content-Type: application/json" -d "@body.json"

check body.json so its a new users

## create a session

$token = "PASTE_YOUR_ACCESS_TOKEN"
$targetUserId = "PASTE_TARGET_USER_ID"

$headers = @{
  Authorization = "Bearer $token"
  "Content-Type" = "application/json"
}

$body = @{
  target_user_id = $targetUserId
} | ConvertTo-Json

$response = Invoke-RestMethod `
  -Uri "http://localhost:4000/sessions" `
  -Method POST `
  -Headers $headers `
  -Body $body

$response
 

## approve a session

$token = "PASTE_YOUR_ACCESS_TOKEN"
$sessionId = "PASTE_SESSION_ID"

$headers = @{
  Authorization = "Bearer $token"
  "Content-Type" = "application/json"
}

$response = Invoke-RestMethod `
  -Uri "http://localhost:4000/sessions/$sessionId/approve" `
  -Method POST `
  -Headers $headers

$response

