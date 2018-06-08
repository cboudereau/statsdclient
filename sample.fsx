#load "StatsdClient.fs"

let client = StatsdClient.Udp.client "127.0.0.1" 61914us
let send = StatsdClient.Udp.sender client |> StatsdClient.send

send (StatsdClient.counter "hello" 1) |> Async.RunSynchronously

//Send multiple stats
let m1 = StatsdClient.counter "hello" 1
let m2 = StatsdClient.counter "world" 1

open StatsdClient.Operators

send (m1 + m2) |> Async.RunSynchronously
