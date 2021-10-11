$os = @("win-x64", "linux-arm", "osx-x64")
$goos = @("windows", "linux", "darwin")
$goarch = @("amd64", "arm", "amd64")

Write-Output "============="
Write-Output "BUILD STARTED"
Write-Output "============="

Remove-Item .\Build -Force -Recurse
Remove-Item .\*\bin -Force -Recurse
Remove-Item .\*\obj -Force -Recurse
Remove-Item TestResults -Force -Recurse

for ($i = 0; $i -lt $folder.length; $i++) {
    dotnet publish -o .\Build\$os[$i] -c Release -r $os[$i] -p:PublishSingleFile=true --self-contained true .\OkayegTeaTimeCSharp\OkayegTeaTimeCSharp.csproj
    Copy-Item .\OkayegTeaTimeCSharp\Resources .\Build\$os[$i]\Resources /E /Y /I
}

dotnet test .\Tests\Tests.csproj

node .\Tools\GitHub\ReadMeGenerator.js

cd .\Tools\Database
go run SqlCreateFormatter.go
cd ..\..

cd .\Starter
for ($i = 0; $i -lt $goos.length; $i++) {
    go env -w GOOS=$goos[$i] GOARCH=$goarch[$i]
    go build -o ..\Build\$os[$i]
    Write-Output "Built Starter for $($goos[$i])"
}
go env -w GOOS=$goos[0] GOARCH=$goarch[0]
cd ..

Write-Output "=============="
Write-Output "BUILD FINISHED"
Write-Output "=============="
