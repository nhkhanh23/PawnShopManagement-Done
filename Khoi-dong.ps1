param([switch]$NoBrowser)
$ErrorActionPreference = 'Stop'
$code = 0
$temporaryZip = $null
try {
    $app = Join-Path $PSScriptRoot 'Application'
    $exe = Join-Path $app 'PawnShopManagement.exe'
    $marker = Join-Path $app '.portable-ready'
    $expectedHash = 'E4151576E46BD4D67E8E9B333E286F299204EB3C6A50F3DFE25424B636477077'
    $prepared = (Test-Path -LiteralPath $exe) -and (Test-Path -LiteralPath $marker)
    if ($prepared) { $prepared = (Get-Content -LiteralPath $marker -Raw).Trim() -eq $expectedHash }
    if (-not $prepared) {
        Write-Host '[PAWNFLOW-ONE-CLICK] Preparing application on first run. Please wait...'
        $temporaryZip = Join-Path $PSScriptRoot ('.runtime-' + [Guid]::NewGuid().ToString('N') + '.zip')
        $outputStream = [System.IO.File]::Create($temporaryZip)
        try {
            foreach ($name in @('payload.001','payload.002','payload.003')) {
                $part = Join-Path (Join-Path $PSScriptRoot 'Runtime-goi') $name
                if (-not (Test-Path -LiteralPath $part)) { throw "Incomplete download: missing Runtime-goi/$name. Extract the FULL GitHub ZIP." }
                $inputStream = [System.IO.File]::OpenRead($part)
                try { $inputStream.CopyTo($outputStream) } finally { $inputStream.Dispose() }
            }
        } finally { $outputStream.Dispose() }
        $sha = [System.Security.Cryptography.SHA256]::Create()
        $hashStream = [System.IO.File]::OpenRead($temporaryZip)
        try { $actualHash = [BitConverter]::ToString($sha.ComputeHash($hashStream)).Replace('-', '') }
        finally { $hashStream.Dispose(); $sha.Dispose() }
        if ($actualHash -ne $expectedHash) {
            throw 'The downloaded runtime files are incomplete or mixed with another version. Download and extract the complete GitHub ZIP into a new folder.'
        }
        [System.Reflection.Assembly]::LoadWithPartialName('System.IO.Compression') | Out-Null
        [System.Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem') | Out-Null
        $archive = [System.IO.Compression.ZipFile]::OpenRead($temporaryZip)
        $safePrefix = [System.IO.Path]::GetFullPath($app).TrimEnd('\') + '\'
        try {
            foreach ($entry in $archive.Entries) {
                $target = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $entry.FullName))
                if (-not $target.StartsWith($safePrefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'Unexpected path in runtime package.' }
                if ($entry.FullName.EndsWith('/')) {
                    [System.IO.Directory]::CreateDirectory($target) | Out-Null
                } else {
                    [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($target)) | Out-Null
                    $inputFile = $entry.Open()
                    try {
                        $outputFile = [System.IO.File]::Create($target)
                        try { $inputFile.CopyTo($outputFile) } finally { $outputFile.Dispose() }
                    } finally { $inputFile.Dispose() }
                }
            }
        } finally { $archive.Dispose() }
        if (-not (Test-Path -LiteralPath $exe)) { throw 'Application could not be extracted. Please use a writable folder such as Documents.' }
        Set-Content -LiteralPath $marker -Value $expectedHash -Encoding ASCII
        Remove-Item -LiteralPath $temporaryZip
        $temporaryZip = $null
    }
    Write-Host 'Starting PawnFlow. No SDK or SQL Server installation required.'
    if ($NoBrowser) { & $exe --desktop --no-browser }
    else { & $exe --desktop }
    $code = $LASTEXITCODE
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    $code = 1
} finally {
    if ($temporaryZip -and (Test-Path -LiteralPath $temporaryZip)) { Remove-Item -LiteralPath $temporaryZip -ErrorAction SilentlyContinue }
}
exit $code
