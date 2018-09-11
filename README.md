# signalr-core
Sandbox for SignalR Core + Redis

There are ASP.NET Core web application with chat using SignalR and console application that sends the message to chat separately from browsers

To run server correctly:
1. build JS bundle
    - install npm within wwwroot
    - npm run build
2. run redis\start-all.cmd
3. run asp.net application in multiple browser tabs and observe communication (localhost:50136 to be used)
4. run console application and observe the messages from console are appearing in browser

To run angular application separatelly:
1. install angular cli (npm install -g @angular/cli)
2. navigate to angular-client folder and run ng serve. The angular-based page will be available using localhost:4200

Latest version is build with master-slave redis architecture.
The source is taken from and adjusted for this test project (setinels are removed and master\slaves are adjusted):
https://github.com/ServiceStack/redis-config