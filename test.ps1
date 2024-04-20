#!/usr/bin/env pwsh
# Copyright (c) 2024 Roger Brown.
# Licensed under the MIT License.

trap
{
	throw $PSItem
}

$ErrorActionPreference = 'Stop'
$InformationPreference = 'Continue'

Get-Command -Noun 'Base64String'

$bytes = [System.Text.Encoding]::ASCII.GetBytes('Hello World')

$base64 = @(,$bytes) | ConvertTo-Base64String

[psobject]@{
	Base64String=$base64
} | Format-Table

$result = $base64 | ConvertFrom-Base64String

$bytes,$result | Format-Hex
