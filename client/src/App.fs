module App

open Feliz
open Elmish
open Shared

type LandingPageModel = {
    CollegeCodeString: string
    MockUserPassword: string
    User: Deferred<Result<ValidatedUser, UserLoginError>>
}

type PageModel = 
    | LandingPage of LandingPageModel
    | UserProfilePage of ValidatedUser
    | CourseStudentOverviewPage of ValidatedMockProfessor

type Msg =
    | SetCollegeCode of string
    | SetUserPassword of string
    | InitiateLoginWorkflow
    | ExecuteLoginWorkflow of AsyncOperationStatus<Result<ValidatedUser, UserLoginError>>

let defaultLandingPageState = { CollegeCodeString = ""; MockUserPassword = ""; User = HasNotStartedYet }

let init() = LandingPage defaultLandingPageState , Cmd.none

let update (msg: Msg) (pageModel: PageModel) =
    let clearStateWithError error =
        let model = { CollegeCodeString = ""; MockUserPassword = ""; User = Resolved (Error error)  }
        LandingPage model, Cmd.ofMsg ShowLoginErrorToastUpdate
    match (pageModel, msg) with
    | (LandingPage model, SetCollegeCode code) -> 
        let updatedModel = { model with CollegeCodeString = code }
        LandingPage updatedModel, Cmd.none

    | (LandingPage model, SetUserPassword pwd) -> 
        let updatedModel = { model with MockUserPassword = pwd }
        LandingPage updatedModel, Cmd.none

    | (LandingPage model, InitiateLoginWorkflow) ->
        let updatedModel = { model with User = InProgress}
        LandingPage updatedModel, Cmd.ofMsg (ExecuteLoginWorkflow Started)

        | LoginError error -> 
            clearStateWithError error

        | Error loginError ->
            clearStateWithError loginError


    // Nice try - I won't allow you to do anything that shouldn't happen ;)
    | (_, _) -> pageModel, Cmd.none

let renderAuthenticationResult (deferredAuthResult: Deferred<Result<ValidatedUser, UserLoginError>>)=
    match deferredAuthResult with
    | HasNotStartedYet -> Html.none
    | InProgress -> Html.h1 "Complex authen in progress. Please wait..."
    | Resolved (Ok validUser) -> Html.h1 (sprintf "Welcome back %s! Please wait a few moments, you will be redirected to your profile" validUser.FullName)
    | Resolved (Error errorCode) ->
        Html.h1 [
            prop.style [ style.color.crimson ]
            prop.text errorCode.ErrorMessage
        ]

let csharpvfsharpimage() = StaticFile.import "./imgs/FSharp.V.CSharp2.png"

let render (state: PageModel) (dispatch: Msg -> unit) =

    Html.div [
        prop.style [
            style.textAlign.center
            style.padding 40
        ]

        prop.children [

            Html.img [
                prop.src(csharpvfsharpimage())
                prop.width 500
            ]

            Html.h1 "Welcome to mock college. Please log in."

            Html.form [
                Html.label [ 
                    prop.style [ style.margin 5; style.padding 20 ]
                    prop.text "Mock college code"
                    prop.name "user_college_code"
                ]

                Html.input [
                    prop.style [ style.margin 5; style.padding 20 ]
                    prop.onChange (SetCollegeCode >> dispatch)
                ]

                Html.br []

                Html.label [
                    prop.style [ style.margin 5; style.padding 20 ]
                    prop.text "Password"
                ]

                Html.input [
                    prop.style [ style.margin 20; style.padding 10; ]
                    prop.type' "password"
                    prop.name "user_password"
                    prop.onChange (SetCollegeCode >> dispatch)
                ]
            ]

            Html.button [
                prop.style [ style.margin 5; style.padding 15 ]
                prop.onClick (fun _ -> dispatch InitiateLoginWorkflow)
                prop.text "Sign in"
            ]

            match state with 
            | LandingPage model -> 
                renderAuthenticationResult model.User
            | _ -> Html.none
        ]
    ]