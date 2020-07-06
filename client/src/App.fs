module App

open Feliz
open Elmish
open Elmish.Toastr
open Shared
open System

type LandingPageModel = {
    CollegeCodeString: string
    MockUserPassword: string
    User: Deferred<Result<ValidatedUser, UserLoginError>>
}

type PageModel = 
    | LandingPage of LandingPageModel
    | UserProfilePage of ValidatedUser
    | CourseStudentOverviewPage of ValidatedMockProfessor
with
    member x.CurrentUserCode =
        match x with 
        | LandingPage model -> model.CollegeCodeString
        | _ -> ""

    member x.CurrentUserPassword = 
        match x with 
        | LandingPage model -> model.MockUserPassword
        | _ -> ""

    member x.User =
        match x with 
        | LandingPage model -> model.User
        | _ -> HasNotStartedYet

type Msg =
    | SetCollegeCode of string
    | SetUserPassword of string
    | ShowLoginErrorToastUpdate
    | ShowLoginSuccessToastUpdate
    | ClearTextInput
    | RedirectToProfile
    | InitiateLoginWorkflow
    | ExecuteLoginWorkflow of AsyncOperationStatus<Result<ValidatedUser, UserLoginError>>

let defaultLandingPageState = { CollegeCodeString = "You should put your college code here."; MockUserPassword = ""; User = HasNotStartedYet }

let init() = LandingPage defaultLandingPageState, Cmd.none

let validateUserStringCredentials code password =
    if String.IsNullOrEmpty(code) then
        LoginError MissingCollegeCode
    elif String.IsNullOrEmpty(password) then
        LoginError MissingPassword
    else
        match (code.StartsWith("STU") && code.Length = 8, code.StartsWith("PROF") && code.Length = 9) with 
        | (true, _)->
            OkForNextStage ("STU", code.Substring(3), password)
        | (_, true) ->
            OkForNextStage ("PROF", code.Substring(4), password)
        | _ -> 
            LoginError UnknownUserCode

let update (msg: Msg) (pageModel: PageModel) =
    let clearStateWithError error =
        let model = { CollegeCodeString = ""; MockUserPassword = ""; User = Resolved (Error error)  }
        LandingPage model, Cmd.ofMsg ShowLoginErrorToastUpdate

    let mapPrefixToRole prefixStr =
        match prefixStr with
        | "STU" -> Some Student
        | "PROF" -> Some Professor
        | _ -> None 

    match (pageModel, msg) with
    | (LandingPage model, SetCollegeCode code) -> 
        let updatedModel = { model with CollegeCodeString = code }
        LandingPage updatedModel, Cmd.none

    | (LandingPage model, SetUserPassword pwd) -> 
        let updatedModel = { model with MockUserPassword = pwd }
        LandingPage updatedModel, Cmd.none

    | (LandingPage model, InitiateLoginWorkflow) ->
        LandingPage model, Cmd.ofMsg (ExecuteLoginWorkflow Started)

    |(LandingPage model, ExecuteLoginWorkflow Started) -> 
        let userValidationResult = validateUserStringCredentials model.CollegeCodeString model.MockUserPassword
        match userValidationResult with
        | LoginError error -> 
            clearStateWithError error

        | OkForNextStage (prefix, universalNumber, pwd) ->
            let platformUser = async {
                let userCollegeCode = 
                    prefix
                    |> mapPrefixToRole |> Option.defaultValue Student
                    |> AlphaNumericCode.create prefix universalNumber |> Option.defaultValue defaultCode

                let credentials = { Password = pwd; Code = userCollegeCode }
                try 
                    let! userSignInResult = Server.api.SignIn credentials
                    return ExecuteLoginWorkflow (Finished userSignInResult)
                with error ->
                    Log.developmentError error
                    let loginError = ServerError error.Message
                    return ExecuteLoginWorkflow (Finished (Error loginError))
            }
            
            let updatedModel = { model with User = InProgress }
            LandingPage updatedModel, Cmd.fromAsync platformUser

    | (LandingPage model, ExecuteLoginWorkflow (Finished userSignInResult)) ->
        match userSignInResult with
        | Ok validUser ->
            let updatedModel = { model with User = Resolved (Ok validUser) }
            LandingPage updatedModel, Cmd.ofMsg ShowLoginSuccessToastUpdate
        
        | Error loginError ->
            clearStateWithError loginError 

    | (LandingPage model, ShowLoginErrorToastUpdate) ->
        model.User
        |> Deferred.map (function // T
            | Error error -> 
                Toastr.message error.ErrorMessage
                |> Toastr.title "Login error!!!"
                |> Toastr.position TopRight
                |> Toastr.timeout 5000
                |> Toastr.withProgressBar
                |> Toastr.showCloseButton
                |> Toastr.error
        ) |> ignore
        
        LandingPage { model with User = HasNotStartedYet }, Cmd.ofMsg ClearTextInput

    | (LandingPage model, ShowLoginSuccessToastUpdate) ->
        model.User 
        |> Deferred.map(function 
            |Ok user ->
                Toastr.message (sprintf "Welcome back %s!" user.FullName)
                |> Toastr.title "Login success"
                |> Toastr.position TopRight 
                |> Toastr.timeout 4000
                |> Toastr.showCloseButton
                |> Toastr.success
        )

        LandingPage model, Cmd.ofMsg RedirectToProfile
    
    | (LandingPage model, ClearTextInput) ->
        LandingPage { model with MockUserPassword = ""; CollegeCodeString = "" }, Cmd.none 

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

let render (state: PageModel) (dispatch: Msg -> unit) =

    Html.div [
        prop.style [
            style.textAlign.center
            style.padding 40
        ]

        prop.children [
            Html.img [
                prop.src (StaticFile.import "./imgs/FSharp.V.CSharp.png")
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
                    prop.style [ style.margin 5; style.padding 20; style.width 300 ]
                    prop.onChange (SetCollegeCode >> dispatch)
                    prop.valueOrDefault state.CurrentUserCode
                ]

                Html.br []

                Html.label [
                    prop.style [ style.margin 5; style.padding 20 ]
                    prop.text "Password"
                ]

                Html.input [
                    prop.style [ style.margin 20; style.padding 10; style.width 300 ]
                    prop.type' "password"
                    prop.name "user_password"
                    prop.valueOrDefault state.CurrentUserPassword
                    prop.onChange (SetUserPassword >> dispatch)
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