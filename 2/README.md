### Resources
Initial starting point for docker compose
https://codeburst.io/load-balancing-an-asp-net-core-web-app-using-nginx-and-docker-66753eb08204

helpful openssl commands
https://www.markbrilman.nl/2011/08/howto-convert-a-pfx-to-a-seperate-key-crt-file/
https://www.sslshopper.com/article-most-common-openssl-commands.html

How to setup the dev certificate when using Docker in development 
https://github.com/dotnet/AspNetCore.Docs/issues/6199

#### Code
Look at the request cert
https://stackoverflow.com/questions/777607/the-remote-certificate-is-invalid-according-to-the-validation-procedure-using

Rob Rich presentation on this topic
https://www.youtube.com/watch?v=oAf3_8k17E8
https://github.com/robrich/https-aspnet-core-docker-deep-dive/blob/main/7.%20mkcert/HTTPSPlayground.API/Dockerfile


### Known Bugs
Openssl requiring certificate signing
https://github.com/openssl/openssl/issues/1418

dev certs on linux
https://github.com/dotnet/aspnetcore/issues/7246

NGINX proxy cant get real remote ip
https://github.com/nginx-proxy/nginx-proxy/issues/133
might be the fix for above bug? https://github.com/nginx-proxy/nginx-proxy/issues/133#issuecomment-456764274

Docker compose network_mode: host is only available for linux hosts :feelsbadman:
https://docs.docker.com/network/network-tutorial-host/#prerequisites