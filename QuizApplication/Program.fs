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



    let evaluateAnswer (userAnswer: string) (correctAnswer: string) =
    userAnswer.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase)


let stopWords = Set.ofList ["the"; "a"; "an"; "and"; "of"; "to"; "in"; "for"; "on"; "at"; "by"; "with"]


let punctuationChars = Set.ofList ['.'; ','; ';'; ':'; '!'; '?'; '\''; '"'; '('; ')'; '['; ']'; '{'; '}'; '-'; '_']


let removePunctuation (text: string) =
    text |> String.filter (fun c -> not (punctuationChars.Contains(c)))


let stem (word: string) =
    if word.EndsWith("ing") then word.Substring(0, word.Length - 3)
    elif word.EndsWith("ed") then word.Substring(0, word.Length - 2)
    elif word.EndsWith("es") || word.EndsWith("s") then word.Substring(0, word.Length - 1)
    else word


let tokenizeAndStem (text: string) =
    text
    |> removePunctuation
    |> fun cleanedText ->
        cleanedText.ToLower().Split([| ' '; '\n'; '\t' |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.filter (fun word -> not (stopWords.Contains(word)))
    |> Array.map stem
    |> Set.ofArray


let isAnswerCorrect (userAnswer: string) (correctAnswer: string) =
    let userTokens = tokenizeAndStem userAnswer
    let correctTokens = tokenizeAndStem correctAnswer

    let intersection = Set.intersect userTokens correctTokens
    let similarity = float intersection.Count / float correctTokens.Count

    similarity >= 0.7 

let calculateScore (userAnswers: UserAnswer list) (questions: Question list) =
    userAnswers
    |> List.fold (fun acc userAnswer ->
        match questions |> List.tryFind (fun q -> q.Id = userAnswer.QuestionId) with
        | Some question ->
            if question.Options.IsEmpty then
                if isAnswerCorrect userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
            else
                if evaluateAnswer userAnswer.Answer question.CorrectAnswer then acc + 1 else acc
        | None -> acc
    ) 0