# signalr-core
Sandbox for SignalR Core + Redis

There are ASP.NET Core web application with chat using SignalR and console application that sends the message to chat separately from browsers

To run it correctly:
1. install chocolatey (https://chocolatey.org/) 
2. install redis using choco and run redis
    - choco install redis-64
    - redis-server
3. make sure default redis port is used (6379) 
    - in order to see all redis activities install redis-cli  and run cmd "redis-cli monitor"
4. run asp.net application in multiple browser tabs and observe communication
5. run console application and observe the messages from console are appearing in browser

Next steps:
1. make console app less dependent on signalr core source code (not possible to make it independent as asp.net\signalr team is not going to make it happen in near future)
2. introduce typescript client instead of js (dev version is already there)
3. make angular based application with typescript client using webpack