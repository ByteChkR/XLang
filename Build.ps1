 param (
    [string]$server = "213.109.162.193",
    [switch]$deploy = $false,
    [Parameter(Mandatory=$false)][string]$username,
    [Parameter(Mandatory=$true)][string]$platform
 )

#Dotnet Publish console
dotnet publish src/XLang.sln --self-contained true -r ${platform} -p:UseAppHost=true -c Release
mv src/XLang.Console/bin/Release/netcoreapp2.1/${platform}/publish ${platform}

#Zip Content with examples and readme
zip XLang-${platform}-bin.zip Readme.md
zip -r XLang-${platform}-bin.zip examples
zip -r XLang-${platform}-bin.zip ${platform}

rm ${platform} -Recurse -Force -Confirm:$false

if ($deploy) {
    #Upload to SRV
    if($username)
    {
        scp -P 22 .\XLang-${platform}-bin.zip ${username}@${server}:/var/www/html/XLang-Install/XLang-${platform}-bin.zip

    } else {
        echo "No Value Specified for parameter: -username"
    }
    rm .\XLang-${platform}-bin.zip -Force -Confirm:$false
    
}
