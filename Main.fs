module App

open Elmish
open Elmish.React
open Feliz

type State =
    { Count: int
      Random: Deferred<double> }

type Msg =
    | Increment
    | Decrement
    | GenerateRandomNumber of AsyncMsg<double>

let random = System.Random()

let init () =
    { Count = 0
      Random = Deferred.HasNotStartedYet },
    Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none

    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

    | GenerateRandomNumber AsyncMsg.Started ->
        let generateRandom () =
            async {
                do! Async.Sleep 1000
                let randomNumber = random.NextDouble()
                return randomNumber
            }

        let command =
            Cmd.OfAsyncImmediate.perform generateRandom () (AsyncMsg.Finished >> GenerateRandomNumber)

        { state with Random = Deferred.InProgress }, command

    | GenerateRandomNumber (AsyncMsg.Finished randomNumber) ->
        { state with Random = Deferred.Resolved randomNumber }, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        prop.className "container mx-auto my-6 box"
        prop.children [
            Html.div [
                prop.classes [ "block" ]
                prop.children [
                    Html.button [
                        prop.classes [
                            "button"
                            "is-small"
                            "m-1"
                        ]
                        prop.onClick (fun _ -> dispatch Increment)
                        prop.text "Increment"
                    ]

                    Html.button [
                        prop.classes [
                            "button"
                            "is-small"
                            "m-1"
                        ]
                        prop.onClick (fun _ -> dispatch Decrement)
                        prop.text "Decrement"
                    ]

                    Html.button [
                        prop.classes [
                            "button"
                            "is-small"
                            "m-1"
                            if state.Random = Deferred.InProgress then
                                "is-loading"
                        ]
                        prop.onClick (fun _ -> dispatch (GenerateRandomNumber AsyncMsg.Started))
                        prop.text "Random Number"
                    ]
                ]
            ]

            Html.div [
                prop.classes [ "block" ]
                prop.children [ Html.h1 state.Count ]
            ]

            Html.div [
                prop.classes [ "block" ]
                prop.children [
                    match state.Random with
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
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withTrace Tracers.console
|> Program.withConsoleTrace
#endif
|> Program.run
