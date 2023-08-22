# Solv Backend

[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-yellow.svg)](https://conventionalcommits.org)

### The backend of Solv platform. A .NET Core backend application, powered by .NET Core 5.0, based on a decoupled architecture and Docker containers.

## Table of Contents
- About
- Getting Started
- Deployment
- Usage
- Built Using
- [Contributing](CONTRIBUTING.md)
- Active Contributors
- Previous Contributors
- Acknowledgments

## About
This project provides a set of Services and Web APIs using .NET Core 5.0.

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See [deployment](#deployment) for notes on how to deploy the project on a live system.

### Prerequisites
Before you continue with the setup you will need to have Node 10+ installed on your machine and then run the bash script `install.sh`. This will install the NPM packages needed for [Conventional Commits](#commit_messages)

### Visual Studio 2017-2019 and Windows based
This is the more straightforward way to get started:
- Install the latest version of Docker for Desktop
- Use Docker in Visual Studio to run the database server or use Microsoft SQL Server and restore the database manually
- Run the Api as you would normally do with port 5200

### CLI and Windows based
For those who prefer the CLI on Windows, using dotnet CLI, docker CLI and VS Code for Windows:
- Install the relevant version of the dotnet SDK
- Install the latest version of Docker for Desktop
- Use `docker-compose -f docker-compose-external.yml up -d` to run the dependencies
- Use `dotnet run` or use an IDE such as Visual Studio Code to run the Api with port 5200

### CLI and Mac based
For those who prefer the CLI on a Mac, using dotnet CLI, docker CLI and VS Code for Mac:
- Install the relevant version of the dotnet SDK
- Install the latest version of Docker for Desktop
- Use `docker-compose -f docker-compose-external.yml up -d` to run the dependencies
- Use `dotnet run` or use an IDE such as Visual Studio Code to run the Api with port 5200

### Installing
- Follow the instructions on [Docker](https://docs.docker.com/install/) to install the latest version of Docker Community Edition for your OS.
- Go to [DotNet](https://dot.net) to install the latest version of the .NET SDK for your OS.

### Running the application using VS Code or Rider
The main IDEs that we are using are Visual Studio Code or Jetbrains Rider. You can also use Visual Studio 2019 on Windows if you prefer.

Before you run the application, you need to make sure that Docker Desktop is running and that you don't have old and dangling docker containers running which are related to the application.

You must first run the application infrastructure before you attempt to start the application locally. You can find the related scripts under the /script folder. Use `infra-run.sh` to start the infrastructure and `infra-down.sh` to stop it.

Once the infrastructure is confirmed to be running in the command line (failing to do so will result in an error), you can now run the application using your preferred IDE. 

At the moment we have the main API service under  src/Tigerspike.Solv.Api and the supporting services under /Services/<Service Name>/Tigerspike.Solv.Services.<Service Name>

In Rider you will need to create a compound configuration to be able to run All Services together (API and supporting services). You can follow the article [here](https://www.jetbrains.com/help/rider/Creating_Compound_Run_Debug_Configuration.html) or the [video](https://codeopinion.com/launching-multiple-projects-in-jetbrains-rider/)

### Running the entire application in Docker
Before you run the application, you need to make sure that Docker Desktop is running and that you don't have old and dangling docker containers running which are related to the application.

The application consists of two parts, the infrastructure such as MySQL instance, SQS, Dynamodb instances and the actual application services and they all run using docker containers.

You can find all the scripts required to run the application under the /scripts folder. 

To build the application use `app-build.sh' which will build the custom application images. You will need to run this command everytime you pull a new code and you want 

To run the application (after the application was built), use `app-run.sh` which will first run the infrastructure part of the application and then the custom application containers.

To stop the application from running, use `app-down.sh` which will bring down the containers.

## Running the tests
You can run the tests by using the standard `dotnet test` command or use your preferred IDE such as Visual Studio or Rider.

## Pull-Requests
Changes are only accepted through Pull Requests. You review the branching strategy document for more details.

## Commit Messages
We follow the Conventional Commits specification for adding human and machine readable meaning to commit messages. You can read more about it https://www.conventionalcommits.org/en/v1.0.0-beta.4/. To understand why we are enforcing the standard for commit messages you can read https://adrianperez.codes/enforcing-commit-conventions/.

### How to commit using Conventional Commits
If you are new to the standards, you can use `git cz` instead of `git commit` and follow the screen instructions. This tool will guide you to create the best commit message possible. For now we have a few requirements for the commit message as stated by the rules https://commitlint.js.org/#/reference-rules.
- In the subject it is preferrable to include the ticket ID in the format of DCTXS2-123
- Use lowercase for your commit message
- Use the present tense such as : feat(settings): add read-only display of user email DCTXS2-123

## Deployment
To release the software use the bitbucket master pipeline to create the release after it was deployed to SIT.

### UAT Deployment
To release to UAT, locate the pipeline related to master and the code that you want to deploy from SIT to UAT and then click on "Release to UAT". Another way to find it is to go to Deployments and looking under the SIT environment.

This will create a new tag with the version number and a new release branch with the version number as well. You can find the new release branch under release/<version number>

Whenever you deploy to UAT you need to make sure to follow the Runbook specified in the sprint.

### Production Deployment
To release to Production, the easiest would be to go to Deployments and clicking on Promote under the UAT environment. This will promote the build from UAT to Production. 

Whenever you deploy to Production you need to make sure to follow the Runbook specified in the sprint.

### Release Scripts
Under /releases you will find the sprint related scripts that you need to execute when deploying to environments which might include SQL scripts, Shell scripts etc.

The scripts are grouped by sprints under /releases/<sprint>.

Under /releases/general you will find the general scripts (aka utility scripts) which are not sprint specific and which are *not* to be executed with every sprint.

### Database Non Breaking Changes

If you know your non-breaking migration doesn’t have a problem then applying the migration while the old application is running provides continuous service to your users. 
Our migrations are stored in the Entity Framework migrations project: Tigerspike.Solv.Infra.Data

### Database Breaking Changes

Deploying breaking changes for the database would require one or more intermediate deployments. You would be looking at a "five stage app update". Read more about it [here](https://www.thereformedprogrammer.net/handling-entity-framework-core-database-migrations-in-production-part-2/)

## Built Using
- ASP.NET Core 5.0 (with .NET Core 5.0)
- ASP.NET WebApi Core
- Auth0 for Authentication
- Entity Framework Core 3.1
- ServiceStack PocoDynamo
- .NET Core Native DI
- AutoMapper
- FluentValidator
- MediatR
- Swagger UI
- xUnit for testing
- MassTransit

## Architecture:
- Full architecture with responsibility separation concerns, SOLID and Clean Code
- Domain Driven Design (Layers and Domain Model Pattern)
- Domain Events
- Domain Notification
- CQRS (Imediate Consistency)
- Event Sourcing
- Unit of Work
- Repository and Generic Repository

## Active Contributors
- George Saadeh
- Lukasz Krzykowski
- Subhankar Biswas
- Ajay Abraham

## Previous Contributors
- Nour Sabouny
- Andy Woollard
- Aaron Hudson

## Acknowledgements
- [Dotnet Architecture Guidelines](https://github.com/dotnet-architecture/eShopOnContainers)
- [Equinox Project](https://github.com/EduardoPires/EquinoxProject)