[<RequireQualifiedAccess>]
module Application

open Elmish

open Feliz
open Feliz.Router
open Extensions

[<RequireQualifiedAccess>]
type Url =
    | Home
    | Blog of blogId: int
    | NotFound

[<RequireQualifiedAccess>]
type Page =
    | Home
    | Blog of blogId: int
    | NotFound

type State =
    { Count: int
      RandomDeferred: Deferred<double>
      CurrentUrl: Url
      CurrentPage: Page }

type Msg =
    | Increment
    | Decrement
    | GenerateRandomNumberAsync of AsyncMsg<double>
    | UrlChanged of Url

let random = System.Random()

let init () =
    { Count = 0
      RandomDeferred = Deferred.HasNotStartedYet
      CurrentUrl = Url.NotFound
      CurrentPage = Page.NotFound },
    Cmd.none

let private parseUrl (segments: string list) =
    match segments with
    | [] -> Url.Home
    | [ "blog"; Route.Int id ] -> Url.Blog id
    | _ -> Url.NotFound

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

    | UrlChanged url ->
        match url with
        | Url.Home ->
            { state with
                CurrentUrl = url
                CurrentPage = Page.Home },
            Cmd.none

        | Url.Blog id ->
            { state with
                CurrentUrl = url
                CurrentPage = Page.Blog id },
            Cmd.none

        | Url.NotFound ->
            { state with
                CurrentUrl = url
                CurrentPage = Page.NotFound },
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

            React.router [
                router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
                router.children [
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
        ]
    ]
