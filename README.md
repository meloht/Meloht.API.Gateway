# Meloht.API.Gateway

* **Load Balancing**
* **Rate Limiting**
* **Health Check**

## Performance testing
Ten thousand concurrent request test

## Performance Test PC 
### JMeter & Service Endpoint 1
|Hardware|Summary|
| ------------- | ------------- | 
|Windows |Windows 10 Pro 22H2|
|CPU| AMD Ryzen 7 5800X 8-Core Processor 3.8GHz|
|RAM| DDR4 3200 MHz 32GB|
|Storage| SSD 2TB|

### Service Endpoint 2

|Hardware|Summary|
| ------------- | ------------- | 
|Windows |Windows 10 Pro 22H2|
|CPU| i7-10510U 2.3GHz|
|RAM| DDR4 2667 MHz 16GB|
|Storage| SSD 2TB|

### Service Endpoint 3

|Hardware|Summary|
| ------------- | ------------- | 
|Windows |Windows 10 Pro 22H2|
|CPU| i7-4500U 2.4GHz|
|RAM| DDR3 1600 MHz 8GB|
|Storage| HDD 1TB|

```json
"Gateway": {
    "RequestQueuePoolSize": 1000,
    "RequestTimeoutSeconds": 120,
    "LoadBalancingPolicy": "RoundRobin",
    "HealthCheck": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5
    },
    "TargetServers": [
      {
        "Id": 1,
        "UniqueName": "test01",
        "Host": "192.168.3.7",
        "Port": 8081,
        "Weight": 1
      },
      {
        "Id": 2,
        "UniqueName": "test02",
        "Host": "192.168.3.26",
        "Port": 8081,
        "Weight": 1
      },
      {
        "Id": 3,
        "UniqueName": "test03",
        "Host": "192.168.3.7",
        "Port": 8082,
        "Weight": 1
      },
      {
        "Id": 4,
        "UniqueName": "test04",
        "Host": "192.168.3.7",
        "Port": 8083,
        "Weight": 1
      },
      {
        "Id": 5,
        "UniqueName": "test05",
        "Host": "192.168.3.26",
        "Port": 8082,
        "Weight": 1
      },
      {
        "Id": 6,
        "UniqueName": "test06",
        "Host": "192.168.3.7",
        "Port": 8084,
        "Weight": 1
      },
      {
        "Id": 7,
        "UniqueName": "test07",
        "Host": "192.168.3.7",
        "Port": 8085,
        "Weight": 1
      },
       {
        "Id": 8,
        "UniqueName": "test08",
        "Host": "localhost",
        "Port": 8081,
        "Weight": 1
      }
    ]
  }
```
<img width="759" height="222" alt="image" src="https://github.com/user-attachments/assets/4d874d0f-2929-4dca-b9c6-818a52bed46b" />

<img width="1541" height="334" alt="image" src="https://github.com/user-attachments/assets/7a6014f8-facf-4c0f-b02d-4f5394dfff10" />

<img width="1532" height="1132" alt="image" src="https://github.com/user-attachments/assets/281e60f5-b06f-43b5-8633-9916cb3d56a9" />

<img width="1576" height="358" alt="image" src="https://github.com/user-attachments/assets/a18e8860-9ac9-44da-b104-01cb29313bb5" />
<img width="1579" height="1130" alt="image" src="https://github.com/user-attachments/assets/93231f6d-f974-44b2-9af9-c4c7cfc26792" />



