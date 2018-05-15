# How to use ManicTime REST API

## Headers
With all requests include [Accept] header with value "application/vnd.manictime.v2+json".
<pre><code>Accept: application/vnd.manictime.v2+json</code></pre>

## Parameters
fromTime and toTime parameters will accept ISO 8601 format
- example for date: 2018-05-15, 
- example for time: 2018-05-15T06:02:06+00:00

## Authentication
ManicTime Server can authenticate users in two ways, with Windows or ManicTime authentication.

### Windows Authentication (NTLM)
Authenticate using Windows account credentials.
<pre><code>curl --ntlm --user : --header "Accept: application/vnd.manictime.v2+json" http://localhost:8080/</code></pre>

This will use currently logged in user. If you want to use specific user, then for --user parameter use `username:password` instead of just colon (:).

### ManicTime Authentication
Authentication uses OAuth 2.0 Resource Owner Password Credentials Grant (https://tools.ietf.org/html/rfc6749#section-4.3).
To authenticate, you must first exchange user credentials (email, password) for access token (access tokens never expire).

#### Get access token
POST http://localhost:8080/api/token

Headers
<pre><code>Content-Type: application/x-www-form-urlencoded;charset=utf-8
Content: grant_type=password&username={username}&password={password}</code></pre>

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Content-Type: application/x-www-form-urlencoded;charset=utf-8" -X POST --data "grant_type=password&username=username&password=password" http://localhost:8080/api/token`

Sample response
<pre><code>200 OK
{
   "token":"a4cee5d8b9b54eefa529e1f288d41f31"
}
</code></pre>

From now on, you must use access token with every request. You need to include Authorization header like this:
<pre><code>Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31</code></pre>

For example:

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" http://localhost:8080/`
## Timelines
To get a list of timelines visible to authenticated user make a call to /api/timelines. (administrators will see all the timelines, regular users will only see their own)

GET /api/timelines

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" http://localhost:8080/api/timelines`


<pre><code>Sample response
200 OK
{
   "timelines":[
      {
         "timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
         "owner":{
            "userId":2,
            "username":"username",
            "displayName":"User"
         },
         "timelineType":{
            "typeName":"ManicTime/ComputerUsage",
            "genericTypeName":"ManicTime/Generic/Group"
         },
         "clientEnvironment":{
            "databaseId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba",
            "deviceName":"MACHINENAME"
         },
         "timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
         "updateState":{
            "timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
            "timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
            "updateTimestamp":131384,
            "updateUtcTime":"2018-05-08T15:35:57.6765993+00:00",
            "isUpdating":false
         },
         "changeTrackingStartTimestamp":1,
         "links":[
            {
               "rel":"self",
               "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2"
            },
            {
               "rel":"manictime/activities",
               "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities"
            },
            {
               "rel":"manictime/activityupdates",
               "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activityupdates"
            }
         ]
      }
   ]
}
</code></pre>

### Get more information about a single timeline
GET /api/timelines/{timelineId}

- {timelineId} timeline identifier

Retrieves single timeline
`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2`

Sample response
<pre><code>
200 OK
{"timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2","owner":{"userId":2,"username":"username","displayName":"User"},"timelineType":{"typeName":"ManicTime/ComputerUsage","genericTypeName":"ManicTime/Generic/Group"},"clientEnvironment":{"databaseId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba","deviceName":"MACHINENAME"},"timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2","updateState":{"timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2","timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2","updateTimestamp":131405,"updateUtcTime":"2018-05-09T06:13:27.0651556+00:00","isUpdating":false},"changeTrackingStartTimestamp":1,"links":[{"rel":"self","href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2"},{"rel":"manictime/activities","href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities"},{"rel":"manictime/activityupdates","href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activityupdates"}]}
</code></pre>

### Get activities
GET /api/timelines/{timelineId}/activities?fromTime={fromTime}&toTime={toTime}

- {timelineId} timeline identifier

- {fromTime} lower time range boundary (for date use: 2018-05-15, for time use: 2018-05-15T06:02:06+00:00 (ISO 8601 format))

- {toTime} upper time range boundary (for date use: 2018-05-15, for time use: 2018-05-15T06:02:06+00:00 (ISO 8601 format))

Gets timeline activities within specified time range 

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" "http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities?fromTime=2018-05-08&toTime=2018-05-09"`

Sample response
<pre><code>200 OK
{
   "timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
   "owner":{
      "userId":2,
      "username":"username",
      "displayName":"User"
   },
   "timelineType":{
      "typeName":"ManicTime/ComputerUsage",
      "genericTypeName":"ManicTime/Generic/Group"
   },
   "clientEnvironment":{
      "databaseId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba",
      "deviceName":"MACHINENAME"
   },
   "timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
   "updateState":{
      "timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
      "timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
      "updateTimestamp":131402,
      "updateUtcTime":"2018-05-08T15:53:51.9312268+00:00",
      "isUpdating":false
   },
   "changeTrackingStartTimestamp":1,
   "activities":[
      {
         "activityId":"6586",
         "displayName":"Active",
         "groupId":"1",
         "isActive":true,
         "startTime":"2018-05-08T12:59:17+02:00",
         "endTime":"2018-05-08T13:01:28+02:00"
      }
   ],
   "groups":[
      {
         "groupId":"1",
         "displayName":"Active",
         "color":"58C10C",
         "skipColor":false,
         "folderId":"6588",
         "textData":"<ComputerUsageGroupTextData><DeleteWhenNotUsed>false</DeleteWhenNotUsed></ComputerUsageGroupTextData>",
         "displayKey":"ManicTime/Active"
      }
   ],
   "groupLists":[

   ],
   "folders":[
      {
         "folderId":"6588",
         "displayName":"ActiveFolder",
         "color":"3A0EC5",
         "displayKey":"ActiveFolder"
      }
   ],
   "links":[
      {
         "rel":"self",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2"
      },
      {
         "rel":"manictime/activities",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities"
      },
      {
         "rel":"manictime/activityupdates",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activityupdates"
      },
      {
         "rel":"manictime/updatedactivities",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities?fromTime=2018-05-08T00:00:00&toTime=2018-05-09T00:00:00&updatedAfterTimestamp=131402,1991595575"
      }
   ]
}
</code></pre>

### Get updated activities
GET /api/timelines/{timelineId}/activities?fromTime={fromTime}&toTime={toTime}&updatedAfterTimestamp={updateTimestamp}

- {updateTimestamp} timestamp of last timeline update

Gets timeline activity changes after specified timestamp. This should only be used as successor call to /api/timelines/{timelineId}/activities (the complete url including parameters is provided by 'links' resource in a call to /api/timelines/{timelineId}/activities).

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" "http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities?fromTime=2018-05-08&toTime=2018-05-09&updatedAfterTimestamp=131402,1991595575"`


Sample response
<pre><code>200 OK
{
   "timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
   "owner":{
      "userId":2,
      "username":"username",
      "displayName":"User"
   },
   "timelineType":{
      "typeName":"ManicTime/ComputerUsage",
      "genericTypeName":"ManicTime/Generic/Group"
   },
   "clientEnvironment":{
      "databaseId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba",
      "deviceName":"MACHINENAME"
   },
   "timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
   "updateState":{
      "timelineId":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
      "timelineKey":"aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2",
      "updateTimestamp":131403,
      "updateUtcTime":"2018-05-08T15:57:54.1752783+00:00",
      "isUpdating":false
   },
   "changeTrackingStartTimestamp":1,
   "updatedAfterTimestamp":131402,
   "activities":[

   ],
   "deletedActivityIds":[
      "6586"
   ],
   "groups":[

   ],
   "deletedGroupIds":[

   ],
   "groupLists":[

   ],
   "deletedGroupListIds":[

   ],
   "folders":[

   ],
   "deletedFolderIds":[

   ],
   "links":[
      {
         "rel":"self",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2"
      },
      {
         "rel":"manictime/activities",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities"
      },
      {
         "rel":"manictime/activityupdates",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activityupdates"
      },
      {
         "rel":"manictime/updatedactivities",
         "href":"http://localhost:8080/api/timelines/aa731e7f-f4c9-4121-aa96-c9f8f5cfdfba-2/activities?fromTime=2018-05-08T00:00:00&toTime=2018-05-09T00:00:00&updatedAfterTimestamp=131403,591248858"
      }
   ]
}
</code></pre>

## Tag Combinations

### Get tag combinations

GET api/tagcombinationlist

Gets list of tags defined by server.

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" "http://localhost:8080/api/tagcombinationlist"`

Sample response
<pre><code>200 OK
{
   "tagCombinations":[
      "Project, Testing",
      "Project, Programming"
   ],
   "links":[
      {
         "rel":"self",
         "href":"http://localhost:8080/api/tagcombinationlist"
      }
   ]
}
</code></pre>

### Update tag combinations
POST http://localhost:8080/api/tagcombinationlist

[Content] Json serialized TagCombinations resource

Overwrites the list of tags defined on server (only available when server settings in "Administration, Tags" specifies "Service" as tag combination source type)

`curl --header "Accept: application/vnd.manictime.v2+json" --header "Authorization: Bearer a4cee5d8b9b54eefa529e1f288d41f31" --header "Content-Type: application/vnd.manictime.v2+json;charset=utf-8" -X POST --data "{\"TagCombinations\":[\"project,\",\",task1\",\",task2\"]}" "http://localhost:8080/api/tagcombinationlist"`

Sample response
<pre><code>200 OK
{
   "tagCombinations":[
      "project, task1",
      "project, task2"
   ],
   "links":[
      {
         "rel":"self",
         "href":"http://localhost:8080/api/tagcombinationlist"
      }
   ]
}
</code></pre>
