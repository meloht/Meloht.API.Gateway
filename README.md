# Meloht.API.Gateway

* **Load Balancing**
* **Rate Limiting**
* **Health Check**

## Performance testing

```json
  "Gateway": {
    "PoolSize": 1000,
    "HttpRequestTimeout": 120,
    "LoadBalancingPolicy": "RoundRobin",
    "DatabaseAutoUpdate": {
      "IntervalSeconds": 120,
      "DatabaseTimeoutSeconds": 5,
      "ConnectionString": null
    },
    "HealthCheck": {
      "IntervalSeconds": 10,
      "RequestTimeoutSeconds": 5
    },
    "TargetServers": [
      {
        "Id": 2,
        "UniqueName": "test02",
        "Host": "192.168.3.7",
        "Port": 8081,
        "Weight": 1
      },
      {
        "Id": 3,
        "UniqueName": "test03",
        "Host": "192.168.3.26",
        "Port": 8081,
        "Weight": 1
      },
      {
        "Id": 4,
        "UniqueName": "test04",
        "Host": "192.168.3.7",
        "Port": 8082,
        "Weight": 1
      },
      {
        "Id": 5,
        "UniqueName": "test05",
        "Host": "192.168.3.7",
        "Port": 8083,
        "Weight": 1
      },
      {
        "Id": 6,
        "UniqueName": "test06",
        "Host": "192.168.3.26",
        "Port": 8082,
        "Weight": 1
      },
       {
        "Id": 7,
        "UniqueName": "test07",
        "Host": "192.168.3.7",
        "Port": 8084,
        "Weight": 1
      }
    ]
  },
```
<img width="1545" height="356" alt="image" src="https://github.com/user-attachments/assets/1031b5f7-607d-4165-b4c2-56a3234d1875" />
