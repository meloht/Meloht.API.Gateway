# Meloht.API.Gateway
Fast, flexible, and easy to extend 
## Feature
* **Reverse Proxy**
* **Load Balancing**
* **Rate Limiting**
* **Health Check**
* **Service Discovery**

## Usage
There are three ways to configure the api gateway
- **Use json to configure the target server address**
  - lib `Meloht.API.Gateway.Json`
- **Use database to configure the target server address**
  - MySql database provider lib`Meloht.API.Gateway.MySql`
  - SQL Server database provider lib`Meloht.API.Gateway.SqlServer`
  - PostgreSQL database provider lib`Meloht.API.Gateway.PostgreSQL`
- **Use Service Discovery to configure the target server address**
  - MySql database provider lib`Meloht.API.ServiceDiscovery.Provider.MySql`
  - SQL Server database provider lib`Meloht.API.ServiceDiscovery.Provider.SqlServer`
  - PostgreSQL database provider lib`Meloht.API.ServiceDiscovery.Provider.PostgreSQL`

### Load Balancing
The weight of the target server can be configured
* **FirstAlphabetical** Select the alphabetically first available destination without considering load. This is useful for dual destination fail-over systems.
* **Random** Select a destination randomly.
* **RoundRobin** Select a destination by cycling through them in order.
* **LeastRequests** Select the destination with the least assigned requests. This requires examining all destinations.
* **PowerOfTwoChoices** Select two random destinations and then select the one with the least assigned requests. This avoids the overhead of LeastRequests and the worst case for Random where it selects a busy destination.

### Json Mode
Use appsetting.json to configure the target server address, using the lib Meloht.API.Gateway.Json
<img width="481" height="390" alt="image" src="https://github.com/user-attachments/assets/f359b0ac-8494-4a99-b2a1-5c4066d05b14" />

#### api gateway setting
```csharp
using Meloht.API.Gateway.Json;

 var builder = WebApplication.CreateBuilder(args);
 // Add services to the container.
 builder.Services.AddControllers();
 builder.Services.AddOpenApi();

 builder.Services.AddGatewayService(builder.Configuration);

 var app = builder.Build();
 app.UseHttpsRedirection();
 app.UseAuthorization();

 app.UseGateway();

 app.MapControllers();
 app.Run();
```

appsetting.json
```json
  "Gateway": {
    "LoadBalancingPolicy": "RoundRobin",

    "ReverseProxy": {
      "RequestQueuePoolSize": 1000,
      "RequestTimeoutSeconds": 120
    },
    "HealthCheck": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5
    },
    "ServiceDiscovery": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5,
      "ServiceDiscoveryHost": "https://localhost:5500"
    },
    "TargetServers": [
      {
        "Id": 1,
        "UniqueName": "test01",
        "Host": "localhost:8081",
        "Protocol": "http",
        "Weight": 1
      }
    ]
  },

```
### Database Mode
<img width="485" height="435" alt="image" src="https://github.com/user-attachments/assets/3f344e97-4998-4fa6-81a1-fd6909d96839" />


#### api gateway setting
```csharp
using Meloht.API.Gateway.MySql;

 var builder = WebApplication.CreateBuilder(args);
 // Add services to the container.
 builder.Services.AddControllers();
 builder.Services.AddOpenApi();

 builder.Services.AddGatewayService(builder.Configuration);

 var app = builder.Build();
 app.UseHttpsRedirection();
 app.UseAuthorization();

 app.UseGateway();

 app.MapControllers();
 app.Run();
```

appsettings.json
```json
  "Gateway": {
    "LoadBalancingPolicy": "RoundRobin",
    "ReverseProxy": {
      "RequestQueuePoolSize": 1000,
      "RequestTimeoutSeconds": 120
    },
    "DatabaseAutoUpdate": {
      "IntervalSeconds": 120,
      "DatabaseTimeoutSeconds": 5,
      "ConnectionString": "Server=127.0.0.1;Port=3306;Database=testdb;User ID=root;Password=123456;"
    },
    "HealthCheck": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5
    },
  }

```

### Service Discovery Mode

<img width="642" height="485" alt="image" src="https://github.com/user-attachments/assets/5607c6e6-333c-45d0-84d1-fbecaa72428a" />


#### api gateway setting
```csharp
using Meloht.API.Gateway.ServiceDiscovery;

 var builder = WebApplication.CreateBuilder(args);
 // Add services to the container.
 builder.Services.AddControllers();
 builder.Services.AddOpenApi();

 builder.Services.AddGatewayService(builder.Configuration);

 var app = builder.Build();
 app.UseHttpsRedirection();
 app.UseAuthorization();

 app.UseGateway();

 app.MapControllers();
 app.Run();
```

