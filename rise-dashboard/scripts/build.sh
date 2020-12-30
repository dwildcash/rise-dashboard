#!/bin/pwsh

Write-host -ForegroundColor Green "Stopping Dashboard..."
systemctl stop kestrel-rise-dashboard.service

cd /var/www/rise-dashboard/
pkill dotnet

Write-host -ForegroundColor Green "Fetching Latest version from github.."
git pull

Write-host -ForegroundColor Green "Building new version..."
dotnet build
chown -R www-data:www-data /var/www/rise-dashboard
systemctl start kestrel-rise-dashboard.service
