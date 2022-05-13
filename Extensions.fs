[<AutoOpen>]
module Extensions

open Elmish

type AsyncOperationStatus<'a> =
    | Started
    | Finished of 'a

type Deferred<'a> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 'a

[<RequireQualifiedAccess>]
module Deferred =
    
    let map (transform: 'a -> 'b) (deferred: Deferred<'a>) : Deferred<'b> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> Resolved (transform value)
    
    let bind (transform: 'a -> Deferred<'b>) (deferred: Deferred<'a>) : Deferred<'b> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> transform value

    let iter (action: 'a -> unit) (deferred: Deferred<'a>) : unit =
        match deferred with
        | HasNotStartedYet -> ()
        | InProgress -> ()
        | Resolved value -> action value

    let resolved deferred =
        match deferred with
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved _ -> true
    
    let exists (predicate: 'a -> bool) deferred = 
        match deferred with
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved value -> predicate value 

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
module Config =
    open Fable.Core

    [<Emit("process.env.API_BASE_ADDRESS ? process.env.API_BASE_ADDRESS : ''")>]
    let GetApiBaseAddress () : string = jsNative

[<RequireQualifiedAccess>]
module Image =
    open Fable.Core.JsInterop

    let inline load (relativePath: string) : string = importDefault relativePath

[<RequireQualifiedAccess>]
module Window =
    open Fable.Core

    [<Emit("encodeURIComponent($0)")>]
    let encodeURIComponent (value: string) : string = jsNative

[<RequireQualifiedAccess>]
module Strings =

    let toPascalCase (value: string) =
        let letter = value.[0].ToString().ToUpper()
        let pascalCased = letter + value.Substring(1)
        pascalCased        