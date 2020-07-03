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
    with 
        member x.IsStudentCode = x.UserPrefix = "STU"

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

[<AutoOpen>]
module SchoolPerformance = 
    type Gpa = {
        Value: float
    }

    let create value =
        match value with 
        | x when value >= 1.0 && value <= 4.3 -> Some { Value = x }
        | _ -> None

type CollegeCode = {
    Id: AnCode
}

type MockUserCredentials = {
    Code: AlphaNumericCode.AnCode
    Password: string
}
with 
    member x.Username = x.Code.UserPrefix + x.Code.UniversalNumber.ToString()

type ValidUserInformation = {
    CollegeCode: AlphaNumericCode.AnCode
    FirstName: string
    LastName: string
    StartedWhen: DateTime
}

type ValidatedMockStudent = {
    UserInformation: ValidUserInformation
    SemesterCourses: string list
    GPA: SchoolPerformance.Gpa
    CompletedCreditCount: int
    EnrolledCreditCount: int
}

type ValidatedMockProfessor = {
    UserInformation: ValidUserInformation
    SemesterCourses: string list
    SupervisedStudentCount: int
}

let tryMakeFullName (firstName: string) (lastName: string) = 
    if isNull firstName |> not && isNull lastName |> not then
        Some (firstName + " " + lastName)
    else
        None

type ValidatedUser =
    | ValidStudent of ValidatedMockStudent
    | ValidProfessor of ValidatedMockProfessor
with 
    member x.FullName =
        match x with 
        | ValidStudent vs -> (vs.UserInformation.FirstName, vs.UserInformation.LastName) ||> tryMakeFullName |> Option.defaultValue "Name missing"
        | ValidProfessor vp -> (vp.UserInformation.FirstName, vp.UserInformation.LastName) ||> tryMakeFullName |> Option.defaultValue "Name missing"

type UserLoginError =
    | NotCollegeCode 
    | UnknownUserCode
    | PasswordNotRecognized
with 
    member x.ErrorMessage =
        match x with 
        | NotCollegeCode -> "The username provided doesn't match any kind of college code supported by the platform."
        | UnknownUserCode -> "The username provided isn't not recognized in the database. Did you make a mistake?"
        | PasswordNotRecognized -> "The provided password doesn't match any records in the database. Did you make a typo?"


/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IServerApi = {
    SignIn: MockUserCredentials -> Async<Result<ValidatedUser, UserLoginError>>
}