appsettings.json
```json
  "Gateway": {
    "LoadBalancingPolicy": "RoundRobin",
    "ReverseProxy": {
      "RequestQueuePoolSize": 1000,
      "RequestTimeoutSeconds": 120
    }
  }

```
#### target server setting
```csharp
using Meloht.API.Gateway.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddClientServiceDiscovery(builder.Configuration);
var app = builder.Build();
app.UseGatewayClient();
```
appsettings.json
```json
  "GatewayClient": {
    "ClientProtocol": "http",
    "ClientUniqueName": "test01",
    "ClientWeight": 1,
    "ServiceDiscoveryProtocol": "http",
    "ServiceDiscoveryHost": "localhost:5500",
    "RequestTimeoutSeconds": 5
  }
```

#### Service Discovery setting
```csharp
using Meloht.API.ServiceDiscovery.Provider.MySql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServiceDiscovery(builder.Configuration);
var app = builder.Build();

app.UseServiceDiscovery();
```
appsettings.json
```json
  "ServiceDiscovery": {
    "DatabaseConfig": {
      "IntervalSeconds": 60,
      "DatabaseTimeoutSeconds": 5,
      "ConnectionString": null
    },
    "HealthCheck": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5
    }
  }
```

## Performance testing
Ten thousand concurrent request test

## Performance Test PC 
|PC|System|CPU|RAM|Storage|Summary|
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | 
| PC1 | Windows 10 Pro 22H2 | AMD Ryzen 7 5800X 8-Core Processor 3.8GHz | DDR4 3200 MHz 32GB | SSD 2TB | JMeter & Service Endpoint 1 | 
| PC2 | Windows 10 Pro 22H2 | i7-10510U 2.3GHz | DDR4 2667 MHz 16GB |  SSD 2TB| Service Endpoint 2 | 
| PC3 | Windows 10 Pro 22H2 |  i7-4500U 2.4GHz | DDR3 1600 MHz 8GB | HDD 1TB | Service Endpoint 3 | 


```json
"Gateway": {
    "LoadBalancingPolicy": "RoundRobin",
    "ReverseProxy": {
      "RequestQueuePoolSize": 1000,
      "RequestTimeoutSeconds": 120
    },
    "HealthCheck": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5
    },
    "TargetServers": [
      {
        "Id": 1,
        "UniqueName": "test01",
        "Host": "192.168.3.7:8081",
        "Protocol": "http",
        "Weight": 1
      },
      {
        "Id": 2,
        "UniqueName": "test02",
        "Host": "192.168.3.26:8081",
        "Protocol": "http",
        "Weight": 1
      },
      {
        "Id": 3,
        "UniqueName": "test03",
        "Host": "192.168.3.7:8082",
        "Protocol": "http",
        "Weight": 1
      },
      {
        "Id": 4,
        "UniqueName": "test04",
        "Host": "192.168.3.7:8083",
        "Protocol": "http",
        "Weight": 1
      },
      {
        "Id": 5,
        "UniqueName": "test05",
        "Host": "192.168.3.26:8082",
        "Protocol": "http",
        "Weight": 1
      },
      {
        "Id": 6,
        "UniqueName": "test06",
        "Host": "192.168.3.7:8084",
        "Protocol": "http",
        "Weight": 1
      },
      {
        "Id": 7,
        "UniqueName": "test07",
        "Host": "192.168.3.7:8085",
        "Protocol": "http",
        "Weight": 1
      },
       {
        "Id": 8,
        "UniqueName": "test08",
        "Host": "localhost:8081",
        "Protocol": "http",
        "Weight": 1
      }
    ]
  }
```

<img width="1541" height="334" alt="image" src="https://github.com/user-attachments/assets/7a6014f8-facf-4c0f-b02d-4f5394dfff10" />

<img width="1532" height="1132" alt="image" src="https://github.com/user-attachments/assets/281e60f5-b06f-43b5-8633-9916cb3d56a9" />

<img width="1576" height="358" alt="image" src="https://github.com/user-attachments/assets/a18e8860-9ac9-44da-b104-01cb29313bb5" />
<img width="1579" height="1130" alt="image" src="https://github.com/user-attachments/assets/93231f6d-f974-44b2-9af9-c4c7cfc26792" />



