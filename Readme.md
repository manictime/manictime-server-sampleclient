This is a sample client for ManicTimeServer. It shows how you can connect to the server, as well as how you can send and receive data.

How to run
---------------------
To run the sample you need Visual studio 2010 or later, with Nuget installed.

How to use
---------------------
In server url enter the url of ManicTimeServer. By default it runs on port 8080, so the full url is
http://<machineName_or_IP>:8080/

In Settings you can choose Message format (XML or JSON).
In Settings you can also choose a user. ManicTimeServer uses integrated security, so usually the logged in user will have access to the server and you can just leave it empty. If currently logged in user doesn't have access to the machine where the server is located, you can specify a user manually.

####The sample has these commands:
1. Home - ManicTimeServer is REST based, so a request for the Home will return available links you can use. 
2. Get timelines - returns a list of timelines which current user has access to
3. Get activities - returns a list of activities for specified timeline and date range
4. Get updated activities - When you request GetActivities, server with a list of activities also returns an updatedactivities URL. A call to this URL will only return changes from the last call. 
So you could run GetActivities first, then GetUpdatedActivities from then on. This is usually only useful for updating the current day where the data changes constantly. 
You could also call GetActivities to get the full list, but it will generate more traffic.
5. Get tag combinations - Get a list of server tag combinations
6. Update tag combinations - This will replace all tag combinations on the server with the set you specify.

Sample client consists of two project, Server.SampleClient and Server.SampleClient.UI.
You can include Server.SampleClient in your projects and just call the exposed methods.