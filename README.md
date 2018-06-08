# statsdclient

F# statsD client.

## Install
https://fsprojects.github.io/Paket/github-dependencies.html#Referencing-a-single-file

In your paket.dependencies just add 
```
github cboudereau/statsdclient StatsdClient.fs
```

Add this line into your paket.refereneces:
```
File: StatsdClient.fs
```

Configure your .gitignore by adding this line :
```
paket-files/
```

## Sample
```fsharp
let client = StatsdClient.Udp.client "127.0.0.1" 61914us
let send = StatsdClient.Udp.sender client |> StatsdClient.send

send (StatsdClient.counter "hello" 1) |> Async.RunSynchronously

//Send multiple stats
let m1 = StatsdClient.counter "hello" 1
let m2 = StatsdClient.counter "world" 1

open StatsdClient.Operators

send (m1 + m2) |> Async.RunSynchronously
```
