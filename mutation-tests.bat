@echo off

pushd "%~dp0"

dotnet tool restore
dotnet restore demo-dotnet-mutation-tests-stryker.slnx
dotnet build demo-dotnet-mutation-tests-stryker.slnx --no-restore
dotnet stryker --solution demo-dotnet-mutation-tests-stryker.slnx --reporter json --reporter cleartext --reporter html -O StrykerOutput/

popd
