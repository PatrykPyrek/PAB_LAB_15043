#!/usr/bin/env bash
set -euo pipefail

BASE_URL="http://localhost:5000"

echo "=== POST /auth/token ==="
token=$(curl -s -X POST "$BASE_URL/auth/token" \
  -H "Content-Type: application/json" \
  -d '{
    "username":"admin",
    "permissions":[
      "Projects.Read","Projects.Write",
      "Tasks.Read","Tasks.Write",
      "Subtasks.Read","Subtasks.Write"
    ]
  }' | sed -n 's/.*"token"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')

echo "Token: $token"

echo
echo "=== GET /projects ==="
curl -i -H "Authorization: Bearer $token" "$BASE_URL/projects"