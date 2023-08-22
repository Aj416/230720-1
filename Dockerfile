# This docker file is used for AWS deployment only

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim

#Introducing ARGs
ARG ENV_NAME=NotDev

#install aws cli because we need to get ssm parameters later
RUN apt-get update && \
    apt-get install -y \
		jq \
        python3 \
        python3-pip \
        python3-setuptools \
    && pip3 install --upgrade pip \
    && apt-get clean
RUN pip3 --no-cache-dir install --upgrade awscli
RUN aws --version

#change workdir
WORKDIR /app

#copy app build output and NewRelic package
COPY /src/Tigerspike.Solv.Api/output .
COPY /newrelic /newrelic
COPY /start-api.sh /start-api.sh

#install NewRelic .Net Core agent
RUN if [ "$ENV_NAME" = "dev" ]; then echo "No need to install NewRelic"; else dpkg -i /newrelic/newrelic-netcore20-agent*.deb; fi

#open desired port
EXPOSE 80