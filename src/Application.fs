[<RequireQualifiedAccess>]
module Application

open Elmish

open Feliz
open Extensions

[<RequireQualifiedAccess>]
type Page =
    | Home
    | Blog of blogId: int
    | NotFound

type State =
    { Count: int
      RandomDeferred: Deferred<double>
      CurrentPage: Page
      CurrentRoute: Router.Route option }

type Msg =
    | Increment
    | Decrement
    | GenerateRandomNumberAsync of AsyncMsg<double>

let random = System.Random()

let updateRoute (route: Router.Route option) state =
    let state = { state with CurrentRoute = route }

    match state.CurrentRoute with
    | None ->
        { state with
            CurrentPage = Page.NotFound },
        Cmd.none

    | Some Router.Route.Home -> { state with CurrentPage = Page.Home }, Cmd.none

    | Some(Router.Route.Blog blogId) ->
        { state with
            CurrentPage = Page.Blog blogId },
        Cmd.none

let init (route: Router.Route option) =
    updateRoute
        route
        { Count = 0
          RandomDeferred = Deferred.HasNotStartedYet
          CurrentPage = Page.NotFound
          CurrentRoute = None }

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none

    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

    | GenerateRandomNumberAsync AsyncMsg.Started ->
        let generateRandom () =
            async {
                do! Async.Sleep 1000
                let randomNumber = random.NextDouble()
                return randomNumber
            }

        let command =
            Cmd.OfAsyncImmediate.perform generateRandom () (AsyncMsg.Completed >> GenerateRandomNumberAsync)

        { state with
            RandomDeferred = Deferred.InProgress },
        command

    | GenerateRandomNumberAsync(AsyncMsg.Completed randomNumber) ->
        { state with
            RandomDeferred = Deferred.Resolved randomNumber },
        Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        prop.classes [ "container"; "mx-auto"; "my-6"; "box" ]
        prop.children [
            Html.div [
                prop.classes [ "block" ]
                prop.children [
                    Html.button [
                        prop.classes [ "button"; "is-small"; "m-1" ]
                        prop.type'.button
                        prop.onClick (fun _ -> dispatch Increment)
                        prop.text "Increment"
                    ]

                    Html.button [
                        prop.classes [ "button"; "is-small"; "m-1" ]
                        prop.type'.button
                        prop.onClick (fun _ -> dispatch Decrement)
                        prop.text "Decrement"
                    ]

                    Html.button [
                        prop.classes [
                            "button"
                            "is-small"
                            "m-1"
                            if state.RandomDeferred = Deferred.InProgress then
                                "is-loading"
                        ]
                        prop.type'.button
                        prop.onClick (fun _ -> dispatch (GenerateRandomNumberAsync AsyncMsg.Started))
                        prop.text "Random Number"
                    ]
                ]
            ]

            Html.div [ prop.classes [ "block" ]; prop.children [ Html.h1 state.Count ] ]

            Html.div [
                prop.classes [ "block" ]
                prop.children [
                    match state.RandomDeferred with
                    | Deferred.HasNotStartedYet -> Html.none
                    | Deferred.InProgress -> Html.h1 "Please wait..."
                    | Deferred.Resolved randomNumber -> Html.h1 randomNumber
                ]
            ]

            Html.div [
                prop.classes [ "block" ]
                prop.children [
                    Html.input [
                        prop.classes [ "input"; "is-small" ]
                        prop.type'.text
                        prop.placeholder "Just an input..."
                    ]
                ]
            ]

            Html.p [
                prop.text (
                    match state.CurrentPage with
                    | Page.Home -> "Home"
                    | Page.Blog blogId -> sprintf "Blog %i" blogId
                    | Page.NotFound -> "Not Found"
                )
            ]
        ]
    ]
