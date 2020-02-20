#!/bin/sh

echo 'Stopping Dashboard'
systemctl stop kestrel-rise-dashboard.service
cd /var/www/rise-dashboard/
pkill dotnet

echo 'Fetching Latest version from github'
git pull

echo 'Building new version'
dotnet build
chown -R www-data:www-data /var/www/rise-dashboard
systemctl start kestrel-rise-dashboard.service
