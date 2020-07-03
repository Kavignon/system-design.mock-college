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
    member this.Counter() =
        async {
            logger.LogInformation("Executing {Function}", "counter")
            do! Async.Sleep 1000
            return { value = 10 }
        }

    member this.SignIn (credentialsOpt: MockUserCredentials option) =
        async {
            logger.LogInformation("Logging the user")
            do! Async.Sleep 4500 // Let's mimick a complex authentification system :)
            return 
                match credentialsOpt with
                | None -> Error NotCollegeCode
                | Some credentials ->
                    let isKnownPlatformUser = (credentials.Username, validUserCodes) ||> List.contains
                    match (isKnownPlatformUser, credentials.Password = validPassword) with 
                    | (true, true) -> 
                        if credentials.Code.IsStudentCode then
                            let userInformation = {
                                CollegeCode = credentials.Code
                                FirstName = "Kevin"
                                LastName = "Avignon"
                                StartedWhen = DateTime.Now.AddYears(-5).AddMonths(-3)
                            }
                            let student = { 
                                SemesterCourses = [ "Data structures and functional programming"; "F# programming"; "System design and architecture"]
                                GPA = defaultGpa
                                CompletedCreditCount = 45
                                EnrolledCreditCount = 11
                                UserInformation = userInformation
                            }
                            Ok (ValidStudent student)
                        else
                            let userInformation = {
                                CollegeCode = credentials.Code
                                FirstName = "Olivier"
                                LastName = "Avignon"
                                StartedWhen = DateTime.Now.AddYears(-7).AddMonths(4)
                            }
                            let professor = {
                                UserInformation = userInformation
                                SemesterCourses = ["Artificial intelligence"; "Machine Learning"; "Language processing" ]
                                SupervisedStudentCount = 250
                            }
                            Ok (ValidProfessor professor)

                    | (_, false) -> Error PasswordNotRecognized
                    | (false, _) -> Error UnknownUserCode
        }

    member this.Build() : IServerApi = {
        SignIn = this.SignIn
    }