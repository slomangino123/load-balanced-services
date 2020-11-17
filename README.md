# Load Balanced Services

### Purpose
This project was to prove the TLS communication path between two ASP .net Core docker containers using docker-compose. In addition to TLS communication NGINX load balancers sit in front of each service which fowards all headers through to each service. Each service in turn, accepts the forwarded headers and recognize requests as TLS encrypted and attach HSTS headers to the response payload.

### Setup
##### Postman
1. Install Certificate
    - Open Postman and navigate to `Gear Icon -> Settings -> Certificates tab -> CA Certificates` Click on the browse button
    - Navigate to `Certificates -> ca -> intermediate -> certs -> Select ca-chain.crt`

Otherwise postman will get: `SSL Error: Unable to verify first certificate.`. Postman doesnt respect the certificates installed on your machine, it needs to be trusted directly.

##### Endpoints
- `/api/test`
- `/api/httpapicall`
- `/api/httpsapicall`

##### Ports
- Service1 Load balancer
    - http: 8101
    - https: 8102
- Service1 Direct
    - http: 8103
    - https: 8104
- Service2 Load balancer
    - http: 8201
    - https: 8202
- Service2 Direct
    - http: 8203
    - https: 8204
    


##### Example Requests & Responses
```
GET /api/test HTTP/1.1
Host: localhost:8102

Response:
{
    "ServiceMessage": "Hello from service1!",
    "HostName": "fd9a4d6f939f \t fd9a4d6f939f\t 10.1.0.2",
    "RequestMethod": "GET",
    "RequestScheme": "https",
    "RequestUrl": "https://nginx/api/test",
    "RequestPath": "/api/test",
    "RequestHeaders": [
        "Cache-Control: no-cache",
        "Connection: keep-alive",
        "Accept: */*",
        "Accept-Encoding: gzip, deflate, br",
        "Host: nginx",
        "User-Agent: PostmanRuntime/7.26.5",
        "X-Real-IP: 10.1.0.1",
        "X-Original-Proto: http",
        "X-Original-Host: localhost:8102",
        "X-Forwarded-Port: 8102",
        "Postman-Token: e711d2d2-db7f-4370-b5ef-c28bcbe28696",
        "X-Original-For: [::ffff:10.1.0.3]:37430"
    ],
    "RemoteIp": "10.1.0.1"
}
```


### Useful Resources
- [Docker compose tutorial](https://codeburst.io/load-balancing-an-asp-net-core-web-app-using-nginx-and-docker-66753eb08204)

- Useful openssl commands
    - [1](https://www.markbrilman.nl/2011/08/howto-convert-a-pfx-to-a-seperate-key-crt-file/)
    - [2](https://www.sslshopper.com/article-most-common-openssl-commands.html)

- [How to setup the dev certificate when using Docker in development](https://github.com/dotnet/AspNetCore.Docs/issues/6199)

##### Code
- [Stack overflow question](https://stackoverflow.com/questions/777607/the-remote-certificate-is-invalid-according-to-the-validation-procedure-using). The third answer down was a useful extension method to replace the CertificateValidationCallBack. This proved very useful when debugging the reason behind certificate validation failures.


- Rob Rich [presentation](https://www.youtube.com/watch?v=oAf3_8k17E8) and [github repo](https://github.com/robrich/https-aspnet-core-docker-deep-dive) on this topic




### Known Bugs
- [Openssl requiring certificate signing](https://github.com/openssl/openssl/issues/1418)

- [Dotnet dev certs on linux](https://github.com/dotnet/aspnetcore/issues/7246)

- [NGINX proxy cant get real remote ip](https://github.com/nginx-proxy/nginx-proxy/issues/133)
    - [might be the fix for above bug](https://github.com/nginx-proxy/nginx-proxy/issues/133#issuecomment-456764274)

- [Docker compose network_mode](https://docs.docker.com/network/network-tutorial-host/#prerequisites): host is only available for linux hosts
