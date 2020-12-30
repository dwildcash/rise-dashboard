#!/bin/pwsh

write-host -ForegroundColor Magenta "Stopping Dashboard..."
systemctl stop kestrel-rise-dashboard.service

cd /var/www/rise-dashboard/
pkill dotnet

write-host -ForegroundColor Green "Fetching Latest version from github.."
git pull

write-host -ForegroundColor Green "Building new version..."
dotnet build
chown -R www-data:www-data /var/www/rise-dashboard

write-host -ForegroundColor Magenta "Restarting Dashboard"
systemctl start kestrel-rise-dashboard.service
