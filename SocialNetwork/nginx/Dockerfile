FROM nginx
 
RUN openssl dhparam -out /etc/ssl/certs/dhparam.pem 2048 && \
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /etc/ssl/certs/self.key -out /etc/ssl/certs/self.crt \
    -subj "/C=RU/ST=Kraken/L=Kraken/O=Kraken/OU=Kraken/CN=Kraken"

COPY nginx.conf /etc/nginx/nginx.conf
