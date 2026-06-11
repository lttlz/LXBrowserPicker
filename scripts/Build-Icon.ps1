param(
    [string]$SourcePath = ".\assets\LXBrowserPicker.master.png",
    [string]$OutputPath = ".\assets\LXBrowserPicker.ico"
)

Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = "Stop"
$sizes = @(16, 32, 48, 64, 128, 256)
$pngImages = New-Object System.Collections.Generic.List[byte[]]
$sourceFullPath = [System.IO.Path]::GetFullPath($SourcePath)

if (-not [System.IO.File]::Exists($sourceFullPath)) {
    throw "Source icon master was not found: $sourceFullPath"
}

function New-IconPng {
    param(
        [System.Drawing.Image]$Source,
        [System.Drawing.Rectangle]$SourceBounds,
        [int]$Size
    )

    $bitmap = New-Object System.Drawing.Bitmap $Size, $Size, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $graphics.Clear([System.Drawing.Color]::Transparent)
    $graphics.DrawImage($Source, (New-Object System.Drawing.Rectangle 0, 0, $Size, $Size), $SourceBounds, [System.Drawing.GraphicsUnit]::Pixel)

    $stream = New-Object System.IO.MemoryStream
    $bitmap.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
    $bytes = $stream.ToArray()

    $stream.Dispose()
    $graphics.Dispose()
    $bitmap.Dispose()

    return $bytes
}

function Get-OpaqueBounds {
    param([System.Drawing.Bitmap]$Bitmap)

    $minX = $Bitmap.Width
    $minY = $Bitmap.Height
    $maxX = -1
    $maxY = -1

    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            if ($Bitmap.GetPixel($x, $y).A -gt 0) {
                if ($x -lt $minX) { $minX = $x }
                if ($y -lt $minY) { $minY = $y }
                if ($x -gt $maxX) { $maxX = $x }
                if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }

    if ($maxX -lt $minX -or $maxY -lt $minY) {
        return New-Object System.Drawing.Rectangle 0, 0, $Bitmap.Width, $Bitmap.Height
    }

    return New-Object System.Drawing.Rectangle $minX, $minY, ($maxX - $minX + 1), ($maxY - $minY + 1)
}

$sourceImage = [System.Drawing.Bitmap]::FromFile($sourceFullPath)
try {
    $sourceBounds = Get-OpaqueBounds -Bitmap $sourceImage
    foreach ($size in $sizes) {
        $pngImages.Add((New-IconPng -Source $sourceImage -SourceBounds $sourceBounds -Size $size))
    }
}
finally {
    $sourceImage.Dispose()
}

$outputFullPath = [System.IO.Path]::GetFullPath($OutputPath)
$outputDir = [System.IO.Path]::GetDirectoryName($outputFullPath)
if (-not [System.IO.Directory]::Exists($outputDir)) {
    [System.IO.Directory]::CreateDirectory($outputDir) | Out-Null
}

$file = [System.IO.File]::Create($outputFullPath)
$writer = New-Object System.IO.BinaryWriter $file
try {
    $writer.Write([UInt16]0)
    $writer.Write([UInt16]1)
    $writer.Write([UInt16]$sizes.Count)

    $offset = 6 + (16 * $sizes.Count)
    for ($i = 0; $i -lt $sizes.Count; $i++) {
        $size = $sizes[$i]
        $bytes = $pngImages[$i]
        $writer.Write([byte]$(if ($size -eq 256) { 0 } else { $size }))
        $writer.Write([byte]$(if ($size -eq 256) { 0 } else { $size }))
        $writer.Write([byte]0)
        $writer.Write([byte]0)
        $writer.Write([UInt16]1)
        $writer.Write([UInt16]32)
        $writer.Write([UInt32]$bytes.Length)
        $writer.Write([UInt32]$offset)
        $offset += $bytes.Length
    }

    foreach ($bytes in $pngImages) {
        $writer.Write($bytes)
    }
}
finally {
    $writer.Dispose()
    $file.Dispose()
}

Write-Host "Wrote $outputFullPath"
