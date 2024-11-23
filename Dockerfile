FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine

COPY . /

RUN apk update
RUN apk add bash unzip

RUN ./build.sh

CMD ~/.bun/bin/bun index.js
