param(
	[string]$queueType = 'msmq',
	[string]$siteCommand = 'web',
	[string[]]$handlerCommands = @('user', 'unsubscribe', 'legacy', 'crm', 'fulfilment'),
	[string]$sitePath = (Resolve-Path "$PSScriptRoot\src\POC"),
	[string]$handlerPath = (Resolve-Path "$PSScriptRoot\src\POC.Handler"),
	[string]$dnvmVersion = 'default'
)

# start handlers
$handlerCommands | % {  start powershell -Verb runas -argument " -noexit -command pushd $handlerPath; dnvm use $dnvmVersion; dnx $_ --QueueType $queueType"  }

# start site
start powershell -Verb runas -argument " -noexit -command pushd $sitePath; dnvm use $dnvmVersion; `$env:QueueType = '$queueType'; dnx $siteCommand"