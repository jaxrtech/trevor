module Trevor.Helpers

open System.Threading.Tasks

// A normal non-generic Task (not a Task<T>) is an IAsyncResult which can be awaited
let awaitTask (task:Task) = task |> Async.AwaitIAsyncResult |> Async.Ignore