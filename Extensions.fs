[<AutoOpen>]
module Extensions

[<RequireQualifiedAccess>]
type AsyncMsg<'a> =
    | Started
    | Finished of 'a

[<RequireQualifiedAccess>]
type Deferred<'a> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 'a

[<RequireQualifiedAccess>]
module Deferred =

    let map (transform: 'a -> 'b) (deferred: Deferred<'a>) : Deferred<'b> =
        match deferred with
        | Deferred.HasNotStartedYet -> Deferred.HasNotStartedYet
        | Deferred.InProgress -> Deferred.InProgress
        | Deferred.Resolved value -> Deferred.Resolved(transform value)

    let bind (transform: 'a -> Deferred<'b>) (deferred: Deferred<'a>) : Deferred<'b> =
        match deferred with
        | Deferred.HasNotStartedYet -> Deferred.HasNotStartedYet
        | Deferred.InProgress -> Deferred.InProgress
        | Deferred.Resolved value -> transform value

    let iter (action: 'a -> unit) (deferred: Deferred<'a>) : unit =
        match deferred with
        | Deferred.HasNotStartedYet -> ()
        | Deferred.InProgress -> ()
        | Deferred.Resolved value -> action value

    let resolved deferred : bool =
        match deferred with
        | Deferred.HasNotStartedYet -> false
        | Deferred.InProgress -> false
        | Deferred.Resolved _ -> true

    let exists (predicate: 'a -> bool) deferred : bool =
        match deferred with
        | Deferred.HasNotStartedYet -> false
        | Deferred.InProgress -> false
        | Deferred.Resolved value -> predicate value

[<RequireQualifiedAccess>]
module Async =

    let singleton x = 
        async { 
            return x 
        }

    let map f x =
        async {
            let! y = x
            return f y
        }

    let bind f x =
        async {
            let! y = x
            return! f y
        }

[<RequireQualifiedAccess>]
module Config =
    open Fable.Core

    [<Emit("process.env.API_BASE_ADDRESS ? process.env.API_BASE_ADDRESS : ''")>]
    let GetApiBaseAddress () : string = jsNative

[<RequireQualifiedAccess>]
module StaticFile =
    open Fable.Core.JsInterop

    let inline import (relativePath: string) : string = importDefault relativePath

[<RequireQualifiedAccess>]
module Strings =

    let toPascalCase (value: string) =
        let letter = value.[0].ToString().ToUpper()
        let pascalCased = letter + value.Substring(1)
        pascalCased
