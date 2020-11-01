cd src
dotnet publish --self-contained true -r linux-x64 -p:UseAppHost=true -c Release
cd ..