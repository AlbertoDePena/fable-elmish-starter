[<AutoOpen>]
module Extensions

open Elmish

/// Describes the state of an asynchronous operation that generates `'t` when it finishes
type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't

/// Describes the status of message (msg)
type AsyncOperationStatus<'t> =
    | Started
    | Finished of 't     

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

    let inline singleton value = async.Return value

    let inline bind f x = async.Bind(x, f)

    let inline map f x = x |> bind (f >> singleton)

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

    /// Applies the given function when the type is in the `Resolved` case
    let iter (action: 'T -> unit) (deferred: Deferred<'T>) : unit =
        match deferred with
        | HasNotStartedYet -> ()
        | InProgress -> ()
        | Resolved value -> (action value)

[<RequireQualifiedAccess>]
module Config =
    open Fable.Core

    [<Emit("process.env.API_BASE_ADDRESS ? process.env.API_BASE_ADDRESS : ''")>]
    let GetApiBaseAddress () : string = jsNative

[<RequireQualifiedAccess>]
module Image =
    open Fable.Core.JsInterop

    let inline load (relativePath: string) : string = importDefault relativePath

[<RequireQualifiedAccess>]
module Strings =

    let toPascalCase (value: string) =
        let letter = value.[0].ToString().ToUpper()
        let pascalCased = letter + value.Substring(1)
        pascalCased        