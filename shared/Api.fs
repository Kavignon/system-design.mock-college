module Shared

/// Defines how routes are generated on server and mapped from client
let routerPaths typeName method = sprintf "/api/%s" method

type Counter = { value : int }

type MockCollegeValidUser = 
    | Student
    | Professor
[<AutoOpen>]
module AlphaNumericCode = 
    type AnCode = {
        UserPrefix: string
        UniversalNumber: int
    }

    /// A validation function based on length
    let private lengthValidator len (s:string) =
        s.Length <= len

    let private userPrefixValidator userPrefix userRole =
        match userRole with
        | Student when lengthValidator 3 userPrefix && userPrefix = "STU"-> true
        | Professor when lengthValidator 4 userPrefix && userPrefix = "PROF"-> true
        | _ -> false

    let create userPrefix universalNumber userRole =
        let isUserPrefixValid = userPrefixValidator userPrefix userRole
        let isUniversalNumberValid = lengthValidator 5 universalNumber
        match (isNull userPrefix, isNull universalNumber, isUserPrefixValid, isUniversalNumberValid)  with
        | (false, false, true, true) -> Some { UserPrefix = userPrefix; UniversalNumber = universalNumber |> int }
        | _ -> None

type CollegeCode = {
    Id: AnCode
}

type MockUserCredentials = {
    Username: AlphaNumericCode.AnCode
    Password: string
    
}
/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IServerApi = {
    Counter : unit -> Async<Counter>
}