::Notes


:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

:: Configurations
@set locationId=1


:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

@echo off
@setlocal EnableDelayedExpansion

@set customerApiUrl=http://alpha_customer_api_LOCAL_%locationId%:5001
@set reportsApiUrl=http://alpha_reports_api_LOCAL_%locationId%:5001
@set betlistsApiUrl=http://alpha_betlists_api_LOCAL_%locationId%:5001
@set apiKeyApiUrl=http://alpha_apikey_api_LOCAL_%locationId%:5001
@set alphaApiServerUrl=https://alpha-gateways.nexdev.net/v%locationId%
@set extApiServerUrl=https://ext-gateways.nexdev.net/v%locationId%

@set settingsFile=appsettings.LOCAL_DEV.json
@set resultFile=appsettings.LOCAL_DEV.json

@call cd ../src/ApiGateways/Ext.ApiGateways
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.GrpcCustomer='%customerApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.GrpcReports='%reportsApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.GrpcBetLists='%betlistsApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.ApiServer='%extApiServerUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"

@call cd ../Alpha.ApiGateways
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.GrpcCustomer='%customerApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.GrpcReports='%reportsApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.GrpcApiKey='%apiKeyApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"
@node -e "let pkg=require('./%settingsFile%'); pkg.Urls.ApiServer='%alphaApiServerUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"



@call cd ../../../src/Services/Alpha.BetLists.API
@node -e "let pkg=require('./%settingsFile%'); pkg.BetListsAppSettings.GrpcCustomer='%customerApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"


@call cd ../../../src/Services/Alpha.Reports.API
@node -e "let pkg=require('./%settingsFile%'); pkg.ReportsAppSettings.GrpcCustomer='%customerApiUrl%'; require('fs').writeFileSync('%resultFile%', JSON.stringify(pkg, null, 4));"



@pause
@exit /b

:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


