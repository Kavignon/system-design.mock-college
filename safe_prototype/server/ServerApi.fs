module Server

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Shared
open System

let validUserCodes = ["STU12345"; "STU23456"; "PROF45678"; "PROF7890"]
let validPassword = "mockuser"


/// An implementation of the Shared IServerApi protocol.
/// Can require ASP.NET injected dependencies in the constructor and uses the Build() function to return value of `IServerApi`.
type ServerApi(logger: ILogger<ServerApi>, config: IConfiguration) =
    member this.SignIn (credentials: MockUserCredentials) =
        async {
            logger.LogInformation("Logging the user")
            do! Async.Sleep 4500 // Let's mimick a complex authentification system :)
            let isKnownPlatformUser = (credentials.Username, validUserCodes) ||> List.contains
            match (isKnownPlatformUser, credentials.Password = validPassword) with 
            | (true, true) -> 
                if credentials.Code.IsStudentCode then
                    let userInformation = {
                        CollegeCode = credentials.Code
                        FirstName = "Kevin"
                        LastName = "Avignon"
                        SemesterCourses = [ "Data structures and functional programming"; "F# programming"; "System design and architecture"]
                        StartedWhen = DateTime.Now.AddYears(-5).AddMonths(-3)
                    }
                    let student = { 
                        GPA = { Value = 1.0 }
                        CompletedCreditCount = 45
                        EnrolledCreditCount = 11
                        UserInformation = userInformation
                    }
                    return Ok (ValidStudent student)
                else
                    let userInformation = {
                        CollegeCode = credentials.Code
                        FirstName = "Olivier"
                        LastName = "Avignon"
                        StartedWhen = DateTime.Now.AddYears(-7).AddMonths(4)
                        SemesterCourses = ["Artificial intelligence"; "Machine Learning"; "Language processing" ]
                    }
                    let professor = {
                        UserInformation = userInformation
                        SupervisedStudentCount = 250
                    }
                    return Ok (ValidProfessor professor)

            | (_, false) -> 
                return Error PasswordNotRecognized
            | (false, _) -> 
                return Error UnknownUserCode
        }

    member this.Build() : IServerApi = {
        SignIn = this.SignIn
    }