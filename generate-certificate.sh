echo "Create PEM cert"
openssl req -new -newkey rsa:4096 -days 365 -nodes -x509 \
    -subj "/C=DE/ST=Berlin/L=Berlin/CN=energyweb.org" \
    -keyout energyweb.key -out energyweb.cert

openssl x509 -in energyweb.cert -sha256 -fingerprint -noout

echo "Convert to PFX"
openssl pkcs12 -export -out telemetry-ingress.pfx -inkey energyweb.key -in energyweb.cert -password pass:EDKHDKxCEkiGGJd4kTRj7k6
rm energyweb.key
rm energyweb.cert

