# signalr-core
Sandbox for SignalR Core + Redis

There are ASP.NET Core web application with chat using SignalR and console application that sends the message to chat separately from browsers

To run server correctly:
1. build JS bundle
    - install npm within wwwroot
    - npm run build
2. install chocolatey (https://chocolatey.org/) 
3. install redis using choco and run redis
    - choco install redis-64
    - redis-server
4. make sure default redis port is used (6379) 
    - in order to see all redis activities install redis-cli  and run cmd "redis-cli monitor"
5. run asp.net application in multiple browser tabs and observe communication (localhost:50136 to be used)
6. run console application and observe the messages from console are appearing in browser

To run angular application separatelly:
1. install angular cli (npm install -g @angular/cli)
2. navigate to angular-client folder and run ng serve. The angular-based page will be available using localhost:4200