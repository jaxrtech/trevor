[<AutoOpen>]
module Trevor.Core

open System.Net
open System.Threading.Tasks

/// Starts a continuous loop using a responder and custom state to reply to incoming requests
module Listener =
    /// An asynchronous function that responds to a request with custom state
    type AsyncResponder<'a> = HttpListenerRequest -> HttpListenerResponse -> 'a -> Async<unit>

    /// Represents an HTTP listener with custom state which is passed to the responder
    type T<'a> = {Listener: HttpListener; State: 'a}

    /// Returns a new listener that can then be later ran and listened on
    let create (state:'a) = 
        let listener = new HttpListener()
        listener.Prefixes.Add "http://*:8080/"
        {Listener=listener; State=state}

    /// Starts a new asynchronous listening loop waiting to respond to requests using the provided responder and custom state
    let listen (responder:AsyncResponder<'a>) {Listener=listener; State=state} =
        listener.Start()
        async { while true do
                    // TODO: some event logging or something here would be nice to see the requests as the come in
                    //       ex: INFO [127.0.0.1:252341] GET "/"
                    let! context = Async.AwaitTask <| listener.GetContextAsync()
                    Async.Start(state |> responder context.Request context.Response) }
        |> Async.Start
        // TODO: need to add some way to safely cancel the task loop and stop the server