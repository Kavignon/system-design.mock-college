## Project description

As part of a tech stack evaluation between F# and SAFE versus C# and Blazor (server, not WASM), I thought making a fun project would make it more worthwhile. I'm currently focused on improving my system design and software architecture skills for small to medium-sized projects so this should fulfill this specific need. In this project, we're going to build a college management system for both students and teachers to use. 

_I'll be focused on building a web application, but nothing stops you from forking the project over your Github and make an equivalent mobile application from the work you find here._

## Use cases and constraints outline

### Use cases

#### In scope

* **Service** has high availability

##### Students
- Students connect to their profile
- Students can consult their classes schedule
- Students can register to their courses at the start of a semester
- Students can drop courses before the deadline
- Students can consult their grades
- Students can consult their profile
- Students can upload their work projects to their course
- Students can consult the lecture notes for any given course they're enrolled in
- Students can consult the quizzes, homework, projects and exams schedule for the courses they're enrolled in

##### Teachers
- Teachers connect to their profile
- Teachers can grade student projects
- Teachers can add notes to students during the semesters
- Teachers can see their teaching schedule
- Teachers set the schedule for the projects, homework, exams and quizzes
- Teachers can set the final mark of a student at the end of a semester

#### Out of scope

- Highly secured system
- Handling a high traffic
- Analytics

### Constraints & assumptions

#### State assumptions

##### General 
* Traffic is not evenly distributed
* Need for relational data
* Scale from 1 user to tens of thousands of users
* Grade notifications don't need to be instantaneous
* 20 thousand active users per month
* 12.096 million read requests per month
* 3 million write requests per month
* Up to 2 Mb per write request

##### Students
* Bringing up the course schedule should be fast
* Uploading a homework assignment should be fast
* The service shouldn't crash during the registration period

##### Teachers
* Posting a grade should be fast

#### Calculate usage 
* 12.096 million read requests per month
    * 432k read requests per day
    * 5 read requests per second
* 3 million write request per 30 days month
    * 100k write operation per day
    * 2 Mb per request * 100k per day * 30 days per month = 2 Tb per month
    * 24 Tb in a year

