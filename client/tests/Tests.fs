module Tests

open Fable.Mocha
open App

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
        
        let deferredResult = updatedState.User |> Deferred.getOptResolved |> Option.defaultValue (Error (ServerError "An error occured"))
        match deferredResult with
        | Ok _ -> 
            Expect.isFalse false "The password was wrong on purpose. It can never be Ok."
        | Error e -> 
            Expect.isTrue (Deferred.resolved updatedState.User) (sprintf "%s" e.ErrorMessage)

    testCase "Log in valid student" <| fun _ ->
        let update state msg = fst (update msg state)
        let initialState = LandingPage defaultLandingPageState
        let incomingMsgs =  [ SetCollegeCode "STU12345"; SetUserPassword "mockuser"; InitiateLoginWorkflow ]
        let updatedState = List.fold update initialState incomingMsgs

        let counterHasValue n =
            updatedState.Counter
            |> Deferred.exists (function
                | Ok counter -> counter.value = n
                | Error _ -> false)

        Expect.isTrue (counterHasValue 2) "Expected updated state to be 2"
]

let allTests = testList "All" [
    appTests
]

[<EntryPoint>]
let main (args: string[]) = Mocha.runTests allTests