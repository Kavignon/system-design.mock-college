module Tests

open Fable.Mocha
open App
open Shared

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let mapResultToTuple (result: Result<ValidatedUser, UserLoginError>) errorMsg=
    match result with 
    | Ok _ -> (false, errorMsg)
    | Error e -> (true, e.ErrorMessage)

let appTests = testList "App tests" [
    testCase "Just providing credentials doesn't authenticate without clicking on the 'sign in' button" <| fun _ ->
        let update state msg = fst (update msg state)
        let initialState = LandingPage defaultLandingPageState
        let incomingMsgs =  [ SetCollegeCode "STU12345"; SetUserPassword "mockuser"; ]
        let updatedState = List.fold update initialState incomingMsgs
        
        Expect.isFalse (Deferred.resolved updatedState.User) "User must be resolved"

    testCase "You can't be authenticated with the wrong password." <| fun _ ->
        let update state msg = fst (update msg state)
        let initialState = LandingPage defaultLandingPageState
        let incomingMsgs =  [ SetCollegeCode "STU12345"; SetUserPassword "12345"; InitiateLoginWorkflow ]
        let updatedState = List.fold update initialState incomingMsgs
        let optResolvedTuple =  updatedState.User |> Deferred.optMapResolved(fun userState -> mapResultToTuple userState "The password was wrong on purpose. It can never be Ok.")

        match optResolvedTuple with
        | Some (isError, errorMsg) -> 
            Expect.isFalse isError errorMsg
        | None-> 
            ()

    testCase "You can't be authenticated when using an unsupported college code." <| fun _ ->
        let update state msg = fst (update msg state)
        let initialState = LandingPage defaultLandingPageState
        let incomingMsgs =  [ SetCollegeCode "ST12345"; SetUserPassword "mockuser"; InitiateLoginWorkflow ]
        let updatedState = List.fold update initialState incomingMsgs
        let optResolvedTuple =  updatedState.User |> Deferred.optMapResolved(fun userState -> mapResultToTuple userState "The code is wrong. This can't be OK.")
        
        match optResolvedTuple with
        | Some (isError, errorMsg) -> 
            Expect.isFalse isError errorMsg
        | None-> 
            ()

    testCase "Signing in as a valid student." <| fun _ ->
        let update state msg = fst (update msg state)
        let initialState = LandingPage defaultLandingPageState
        let incomingMsgs =  [ SetCollegeCode "STU12345"; SetUserPassword "mockuser"; InitiateLoginWorkflow; ExecuteLoginWorkflow Started ]
        let updatedState = List.fold update initialState incomingMsgs

        Expect.isTrue (Deferred.resolved updatedState.User) "The user was able to log in."
]

let allTests = testList "All" [
    appTests
]

[<EntryPoint>]
let main (args: string[]) =
    #if FABLE_COMPILER
    Mocha.runTests allTests
    #else
    runTestsWithArgs defaultConfig args allTests
    #endif