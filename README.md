![Build Status](https://ci.appveyor.com/api/projects/status/github/RagtimeWilly/Astro.CQRS?branch=master&svg=true) [![NuGet](https://img.shields.io/nuget/v/Astro.CQRS.svg)](https://www.nuget.org/packages/Astro.CQRS/)

Collection of libraries for implementing the CQRS pattern on the Azure platform

## Core library

### Installing from NuGet

`PM> Install-Package Astro.CQRS`

Library includes:

* Event sourcing base classes (for aggregates, events, event stores)
* Messaging base classes (publishers, subscribers, event handlers, dispatchers)

## Event Store

### Installing from NuGet

`PM> Install-Package Astro.CQRS.EventStore`

Library includes:

* Implementation of an event store using Azure Table Storage

## Messaging

### Installing from NuGet

`PM> Install-Package Astro.CQRS.Messaging`

Library includes:

* Implementation of command and event publishers using Azure Service Bus
* Implementation of command and event subscribers using Azure Service Bus

## Getting help

If you have any problems or suggestions please create an [issue](https://github.com/RagtimeWilly/Astro.CQRS/issues) or a [pull request](https://github.com/RagtimeWilly/Astro.CQRS/pulls)