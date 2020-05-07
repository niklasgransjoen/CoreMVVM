@echo off

dotnet build CoreMVVM.sln
dotnet test Tests\CoreMVVM.Tests\CoreMVVM.Tests.csproj
dotnet test Tests\CoreMVVM.Windows.Tests\CoreMVVM.Windows.Tests.csproj