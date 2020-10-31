cd src
dotnet publish --self-contained true -r win-x64 -p:UseAppHost=true -c Release
cd ..