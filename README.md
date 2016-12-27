# AntiSpam
![Version 1.2.0](https://img.shields.io/badge/Version-1.2.0-blue.svg)
![API 2.0](https://img.shields.io/badge/API-2.0-green.svg)

Introduction
-----
This plugin provides a chat filter, allowing automatic filtering of "spam".

Permissions
-----
`antispam.ignore`<br />
User's chat will not be filtered by this plugin.

Configuration
-----
_antispamconfig.json_

| Option | Type | Default |
|---|---|---|
|DisableBossMessages|Boolean|false|
|DisableOrbMessages|Boolean|false|
|Action|string|"ignore"|
|CapsRatio|double|0.66|
|CapsWeight|double|2.0|
|NormalWeight|double|1.0|
|ShortLength|int|4|
|ShortWeight|double|1.5|
|Threshold|double|5.0|
|Time|int|5|