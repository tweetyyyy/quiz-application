module Program.fs
open System 

let evaluateMultipleChoice (userAnswer: string) (correctAnswer: string) =
    userAnswer.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase)

let evaluateWrittenAnswerFlexible (userAnswer: string) (correctAnswer: string) =
    let userAnswerTrimmed = userAnswer.Trim().ToLower()
    let correctAnswerTrimmed = correctAnswer.Trim().ToLower()
    userAnswerTrimmed.Contains(correctAnswerTrimmed) || correctAnswerTrimmed.Contains(userAnswerTrimmed)

let calculateScore (userAnswers: UserAnswer list) (questions: Question list) =
    userAnswers
    |> List.fold (fun acc userAnswer ->
        match questions |> List.tryFind (fun q -> q.Id = userAnswer.QuestionId) with
        | Some question ->
            if question.Options.IsEmpty then
                // Written question
                if evaluateWrittenAnswerFlexible userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
            else
                // Multiple-choice question
                if evaluateMultipleChoice userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
        | None -> acc
    ) 0