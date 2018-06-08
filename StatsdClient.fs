[<RequireQualifiedAccess>]
module StatsdClient

open System.Net.Sockets

type Metric = private Metric of string

let counter name value = sprintf "%s:%i|c" name value |> Metric
let gauge name value = sprintf "%s:%i|g" name value |> Metric
let timer name (value:System.TimeSpan) = sprintf "%s:%i|ms" name (int value.TotalMilliseconds) |> Metric

let stopwatch = System.Diagnostics.Stopwatch.StartNew

let empty = Metric ""

let private (|Empty|_|) (Metric x) = if System.String.IsNullOrWhiteSpace(x) then None else Some ()

module Operators = 
    let (+) x y = 
        match x, y with
        | Empty, x' | x', Empty -> x'
        | Metric x', Metric y' -> sprintf "%s\n%s" x' y' |> Metric

type [<Struct>] Sender = private Sender of (Metric -> Async<unit>)

let send (Sender f) m = f m

module Udp = 
    let client hostname (port:uint16) = new System.Net.Sockets.UdpClient(hostname, int port)

    let sender (client:UdpClient) = 
        fun (Metric m) -> 
            let bytes = System.Text.Encoding.ASCII.GetBytes(m:string)
            client.SendAsync(bytes, bytes.Length)
            |> Async.AwaitTask
            |> Async.Ignore
        |> Sender

module Zero = 
    let sender = 
        fun _ -> async.Return () 
        |> Sender