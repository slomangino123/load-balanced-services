#!/bin/sh
openssl req -config cert.config -new -out cert-csr.pem
openssl x509 -req -days 3650 -extfile cert.config -extensions v3_req -in cert-csr.pem -signkey cert-key.pem -out cert.crt -CAkey rootCA.key -CA rootCA.crt -CAcreateserial -sha256
openssl pkcs12 -export -out cert.pfx -inkey cert-key.pem -in cert.crt -password pass:TestPass123

# Generate the certificate
New-SelfSignedCertificate -DnsName "localhost", "host.docker.internal", "127.0.0.1", "service1", "service2", "compose_nginx1_1", "compose_nginx2_1" -KeyUsage CertSign, DigitalSignature, KeyEncipherment -FriendlyName "Better ASP.NET Core HTTPS development certificate" -CertStoreLocation "Cert:\CurrentUser\My"

# Export the generated cert to a pfx file.
# -> run
# -> mmc.exe
# -> add/remove snapins
# -> certificates
# -> Current user
# -> find localhost cert
# -> copy to file
# -> export private key
# -> export all extended properties
# -> Password: TestPass123
# -> cert.pfx
# -> finish

# Extract .crt file from pfx
openssl pkcs12 -in cert.pfx -clcerts -nokeys -out cert.crt
#
# Extract encrypted private key from pfx 
openssl pkcs12 -in cert.pfx -nocerts -out cert-key-e.key
# 
# Decrypt private key
openssl rsa -in cert-key-e.key -out cert-key.key
# 
# 
# 
