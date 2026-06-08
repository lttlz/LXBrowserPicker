param(
    [string]$OutputPath = ".\assets\LXBrowserPicker.ico"
)

Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = "Stop"
$sizes = @(16, 32, 48, 64, 128, 256)
$pngImages = New-Object System.Collections.Generic.List[byte[]]

function New-IconPng {
    param([int]$Size)

    $bitmap = New-Object System.Drawing.Bitmap $Size, $Size, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.Clear([System.Drawing.Color]::Transparent)

    $scale = $Size / 256.0
    $rect = New-Object System.Drawing.RectangleF (18 * $scale), (18 * $scale), (220 * $scale), (220 * $scale)
    $radius = 48 * $scale
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $diameter = 2 * $radius
    $path.AddArc($rect.X, $rect.Y, $diameter, $diameter, 180, 90)
    $path.AddArc($rect.Right - $diameter, $rect.Y, $diameter, $diameter, 270, 90)
    $path.AddArc($rect.Right - $diameter, $rect.Bottom - $diameter, $diameter, $diameter, 0, 90)
    $path.AddArc($rect.X, $rect.Bottom - $diameter, $diameter, $diameter, 90, 90)
    $path.CloseFigure()

    $bg = New-Object System.Drawing.Drawing2D.LinearGradientBrush $rect, ([System.Drawing.Color]::FromArgb(255, 32, 104, 216)), ([System.Drawing.Color]::FromArgb(255, 21, 33, 63)), 45
    $graphics.FillPath($bg, $path)

    $routePen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 88, 225, 205)), ([Math]::Max(2, 18 * $scale))
    $routePen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $routePen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $graphics.DrawBezier($routePen, 72 * $scale, 140 * $scale, 110 * $scale, 140 * $scale, 142 * $scale, 120 * $scale, 174 * $scale, 90 * $scale)
    $graphics.DrawBezier($routePen, 118 * $scale, 140 * $scale, 140 * $scale, 140 * $scale, 152 * $scale, 156 * $scale, 174 * $scale, 168 * $scale)

    $whiteBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(245, 255, 255, 255))
    foreach ($point in @(@(188, 72), @(192, 160))) {
        $diameterNode = 56 * $scale
        $graphics.FillEllipse($whiteBrush, ($point[0] * $scale) - ($diameterNode / 2), ($point[1] * $scale) - ($diameterNode / 2), $diameterNode, $diameterNode)
    }

    $letterPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::White), ([Math]::Max(2, 16 * $scale))
    $letterPen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $letterPen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $letterPen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $graphics.DrawLines($letterPen, @(
        (New-Object System.Drawing.PointF (58 * $scale), (67 * $scale)),
        (New-Object System.Drawing.PointF (58 * $scale), (149 * $scale)),
        (New-Object System.Drawing.PointF (100 * $scale), (149 * $scale))
    ))
    $graphics.DrawLine($letterPen, 116 * $scale, 68 * $scale, 166 * $scale, 148 * $scale)
    $graphics.DrawLine($letterPen, 164 * $scale, 68 * $scale, 114 * $scale, 148 * $scale)

    $stream = New-Object System.IO.MemoryStream
    $bitmap.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
    $bytes = $stream.ToArray()

    $stream.Dispose()
    $letterPen.Dispose()
    $whiteBrush.Dispose()
    $routePen.Dispose()
    $bg.Dispose()
    $path.Dispose()
    $graphics.Dispose()
    $bitmap.Dispose()

    return $bytes
}

foreach ($size in $sizes) {
    $pngImages.Add((New-IconPng -Size $size))
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
