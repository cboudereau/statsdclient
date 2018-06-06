module StatsdClient

open System.Net.Sockets

type Metric = private Metric of string

let counter name value = sprintf "%s:%i|c" name value |> Metric
let gauge name value = sprintf "%s:%i|g" name value |> Metric
let timer name value = sprintf "%s:%i|ms" name value |> Metric

let empty = Metric ""

let private (|Empty|_|) (Metric x) = if System.String.IsNullOrWhiteSpace(x) then None else Some ()

let (+) x y = 
    match x, y with
    | Empty, x' | x', Empty -> x'
    | Metric x', Metric y' -> sprintf "%s\n%s" x' y' |> Metric

let udp hostname (port:uint16) = new System.Net.Sockets.UdpClient(hostname, int port)

type [<Struct>] Sender = private Sender of (Metric -> Async<unit>)

let sender (client:UdpClient) = 
    fun (Metric m) -> 
        let bytes = System.Text.Encoding.ASCII.GetBytes(m:string)
        client.SendAsync(bytes, bytes.Length)
        |> Async.AwaitTask
        |> Async.Ignore
    |> Sender

let send (Sender f) m = f m