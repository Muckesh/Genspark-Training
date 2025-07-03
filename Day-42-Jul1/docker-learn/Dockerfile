# Docker build

# using debian
FROM debian:bullseye

WORKDIR /app

# install node
RUN apt-get update &&\
    apt-get install -y curl &&\
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - &&\
    apt-get install -y nodejs

# copy the required files
COPY . .

EXPOSE 3000

CMD ["node","server.js"]