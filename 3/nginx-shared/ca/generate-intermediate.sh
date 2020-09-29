# ------- Generate Intermediate Certificate --------- #

# generate intermediate private key
openssl genrsa \
      -out intermediate/private/intermediate.key 4096
      
# generate intermediate signing request
openssl req -config intermediate/intermediate.config -new -sha256 \
      -key intermediate/private/intermediate.key \
      -out intermediate/csr/intermediate.csr      

# generate the intermediate certificate in the chain
openssl ca -config ca.config \
      -extensions v3_intermediate_ca \
      -days 3650 -notext -md sha256 \
      -in intermediate/csr/intermediate.csr \
      -out intermediate/certs/intermediate.crt
      
# Print the intermediate certificate with no other output
openssl x509 -noout -text \
      -in intermediate/certs/intermediate.crt

# Verify the intermediate certificate chain using the CA
openssl verify -CAfile certs/ca.crt \
      intermediate/certs/intermediate.crt

# Create a new file representing the certificate chain from CA to intermediate
# shell
cat intermediate/certs/intermediate.crt \
      certs/ca.crt > intermediate/certs/ca-chain.crt

# powershell
# Get-Content certs/ca.crt, intermediate/certs/intermediate.crt | Out-file intermediate/certs/ca-chain.crt