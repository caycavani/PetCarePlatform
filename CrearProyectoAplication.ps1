$arch = if ([Environment]::Is64BitOperatingSystem) { "x64" } else { "x86" }
$url = "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.100-windows-$arch-installer"
Start-Process $url