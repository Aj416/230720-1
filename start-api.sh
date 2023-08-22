#!/bin/bash

set -e
aws --version

echo "Starting API.."
dotnet Tigerspike.Solv.Api.dll