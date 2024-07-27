::
::
echo off
echo........................................
echo Generate solution code from added Entity classes
echo........................................
echo off
:PROMPT
SET /P AREYOUSURE=Are you sure you want to delete generated files(Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END

::Select the VS version
::SET tt="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\TextTransform.exe"
::SET tt="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\TextTransform.exe"
::SET tt="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\TextTransform.exe"
::SET tt="C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\TextTransform.exe"
::SET tt="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\TextTransform.exe"
SET tt="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\TextTransform.exe"
::SET tt="C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\TextTransform.exe" - there is an issue with running this 2022 version

echo off
echo Delete previously generated cs code files 
 DEL /F "1_t4EntityHelpersGenerate.cs"
 DEL /F "..\AppLicenseserver.Domain\Domain\2_t4DomainViewModelsGenerate.cs"	
 DEL /F "..\AppLicenseserver.Domain\Mapping\3_t4DomainMappingProfileGenerate.cs"	
 DEL /F "..\AppLicenseserver.Domain\Service\4_t4DomainServicesGenerate.cs"	
 DEL /F "..\AppLicenseserver.Api\Controllers\5_t4ApiControllerGenerate.cs"	
 DEL /F "..\AppLicenseserver.Api\5_t4ApiStartupAdditionsGenerate.cs"	
 DEL /F "..\AppLicenseserver.Test\6_t4IntegrationTestGenerate.cs"
echo .
echo Run all T4s...
echo -generate entity helpers
%tt% "1_t4EntityHelpersGenerate.tt" -out "1_t4EntityHelpersGenerate.cs"
echo -generate domain classes
%tt% "..\AppLicenseserver.Domain\Domain\2_t4DomainViewModelsGenerate.tt" -out "..\AppLicenseserver.Domain\Domain\2_t4DomainViewModelsGenerate.cs"
echo -generate mapper classes
%tt% "..\AppLicenseserver.Domain\Mapping\3_t4DomainMappingProfileGenerate.tt" -out "..\AppLicenseserver.Domain\Mapping\3_t4DomainMappingProfileGenerate.cs"	
echo -generate services classes
%tt% "..\AppLicenseserver.Domain\Service\4_t4DomainServicesGenerate.tt" -out "..\AppLicenseserver.Domain\Service\4_t4DomainServicesGenerate.cs"	
echo -generate controller classes
%tt% "..\AppLicenseserver.Api\Controllers\5_t4ApiControllerGenerate.tt" -out "..\AppLicenseserver.Api\Controllers\5_t4ApiControllerGenerate.cs"
echo -generate extended Startup code
%tt% "..\AppLicenseserver.Api\5_t4ApiStartupAdditionsGenerate.tt" -out "..\AppLicenseserver.Api\5_t4ApiStartupAdditionsGenerate.cs"
echo -generate Postman json tests
%tt% "..\AppLicenseserver.Test\Postman\t4PostmanTestsGenerate.tt" -out "..\AppLicenseserver.Test\Postman\RestApiN.Postman_tests_collection.json"		
echo -generate test classes
%tt% "..\AppLicenseserver.Test\6_t4IntegrationTestGenerate.tt" -out "..\AppLicenseserver.Test\6_t4IntegrationTestGenerate.cs"	
echo -add new db migration
%tt% "t4_AddMigration.tt" -out "t4_AddMigration.cs"
echo T4s completed.
echo !!! Rename the migration file !!!
pause
:END