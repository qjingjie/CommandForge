# CommandForge

A .NET 6 WPF application for sending and receiving JSON messages.

[![Deploy CommandForge](https://github.com/qjingjie/CommandForge/actions/workflows/cd.yml/badge.svg)](https://github.com/qjingjie/CommandForge/actions/workflows/cd.yml)

## Available Interfaces

### 1. ZeroMQ Subscriber

1.1 On application startup, the following folder path will be created,

> %AppData%/CommandForge/ZmqSubscribe

<br/>

1.2 To add a predefined list of available topics to subscribe, create a ".json" file based on the format shown in the example below and place it in the "ZmqSubscribe" folder.

```json
{
  "InterfaceName": "InterfaceName",
  "Topics": ["Topic1", "Topic2", "Topic3"]
}
```
