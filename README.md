# Meloht.API.Gateway

* **Load Balancing**
* **Rate Limiting**
* **Health Check**

## Performance testing
Ten thousand concurrent request test
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
<img width="1545" height="356" alt="image" src="https://github.com/user-attachments/assets/1031b5f7-607d-4165-b4c2-56a3234d1875" />
<img width="1541" height="334" alt="image" src="https://github.com/user-attachments/assets/7a6014f8-facf-4c0f-b02d-4f5394dfff10" />

<img width="1532" height="1132" alt="image" src="https://github.com/user-attachments/assets/281e60f5-b06f-43b5-8633-9916cb3d56a9" />

