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
}

type ValidatedMockProfessor = {
    UserInformation: ValidUserInformation
    SemesterCourses: string list
    SupervisedStudentCount: int
}

type ValidatedUser =
    | ValidStudent of ValidatedMockStudent
    | ValidProfessor of ValidatedMockProfessor

/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IServerApi = {
    SignIn: MockUserCredentials -> Result<ValidatedUser, string>
}