
# ------- Generate Certificate Authority --------- #

# generate ca private key
openssl genrsa -out private/ca.key 4096

# generate the CA (Certificate Authority)
openssl req -config ca.config \
      -key private/ca.key \
      -new -x509 -days 7300 -sha256 -extensions v3_ca \
      -out certs/ca.crt


# Perform some openssl manipulation to get the cert into the file extensions we need.
# Extract .crt file from pfx
# openssl pkcs12 -in server.pfx -clcerts -nokeys -out cert.crt -passin pass:$TEST_PASSWORD
#
# Extract encrypted private key from pfx 
# openssl pkcs12 -in server.pfx -nocerts -out cert-key-encrypted.key -passin pass:$TEST_PASSWORD -passout pass:$TEST_PASSWORD
# 
# Decrypt private key
# openssl rsa -in cert-key-encrypted.key -out cert-key.key -passin pass:$TEST_PASSWORD
# 
# 
# 
