[<AutoOpen>]
module Extensions

open Elmish

/// Describes the state of an asynchronous operation that generates `'t` when it finishes
type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't

/// Describes the status of message (msg)
type MsgStatus<'t> =
    | Dispatched
    | Handled of 't     

[<RequireQualifiedAccess>]
module Cmd =
   
    let fromAsync (operation: Async<'msg>) : Cmd<'msg> =
        let delayedCmd (dispatch: 'msg -> unit) : unit =
            let delayedDispatch = async {
                let! msg = operation
                dispatch msg
            }

            Async.StartImmediate delayedDispatch

        Cmd.ofSub delayedCmd

[<RequireQualifiedAccess>]
module Async =

    let singleton x = async {
        return x
    }

    let map f (computation: Async<'t>) = async {
        let! x = computation
        return f x
    }

    let bind f (computation: Async<'t>) = async {
        let! x = computation
        return! f x
    }

[<RequireQualifiedAccess>]
module Deferred =
    
    /// Returns whether the `Deferred<'T>` value has been resolved or not.
    let resolved = function
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved _ -> true
    
    /// Returns whether the `Deferred<'T>` value is in progress or not.
    let inProgress = function
        | HasNotStartedYet -> false
        | InProgress -> true
        | Resolved _ -> false
    
    /// Verifies that a `Deferred<'T>` value is resolved and the resolved data satisfies a given requirement.
    let exists (predicate: 'T -> bool) = function
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved value -> predicate value
        
    /// Transforms the underlying value of the input deferred value when it exists from type to another
    let map (transform: 'T -> 'U) (deferred: Deferred<'T>) : Deferred<'U> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> Resolved (transform value)
    
    /// Like `map` but instead of transforming just the value into another type in the `Resolved` case, it will transform the value into potentially a different case of the the `Deferred<'T>` type.
    let bind (transform: 'T -> Deferred<'U>) (deferred: Deferred<'T>) : Deferred<'U> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> transform value