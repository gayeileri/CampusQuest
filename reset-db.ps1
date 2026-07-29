<#
.SYNOPSIS
	Simple helper to clear UserMissions table in local SQLite DB used by CampusQuest.API.

.DESCRIPTION
	Deletes rows from UserMissions. By default deletes all rows. Optionally pass -MissionId to remove
	only rows for a specific mission GUID.

.USAGE
	.\reset-db.ps1
	.\reset-db.ps1 -MissionId "11111111-1111-1111-1111-111111111111"
	.\reset-db.ps1 -DbPath 'CampusQuest.API\\CampusQuest.db'

NOTE: This script expects sqlite3 to be available on PATH. On Windows you can install the sqlite3
	  CLI or use the sqlite3.exe that comes with many packages.
#>

[CmdletBinding()]
param(
	[string]$DbPath = "CampusQuest.API\CampusQuest.db",
	[string]$MissionId = ""
)

try {
	$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
	$fullDbPath = if ([System.IO.Path]::IsPathRooted($DbPath)) { $DbPath } else { Join-Path $scriptDir $DbPath }

	if (-not (Test-Path $fullDbPath)) {
		Write-Error "Database file not found: $fullDbPath"
		exit 2
	}

	# Build SQL
	if ([string]::IsNullOrWhiteSpace($MissionId)) {
		$sql = "DELETE FROM UserMissions;"
	} else {
		$escaped = $MissionId.Replace("'","''")
		$sql = "DELETE FROM UserMissions WHERE MissionId='${escaped}';"
	}

	Write-Host "Running:" sqlite3 `"$fullDbPath`" "$sql"

	$proc = Start-Process -FilePath "sqlite3" -ArgumentList @($fullDbPath, $sql) -NoNewWindow -Wait -PassThru -RedirectStandardOutput "$env:TEMP\reset-db-out.txt" -RedirectStandardError "$env:TEMP\reset-db-err.txt"

	if ($proc.ExitCode -ne 0) {
		Write-Error "sqlite3 exited with code $($proc.ExitCode). See $env:TEMP\reset-db-err.txt"
		Get-Content "$env:TEMP\reset-db-err.txt" -ErrorAction SilentlyContinue
		exit $proc.ExitCode
	}

	# Show remaining rows count
	$countSql = "SELECT COUNT(*) FROM UserMissions;"
	$count = & sqlite3 $fullDbPath $countSql 2>$null
	Write-Host "UserMissions rows remaining: $count"
	Write-Host "Done."
}
catch {
	Write-Error $_.Exception.Message
	exit 1
}
