# Client-Server Network

A client-server network using C# and Sockets "location" being the client and "locationserver" being the server. They use http 0.9, 1.0, 1.1 and Whois protocol to ask and receive data from the server. The client can also update data on the server by sending an update request.

The data the server uses is stored in a .txt file. There is also a log file that is added to and updated every time a request is sent.

As an additional part to this, there is also a pong game that sends and receives high scores to the server which are also stored in a .txt file.

Multiplayer over the server was planned but has not yet been implemented.
