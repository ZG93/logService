# logService
# 日志系统接口


## 概览
日志服务API*


## 版本信息
| 版本号  | 作者 | 日期       | 更新内容               |
| :------ | :--- | :--------- | :--------------------- |
| v.0.0.1 | gz   | 2020/07/30 | 第一版                 |

## 1. 队列

### 1.1 日志队列

#### 队列名称
```
log_service_msg_queue
```
#### 队列内容

```json
{
	"type": 0,
	"name": "系统后台",
	"service_name": "adminService",
	"user": "admin",
	"order_id": "D623518t572198t578321",
	"msg": "提取10BCB",
	"memo": "券商提币",
	"operation_time": "2019-12-22 12:00:00"
}
```


| 属性           | 类型     | 必选 | 说明            |
| -------------- | -------- | ---- | --------------- |
| type           | int      | 是   | 日志类型,分配的 |
| name           | string   | 是   | 日志类型名称    |
| service_name   | string   | 否   | 自身服务名称    |
| user           | string   | 否   | 用户名称        |
| order_id       | string   | 否   | 订单            |
| msg            | string   | 否   | 操作日志        |
| memo           | string   | 否   | 备注            |
| operation_time | DateTime | 是   | 操作时间        |


## 2. 日志服务api

### 2.1 日志查询

#### 接口路径
```
Get logapi/search?type=1&orderid=11&user=123&stime=20200107010000&etime=20200907180000&page=1&count=1
```
#### 请求参数


| 属性     | 类型   | 必选 | 说明                                |
| -------- | ------ | ---- | ----------------------------------- |
| type     | int    | 是   | 日志类型,分配的                     |
| user     | string | 否   | 用户名称                            |
| order_id | string | 否   | 订单                                |
| stime    | string | 否   | 查询起始时间， 格式(yyyyMMddHHmmss) |
| etime    | string | 否   | 查询结束时间， 格式(yyyyMMddHHmmss) |
| page     | int    | 否   | 查询页数                            |
| count    | int    | 否   | 查询每页个数                        |


##### 返回参数


| 属性           | 类型     | 说明            |
| -------------- | -------- | --------------- |
| page           | int      | 当前第几页      |
| count          | int      | 每页多少条      |
| totalpage      | int      | 总共多少页      |
| total          | int      | 总条数          |
| type           | int      | 日志类型,分配的 |
| name           | string   | 日志类型名称    |
| service_name   | string   | 自身服务名称    |
| user           | string   | 用户名称        |
| order_id       | string   | 订单            |
| msg            | string   | 操作日志        |
| memo           | string   | 备注            |
| operation_time | DateTime | 操作时间        |


##### HTTP响应示例

```json
{
    "code":0,
    "message":"success",
    "data":{
        "pageInfo":{
            "page":1,
            "count":1,
            "totalpage":2,
            "total":2
        },
        "records":[
            {
                "type":1,
                "name":"test",
                "serviceName":"test",
                "user":"1",
                "orderId":"1",
                "msg":"1",
                "memo":"1",
                "operationTime":"2020-07-21 17:55:17"
            }]
    }
}

```

### 2.2 查询队列

#### 接口路径
```
Get logapi/queue/list
```

##### 返回参数


| 属性           | 类型     | 说明            |
| -------------- | -------- | --------------- |
| data           | list     | 队列名称        |


##### HTTP响应示例

```json
{
  "code": 0, 
  "message": "success", 
  "data": [
    "log_service_msg_queue", 
    "log_service_msg_queue_1"
  ]
}

```

### 2.3 注册队列

#### 接口路径
```
Get logapi/register?queuename=log_service_msg_queue_1
```
#### 请求参数


| 属性      | 类型   | 必选 | 说明     |
| --------- | ------ | ---- | -------- |
| queuename | string | 是   | 队列名称 |


##### HTTP响应示例

```json
{
  "code": 0, 
  "message": "success", 
  "data": "注册成功"
}

```
