# signalr-core
Sandbox for SignalR Core + Redis

There are ASP.NET Core web application with chat using SignalR and console application that sends the message to chat separately from browsers

To run it correctly:
1. install chocolatey (https://chocolatey.org/) 
2. install redis using choco and run redis
3. make sure default redis port is used (6379)
4. run asp.net application in multiple browser tabs and observe communication
5. run console application and observe the messages from console are appearing in browser

Next steps:
1. make console app less dependent on signalr core source code
2. introduce typescript client instead of js
3. make angular based application with typescript client using webpack