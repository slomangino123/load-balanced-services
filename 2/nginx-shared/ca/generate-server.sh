
# ------- Generate Server Certificate --------- #
# Generate the server private key
openssl genrsa \
      -out intermediate/private/server.key 4096

# Use the private key to Generate a Signing request
openssl req -config intermediate/server.config \
      -key intermediate/private/server.key \
      -new -sha256 -out intermediate/csr/server.csr

# Sign the certificate request targeting the server_cert extension
openssl ca -config intermediate/server.config \
      -extensions server_cert -days 3750 -notext -md sha256 \
      -in intermediate/csr/server.csr \
      -out intermediate/certs/server.crt      
    
# Print the server certificate with no other output
openssl x509 -noout -text \
      -in intermediate/certs/server.crt      

# Verify the intermediate certificate chain using the CA
openssl verify -CAfile intermediate/certs/ca-chain.crt \
      intermediate/certs/server.crt      

# Use the pkcs12 tool to export the certificate as a .pfx using the private key and certificate chain.
openssl pkcs12 -export -in intermediate/certs/server.crt \
	 -inkey intermediate/private/server.key \
	 -out intermediate/certs/server.pfx \
	 -certfile intermediate/certs/ca-chain.crt \
       -password pass:TestPass123
