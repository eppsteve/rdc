rdc
===

Remote Desktop Control implementation in C#. The application establishes a connection to a remote host
via sockets allowing remote control by sending mouse and keyboards commands. The desktop can be viewed 
remotely due to a mechanism which grabs a screenshot and sends it to the client. The library system32.dll
is used in order to send mouse commands.
