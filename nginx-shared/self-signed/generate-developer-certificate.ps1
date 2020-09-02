# This will generate a self signed certificate with the correct usages in a docker environment
# See the readme -> bugs section for why we need to generate it this way. 
# tldr; openssl has a bug that requires the cert to have keyCertSign turned on

# Set the dev environment password to be used on the certificate.
$TEST_PASSWORD = "TestPass123"

$SECURE_PASSWORD = ConvertTo-SecureString -String $TEST_PASSWORD -Force -AsPlainText

# Generate the certificate. 
# New-SelfSignedCertificate needs to put the cert into a cert store
# so we are grabbing the thumbprint off of the command and saving it to a variable so we can
# export it and then remove it from the store.

# New-SelfSignedCertificate -DnsName "localhost", "host.docker.internal", "127.0.0.1", "service1", "service2", "compose_nginx1_1" -KeyUsage CertSign, DigitalSignature, KeyEncipherment -FriendlyName ".net Core Https development certificate" -CertStoreLocation "Cert:\CurrentUser\My"
$THUMBPRINT = (New-SelfSignedCertificate -DnsName "localhost" -KeyUsage CertSign, DigitalSignature, KeyEncipherment -FriendlyName ".net Core Https development certificate" -CertStoreLocation "Cert:\CurrentUser\My").Thumbprint

# Export the certificate as a .pfx file
Get-ChildItem -Path Cert:\CurrentUser\My\$THUMBPRINT | Export-PfxCertificate -FilePath .\cert.pfx -Password $SECURE_PASSWORD

# Remove the certificate from the windows store, it doesnt need to be there
Remove-Item -Path Cert:\CurrentUser\My\$THUMBPRINT

# Perform some openssl manipulation to get the cert into the file extensions we need.
# Extract .crt file from pfx
openssl pkcs12 -in cert.pfx -clcerts -nokeys -out cert.crt -passin pass:$TEST_PASSWORD
#
# Extract encrypted private key from pfx 
openssl pkcs12 -in cert.pfx -nocerts -out cert-key-encrypted.key -passin pass:$TEST_PASSWORD -passout pass:$TEST_PASSWORD
# 
# Decrypt private key
openssl rsa -in cert-key-encrypted.key -out cert-key.key -passin pass:$TEST_PASSWORD
# 
# 
# 
